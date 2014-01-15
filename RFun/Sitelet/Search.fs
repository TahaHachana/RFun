module Website.Search

open IntelliFactory.WebSharper

module Server =
    open System
    open TankTop
    open TankTop.Dto

    let client = TankTopClient Credentials.indexdenUrl

    let query text start =
        let q = Query ("name:" + text)
        q.Fetch <- ["*"]
        q.Snippet <- ["description"]
        q.Start <- Nullable start
        q

    let subStr (str:string) =
        match str.Length with
        | length when length < 150 -> str
        | _ -> str.[..147] + "..."
    
    [<Remote>]    
    let results q start =
        async {
            let q' = query q start
            let searchResults = client.Search("RFun", q')
            let results =
                searchResults.Results
                |> Seq.toArray
                |> Array.map (fun x ->
                    let desc = subStr x.Fields.["description"]
                    x.DocId, x.Fields.["name"], desc, x.Fields.["package"])
            return (float searchResults.Matches, results)
        }

[<Inline "window.scrollTo(0, 0)">]
let scroll() = X<unit>

[<JavaScript>]
module private Client =
    open IntelliFactory.WebSharper.Html
    open IntelliFactory.WebSharper.Html5
    open IntelliFactory.WebSharper.JQuery

    let link href text = A [HRef href] -< [Text text]

    let search queryStr page =
        async {
            let start = (page - 1) * 10
            return! Server.results queryStr start
        }

    let displayResults results =
        let lis =
            results
            |> Array.map (fun (docId, name, desc, pack) ->
                LI [Attr.Class "list-group-item"] -< [
                    H3 [Attr.Class "list-group-item-heading"] -< [
                        A [HRef <| "/function/" + docId; Attr.Target "_blank"] -< [
                            Text name
                        ]
                    ]
                    Div [Text desc]
                    H4 [Text <| "Package: " + pack]
                ])
        let ul = UL [Attr.Class "col-lg-8"] -< lis
        JQuery.Of("#results").Empty().Append(ul.Dom).Ignore
        scroll()
        

    let search' queryStr page =
        async {
            let! results = search queryStr page
            displayResults (snd results)
        } |> Async.Start

    let pageLi x queryStr =
        let xStr = string x
        let li = LI [Attr.Class "page"] -< [link "#" xStr]
        li.Dom.AddEventListener("click", (fun (e : Dom.Event) ->
            e.PreventDefault()
            search' queryStr x
            JQuery.Of(".page").RemoveClass("active").Ignore
            li.AddClass("active")
            ), false)
        li


    let pagesUl queryStr pages =
        UL [Attr.Class "pagination"] -< [
            yield! Array.map (fun x -> pageLi x queryStr) pages
        ]

    let paginationDiv matches pageId queryStr =
        let pages =
            matches / 10. |> ceil
            |> int |> fun x -> [|1 .. x|]
        let length = pages.Length
        let pages' = Seq.truncate 10 pages |> Seq.toArray
        let div =
            match length with
                | 1 -> Div []
                | _ ->
                    Div [Attr.Class "row"] -< [
                        pagesUl queryStr pages'
                    ]
        JQuery.Of("#pagination").Append div.Dom |> ignore

    let searchForm() =
        Div [Attr.Class "row"; Id "search-form"] -< [
            Div [Attr.Class "input-group input-group-lg"] -< [
                Span [Attr.Class "input-group-addon"] -< [Text "RFun"]
                Input [Attr.Type "text"; Attr.Class "form-control query"; Id "query-2"]
                |>! OnKeyUp (fun _ key ->
                    match key.KeyCode with
                        | 13 ->
                            JQuery.Of("#query-2").Blur().Ignore
                            JQuery.Of("#search-btn-2").Click().Ignore
                        | _  -> ())           
                Span [Attr.Class "input-group-btn"] -< [
                    Button [
                        Attr.Class "btn btn-primary"
                        Attr.Type "button"
                        Id "search-btn-2"
                    ] -< [Text "Search"]
                ]
            ]
            Script [Attr.Src "/Scripts/AutoComplete.js"]
        ]

    let bindClick() =
        JQuery.Of("#search-btn-2").Click((fun _ _ ->
            async {
                JQuery.Of("#results").Empty().Ignore
                JQuery.Of("#pagination").Empty().Ignore
                JQuery.Of("#progress").Show().Ignore
                let queryStr = JQuery.Of("#query-2").Val() |> string
                let! searchResults = search queryStr 1
                let matches = fst searchResults
                let results = snd searchResults
                match matches with
                | 0. ->
                    JQuery.Of("#progress").Hide().Ignore
                    let alert =
                        Div [
                            Attr.Class "alert alert-danger"
                            Text "Your search did not match any functions."
                            Attr.Style "width: 30%;"
                        ]
                    JQuery.Of("#results").Append alert.Dom |> ignore
                | 1. ->
                    let f, _, _, _ = Array.get results 0
                    Window.Self.Location.Assign("/function/" + f)
                | _ ->
                    JQuery.Of("#progress").Hide().Ignore
                    displayResults results
                    paginationDiv matches 1 queryStr
                    JQuery.Of(".page").First().AddClass("active").Ignore
                } |> Async.Start
                )).Ignore

    let handleClick q =
        async {
            JQuery.Of("#progress").Show().Ignore
            JQuery.Of("#results").Empty().Ignore
            JQuery.Of("#pagination").Empty().Ignore
            let! searchResults = search q 1
            let matches = fst searchResults
            let results = snd searchResults
            match matches with
            | 0. ->
                JQuery.Of("#progress").Hide().Ignore
                let alert =
                    Div [
                        Attr.Class "alert alert-danger col-lg-offset-3 col-lg-6"
                        Text "Your search did not match any functions."
                    ]
                JQuery.Of("#results").Append alert.Dom |> ignore
            | 1. ->
                let f, _, _, _ = Array.get results 0
                Window.Self.Location.Assign("/function/" + f)
            | _ ->
                JQuery.Of("#progress").Hide().Ignore
                let form = searchForm()
                JQuery.Of(".jumbotron").ReplaceWith(form.Dom).Ignore
                JQuery.Of("#query-2").Val(q).Ignore
                displayResults results
                paginationDiv matches 1 q
                JQuery.Of(".page").First().AddClass("active").Ignore
                bindClick()
        } |> Async.Start
                                
    let main() =
        let inp =
            Input [
                Attr.Id "query-1"
                Attr.Type "text"
                Attr.Class "query form-control input-lg"
                HTML5.Attr.AutoFocus "autofocus"
                HTML5.Attr.PlaceHolder "R Function"
            ]
            |>! OnKeyUp (fun _ key ->
                match key.KeyCode with
                    | 13 ->
                        JQuery.Of("#query-1").Blur().Ignore                        
                        JQuery.Of("#search-btn-1").Click().Ignore
                    | _  -> ())           
        Div [Attr.Class "row"] -< [
            Div [Attr.Class "col-lg-6 col-lg-offset-3"] -< [
                Div [Attr.Class "input-group input-group-lg"] -< [
                    inp
                    Span [Attr.Class "input-group-btn"] -< [
                        Button [
                            Id "search-btn-1"
                            Text "Search"
                            Attr.Type "button"
                            Attr.Class "btn btn-primary btn-lg"
                        ]
                        |>! OnClick (fun _ _ ->
                            let q = inp.Value.Trim()
                            handleClick q)
                    ]
                ]
                Script [Attr.Src "/Scripts/AutoComplete.js"]
            ]
        ]
type Control() =
    inherit Web.Control()

    [<JavaScript>]
    override __.Body = Client.main() :> _