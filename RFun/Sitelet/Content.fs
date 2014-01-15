module Website.Content

open IntelliFactory.Html
open IntelliFactory.WebSharper.Sitelets
open Nav
open System.Text.RegularExpressions

module Home =

    let jumbotron =
        Div [Class "jumbotron"] -< [
            Div [Class "container text-center"] -< [
                H1 [Text "RFun"] -< [
                    Text " "
                    Small [Text "Beta"]
                ]:> INode<_>
                new Search.Control() :> _
            ]
        ]

    let body ctx =
        Div [Id "wrap"] -< [
            navElt (Some "Home") ctx
            Div [Class "container"; Id "main"] -< [
                Div [Id "progress"; Class "col-lg-offset-3 col-lg-4 alert"] -< [
                    Div [Class "progress progress-striped active"] -< [
                        Div [Class "progress-bar"; Style "width: 100%;"]
                    ]
                ]
                jumbotron
                Div [Class "row"; Id "results"]
                Div [Id "pagination"]
            ]
            Div [Id "push"]
        ]

module About =
    let body ctx =
        Div [Id "wrap"] -< [
            navElt (Some "About") ctx
            Div [Class "container"; Id "main"] -< [
                Div [Class "page-header"] -< [
                    H1 [Text "About"]
                ]
                Div [
                    P [Class "lead"] -< [
                        Text "RFun is a search engine built to ease looking up functions arguments when working with "
                        A [HRef "https://github.com/BlueMountainCapital/FSharpRProvider"; Target "_blank"] -< [Text "RProvider"]
                        Text ", F#'s type provider for interoperating with the R language for statistical computing."
                    ]
                    P [Class "lead"] -< [
                        Text "RFun is powered by the "
                        A [HRef "http://www.websharper.com/"; Target "_blank"] -< [Text "WebSharper"]
                        Text " framework."
                    ]
                ]
            ]
            Div [Id "push"]
        ]


      
module Function =
    
    let keywordRegex = Regex("^(#else|#endif|#help|#I|#if|#light|#load|#quit|#r|#time|abstract|and|as|assert|base|begin|class|default|delegate|do|done|downcast|downto|elif|else|end|exception|extern|false|finally|for|fun|function|global|if|in|inherit|inline|interface|internal|lazy|let|let!|match|member|module|mutable|namespace|new|not|null|of|open|or|override|private|public|rec|return|return!|select|static|struct|then|to|true|try|type|upcast|use|use!|val|void|when|while|with|yield|yield!)$",RegexOptions.Compiled)

    let safeName (name: string) =
        name.Replace("_","__").Replace(".", "_")
        |> fun x -> keywordRegex.Replace(x, "``"+ x + "``")
    
    let title (f:Mongo.Function) = "R " + f.Name + " Function"

    let metaDesc (f:Mongo.Function) = "Arguments information for the " + f.Name + " R function."

    let argTr (arg:Mongo.Arg) =
        TR [
            TD [Text arg.Name]
            TD [Text arg.Description]
        ]

    let argsTableDiv (args:Mongo.Arg []) args' =
        match args' with
        | [||] -> Div [Class "row"]
        | _ ->
            Div [Class "row"] -< [
                Div [Class "panel panel-default"] -< [
                    Div [Class "panel-heading"] -< [Text "Arguments"]
                    Div [Class "panel-body"] -< [
                        Table [Class "table table-striped"; Id "args-table"] -< [
                            TR [
                                TH [Text "Argument"]
                                TH [Text "Details"]
                            ]
                            -< Array.map argTr args
                        ]
                    ]
                ]
            ]

    let fsharpCode name (args':Mongo.Arg []) =
        let name = safeName name
        match args' with
        | [||] -> "<pre>R." + name
        | _ ->
            let spans =
                args'
                |> Array.map (fun arg ->
                    "    " + "<span style=\"color: red;\">\"" + arg.Name + "\"</span>, " + arg.Name)
                |> String.concat "\n"
            String.concat
                "\n"
                [
                    yield "<pre>namedParams ["
                    yield spans + " ]"
                    yield "|> R." + name + "</pre>"
                ]
        |> Element.VerbatimContent
    
    let body ctx (f:Mongo.Function) =
        let args = f.Args
        let args' = args |> Array.filter (fun arg -> arg.Name <> "...")
        let argsDiv = argsTableDiv args args'
        let code = fsharpCode f.Name args'
        Div [Id "wrap"] -< [
            navElt None ctx
            Div [Class "container"; Id "main"] -< [
                Div [Class "page-header"] -< [
                    H1 [Text f.Name]
                    Small [Text <| "Package: " + f.Package]
                ]
                Div [Class "row"] -< [
                    P [Class "lead"] -< [Text f.Description]
                ]
                argsDiv
                Div [Class "row"] -< [
                    Div [Class "panel panel-default"] -< [
                        Div [Class "panel-heading"] -< [Text "Usage"]
                        Div [Class "panel-body"] -< [
                            Div [Class "col-lg-6"] -< [
                                H3 [Text "F#"]
                                code
                            ]
                            Div [Class "col-lg-6"] -< [
                                H3 [Text "R"]
                                Pre [Text f.Usage]
                            ]
                        ]
                    ]

                ]
                Div [Class "row"] -< [
                    Div [Class "panel panel-default"] -< [
                        Div [Class "panel-heading"] -< [Text "Reference"]
                        Div [Class "panel-body"] -< [
                            A [HRef f.Reference; Target "_blank"] -< [Text f.Reference]
                        ]
                    ]
                ]
            ]
            Div [Id "push"]
        ]

module Error =
    let body ctx =
        Div [Id "wrap"] -< [
            navElt None ctx
            Div [Class "container"; Id "main"] -< [
                Div [Class "page-header"] -< [
                    H1 [Text "Error"]
                    P [Text "The requested URL doesn't exist."]
                ]
            ]
            Div [Id "push"]
        ]