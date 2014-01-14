module Website.Content

open IntelliFactory.Html
open IntelliFactory.WebSharper.Sitelets
open Nav

module Home =

    let jumbo =
        Div [Class "jumbotron"] -< [
            Div [Class "container text-center"] -< [
                H1 [Text "RFun"] :> INode<_>
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
//                searchForm
                jumbo
                Div [Class "row"; Id "results"]
                Div [Id "pagination"]

//                HR []
//                Div [Class "row"] -< [
////                    colDiv ctx 1
////                    colDiv ctx 2
////                    colDiv ctx 3
//                ]
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

open System.Text.RegularExpressions

let keywordRegex   = Regex("^(#else|#endif|#help|#I|#if|#light|#load|#quit|#r|#time|abstract|and|as|assert|base|begin|class|default|delegate|do|done|downcast|downto|elif|else|end|exception|extern|false|finally|for|fun|function|global|if|in|inherit|inline|interface|internal|lazy|let|let!|match|member|module|mutable|namespace|new|not|null|of|open|or|override|private|public|rec|return|return!|select|static|struct|then|to|true|try|type|upcast|use|use!|val|void|when|while|with|yield|yield!)$",RegexOptions.Compiled)

let safeName (name: string) =
    name.Replace("_","__").Replace(".", "_")
    |> fun x -> keywordRegex.Replace(x, "``"+ x + "``")
      
module Function =
    
    let title (f:Mongo.Function) = "R " + f.Name + " Function"

    let metaDesc (f:Mongo.Function) = "Arguments information for the " + f.Name + " R function."
    
    let body ctx (f:Mongo.Function) =
        let trs =
            f.Args
            |> Array.map (fun arg ->
                TR [
                    TD [Text arg.Name]
                    TD [Text arg.Description]
                ])
        let fCode =
            let name = safeName f.Name
            let args =
                f.Args
                |> Array.map (fun arg ->
                    "    " + "<span style=\"color: red;\">\"" + arg.Name + "\"</span>, " + arg.Name)
                |> String.concat "\n"
            String.concat
                "\n"
                [
                    yield "<pre>namedParams ["
                    yield args + " ]"
                    yield "|> R." + name + "</pre>"
                ]
            |> Element.VerbatimContent
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
                Div [Class "row"] -< [
                    Div [Class "panel panel-default"] -< [
                        Div [Class "panel-heading"] -< [Text "Arguments"]
                        Div [Class "panel-body"] -< [
                            Table [Class "table table-striped"; Id "args-table"] -< [
                                TR [
                                    TH [Text "Argument"]
                                    TH [Text "Details"]
                                ]
                                -< trs
                            ]
                        ]
                    ]
                ]
                Div [Class "row"] -< [
                    Div [Class "panel panel-default"] -< [
                        Div [Class "panel-heading"] -< [Text "Usage"]
                        Div [Class "panel-body"] -< [
                            Div [Class "col-lg-6"] -< [
                                H3 [Text "F#"]
                                fCode
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

//module Sub =
//    let body ctx pageId =
//        Div [Id "wrap"] -< [
//            navElt None ctx
//            Div [Class "container"; Id "main"] -< [
//                Div [Class "page-header"] -< [
//                    H1 [Text <| "Sub Page " + pageId]
//                ]
//            ]
//            Div [Id "push"]
//        ]
//
//module Login =
//    let body action action' ctx =
//        let link =
//            match action with
//            | Some action -> action
//            | None -> action'
//            |> ctx.Link
//        Div [Id "wrap"] -< [
//            navElt None ctx
//            Div [Class "container"; Id "main"] -< [
//                Div [new Login.Control(link)]
//            ]
//        ]
//
//module Admin =
//    let body ctx =
//        Div [Id "wrap"] -< [
//            navElt None ctx
//            Div [Class "container"; Id "main"] -< [
//                Div [Class "page-header"] -< [
//                    H1 [Text "Admin Page"]
//                ]
//            ]
//            Div [Id "push"]
//        ]

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