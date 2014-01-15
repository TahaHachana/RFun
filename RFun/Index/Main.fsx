module Main

#r "System.Net.Http"
#r "..\packages\FSharp.Http.1.0.0\lib\Net45\FSharp.Http.dll"
#r "..\packages\HtmlAgilityPack.1.4.6\lib\Net45\HtmlAgilityPack.dll"
#r "..\packages\XTract.0.1.24\lib\Net45\XTract.dll"
#load "Throttler.fsx"
#load "Credentials.fsx"
#load "Indexden.fsx"
#load "Mongo.fsx"

open FSharp.Http
open System.Collections.Concurrent
open System.Text.RegularExpressions
open XTract

[<CLIMutable>]
type Function =
    {
        Name : string
        Args : Arg []
        Description : string
        Reference : string
        Package : string
        Usage : string
    }

    static member New name args description reference package usage =
        {
            Name = name
            Args = args
            Description = description
            Reference = reference
            Package = package
            Usage = usage
        }
        
and [<CLIMutable>] Arg =
    {
        Name : string
        Description : string
    }

    static member New name description =
        {
            Name = name
            Description = description
        }

let libraryUrl = "http://stat.ethz.ch/R-manual/R-devel/library/"

let library =
    Http.GetString libraryUrl
    |> Option.get

let packageUrlRegex = Regex.compile "href=\"([^/]+/)\""
let functionUrlRegex = Regex.compile "href=\"(.+?\.html)\""
let argumentsRegex = Regex.compile "<h3>Arguments</h3>"
let functionRegex = Regex.compile "([^ ]+)\(((?:[^()]|(?<open>\()|(?<-open>\)))+(?(open)(?!)))\)"
let descRegex = Regex.compile "(?s)<h3>Description</h3>(.+?)<h3>Usage</h3>"
let packageRegex = Regex.compile "library/([^/]+)/"

let packageUrl m = String.concat "" [libraryUrl; Regex.matchGroup m 1; "html/"]
let functionUrl url m = String.concat "" [url; Regex.matchGroup m 1]

let packagesUrls =
    packageUrlRegex.Matches library
    |> Seq.cast<Match>
    |> Seq.map packageUrl
    |> Seq.toArray

let functionsUrlsBag = ConcurrentBag<string>()

let functionsUrls (packageUrl:string) =
    async {
        let! html = Http.AsyncGetString packageUrl
        match html with
        | None -> ()
        | Some value ->
            Regex.matches functionUrlRegex value
            |> Seq.cast<Match>
            |> Seq.map (fun x -> functionUrl packageUrl x)
            |> Seq.toArray
            |> Array.filter (fun x -> x.EndsWith "00html" = false)
            |> Array.iter functionsUrlsBag.Add
    }

let functionsBag = ConcurrentBag<Function>()

let isFunction html = argumentsRegex.IsMatch html

let descText html =
    Regex.regexGroup descRegex 1 html
    |> String.decodeHtml
    |> String.stripTags
    |> String.stripSpaces
    |> fun x -> x.Trim()

let preText htmlDoc =
    Node.select htmlDoc "/html/body/pre[1]"
    |> Node.innerText
    |> Option.get

let packageName url = Regex.regexGroup packageRegex 1 url

let parseFunction m description reference =
    let name = Regex.matchGroup m 1
    let args =
        Regex.matchGroup m 2
        |> fun x -> x.Split ','
        |> Array.map (fun x ->
            x.Split '='
            |> fun x -> Array.get x 0
            |> fun x -> Arg.New (x.Trim()) "")
    let package = packageName reference
    Function.New name args description reference package m.Value

let rec tuplesList lst =
    [|
        let length = Seq.length lst
        match length with
        | 0 -> ()
        | _ ->
            let tuple = Seq.take 2 lst    
            yield Seq.nth 0 tuple, Seq.nth 1 tuple
            let lst' = Seq.skip 2 lst
            yield! tuplesList lst'
    |]

let argInfo (tds:HtmlAgilityPack.HtmlNode*HtmlAgilityPack.HtmlNode) =
    let names, desc = tds
    let desc' =
        desc.InnerText
        |> String.decodeHtml
        |> String.stripSpaces
        |> fun x -> x.Trim()
    names.InnerText.Split ','
    |> Array.map (fun x -> Arg.New (x.Trim()) desc')

let scrapeArgs htmlDoc =
    Node.selectList htmlDoc "/html/body/table[2]/tr/td"
    |> Option.get
    |> tuplesList
    |> Array.map argInfo
    |> Array.concat

let updateArgs f args =
    let fArgs = f.Args
    let args' =
        Array.filter
            (fun arg ->
                Array.exists
                    (fun (fArg:Arg) -> fArg.Name = arg.Name)
                    fArgs)
            args
    {f with Args = args'}

let collectFunctions html reference =
    let htmlDoc = Html.load html
    let description = descText html
    let pre = preText htmlDoc
    let functions =
        Regex.matches functionRegex pre
        |> Seq.cast<Match>
        |> Seq.toArray
        |> Array.map (fun x -> parseFunction x description reference)
    let args = scrapeArgs htmlDoc
    functions
    |> Array.iter (fun f ->
        updateArgs f args
        |> functionsBag.Add)

let printer =
    MailboxProcessor<string>.Start(fun inbox ->
        let rec loop() =
            async {
                let! msg = inbox.Receive()
                printfn "%s" msg
                return! loop()
            }
        loop())

let scrapeFunctions (url:string) =
    async {
        let msg = sprintf "Crawling: %s" url
        printer.Post msg
        try
            let! html = Http.AsyncGetString url
            match html with
            | None -> ()
            | Some value -> 
                match isFunction value with
                | false -> ()
                | true -> collectFunctions value url
        with _ -> ()
    }

let h =
    async {
        printfn "Done"
    }

let indexFunction idx (f:Function) =
    async {
        let name = f.Name
        let msg = sprintf "Indexing: %s" name
        printer.Post msg
        let idx' = idx + 1
        let package, description, usage = f.Package, f.Description, f.Usage
        Indexden.addDoc (string idx') name package description
        let args' = f.Args |> Array.map (fun arg -> Mongo.Arg.New arg.Name arg.Description)
        let f' = Mongo.MongoFunction.New idx' name args' description f.Reference package usage
        Mongo.functions.Insert f' |> ignore
        ()
    }
    
let g = 
    async {
        let asyncs = functionsBag.ToArray() |> Array.mapi indexFunction
        Throttler.throttle asyncs 5 h
    }

let f =
    async {
        let asyncs = functionsUrlsBag.ToArray() |> Array.map scrapeFunctions
        Throttler.throttle asyncs 5 g        
    }

let asyncs = Array.map functionsUrls packagesUrls

Throttler.throttle asyncs 5 f
