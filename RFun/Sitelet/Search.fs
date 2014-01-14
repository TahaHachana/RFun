module Website.Search

open IntelliFactory.WebSharper

module Server =
    open System
    open TankTop
    open TankTop.Dto

    let client = TankTopClient Credentials.indexdenUrl

    let query text start =
        let q = Query text
        q.MatchAnyField <- Nullable(true)
        q.Fetch <- ["*"]
        q.Snippet <- ["description"]
        q.Start <- Nullable start
//        q.Len <- Nullable 5
        q
//        let search = client.Search("WSSnippets", query)

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
                |> Array.map (fun x -> // x.Fields)
                    let desc = subStr x.Fields.["description"]
                    x.DocId, x.Fields.["name"], desc, x.Fields.["package"])
    //                let title = x.Snippets.["title"] |> function "" -> x.Fields.["title"] | t -> t
    //                let description = x.Snippets.["description"] |> function "" -> x.Fields.["description"] | d -> d
    //                x.DocId, title, x.Snippets.["description"], x.Snippets.["code"])
    //                x.DocId, x.Fields.["name"], x.Fields.["description"])
            return (float searchResults.Matches, results)
        }

//    let test = results "plot" 0

[<Inline "encodeURIComponent($uri)">]
let inline encode (uri : string) = X<string>
            
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

    let search' queryStr page =
        async {
            let! results = search queryStr page
            displayResults (snd results)
        } |> Async.Start

//    let prevLi pageId queryStr =
//        match pageId with
//        | 1 -> LI [Attr.Class "disabled"] -< [link "#" "«"]
//        | _ ->
//            LI [Text "«"] -< [link "#" "«"]
//            |>! OnClick(fun _ _ -> search' queryStr pageId)
//
//    let nextLi pageId pagesLength queryStr =
//        match pageId with
//        | _ when pageId = pagesLength -> LI [Attr.Class "disabled"] -< [link "#" "»"]
//        | _ ->
//            LI [link "#" "»"]
//            |>! OnClick(fun _ _ -> search' queryStr pageId)


    let pageLi x pageId queryStr =
        let xStr = string x
        let li = LI [Attr.Class "page"] -< [link "#" xStr]
        li.Dom.AddEventListener("click", (fun (e : Dom.Event) ->
            e.PreventDefault()
            search' queryStr x
            JQuery.Of(".page").RemoveClass("active").Ignore
            li.AddClass("active")
            ), false)
        li

//        match x with
//        | _ when x = pageId ->
//            let li = LI [Attr.Class "page"] -< [link "#" xStr]
//            li.Dom.AddEventListener("click", (fun (e : Dom.Event) ->
//                e.PreventDefault()
//                search' queryStr x
//                JQuery.Of(".page").RemoveClass("active").Ignore
//                li.AddClass("active")
//                ), false)
//            li
//        | _ ->
//            let li = LI [Attr.Class "page"] -< [link "#" xStr]
//            li.Dom.AddEventListener("click", (fun (e : Dom.Event) ->
//                e.PreventDefault()
//                search' queryStr x), false)
//            li

    let pagesUl pageId queryStr pages length =
        UL [Attr.Class "pagination"] -< [
//            yield prevLi pageId queryStr
            yield! Array.map (fun x -> pageLi x pageId queryStr) pages
//            yield nextLi pageId length queryStr
        ]

    let paginationDiv matches pageId queryStr =
        let pages =
            matches / 10. |> ceil
            |> int |> fun x -> [|1 .. x|]
        let length = pages.Length
        let pages' = Seq.truncate 10 pages |> Seq.toArray //Seq.take 10 pages |> Seq.toArray
//            match length with
//            | l when l < 11 -> pages
//            | _ ->
//                pages//.[(pageId - 5) .. ]
//                |> Seq.truncate 10
//                |> Seq.toArray
        let div =
            match length with
                | 1 -> Div []
                | _ ->
                    Div [Attr.Class "row"] -< [
                        pagesUl pageId queryStr pages' length
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
                    Button [Attr.Class "btn btn-primary"; Attr.Type "button"; Id "search-btn-2"] -< [Text "Search"]
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
                    let alert = Div [Attr.Class "alert alert-danger"; Text "Your search did not match any functions."; Attr.Style "width: 30%;"]
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
    
    let main() =
        let inp =
            Input [Attr.Id "query-1"; Attr.Type "text"; Attr.Class "query form-control input-lg"; HTML5.Attr.AutoFocus "autofocus"; HTML5.Attr.PlaceHolder "R Function"] //; Attr.Style "font-size: 30px; height: 40px"]
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
                        Button [Id "search-btn-1"; Text "Search"; Attr.Type "button"; Attr.Class "btn btn-primary btn-lg"] //; Attr.Style "height: 50px; font-size: 20px;"]
                        |>! OnClick (fun _ _ ->
                            let q = inp.Value.Trim()
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
                                    let alert = Div [Attr.Class "alert alert-danger col-lg-offset-3 col-lg-6"; Text "Your search did not match any functions."]
                                    JQuery.Of("#results").Append alert.Dom |> ignore
                                | 1. ->
                                    let f, _, _, _ = Array.get results 0
                                    Window.Self.Location.Assign("/function/" + f)
                                | _ ->
                                    JQuery.Of("#progress").Hide().Ignore
                                    let form = searchForm()
                                    JQuery.Of(".jumbotron").ReplaceWith(form.Dom).Ignore
//                                    JQuery.Of("#search-form").Show().Ignore
                                    JQuery.Of("#query-2").Val(q).Ignore
                                    displayResults results
                                    paginationDiv matches 1 q
                                    JQuery.Of(".page").First().AddClass("active").Ignore
                                    bindClick()
//                                    JQuery.Of("#search").Click((fun _ _ ->
//                                        async {
//                                            JQuery.Of("#results").Empty().Ignore
//                                            JQuery.Of("#pagination").Empty().Ignore
//                                            let! searchResults = search (JQuery.Of("#query2").Val() |> string) 1
//                                            let matches = fst searchResults
//                                            let results = snd searchResults
//                                            match matches with
//                                            | 0. ->
////                                                JQuery.Of("#progress").Hide().Ignore
//                                                let alert = Div [Attr.Class "alert alert-danger col-lg-offset-3 col-lg-6"; Text "Your search did not match any functions."]
//                                                JQuery.Of("#results").Append alert.Dom |> ignore
//                                            | 1. ->
//                                                let f, _, _, _ = Array.get results 0
//                                                Window.Self.Location.Assign("/function/" + f)
//                                            | _ ->
////                                                JQuery.Of("#progress").Hide().Ignore
////                                                JQuery.Of(".jumbotron").Remove().Ignore
////                                                JQuery.Of("#search-form").Show().Ignore
////                                                JQuery.Of("#query2").Val(q).Ignore
//                                                displayResults results
//                                                paginationDiv matches 1 (JQuery.Of("#query2").Val() |> string)
//                                                JQuery.Of(".page").First().AddClass("active").Ignore
//                                            } |> Async.Start
//                                        )).Ignore
                            } |> Async.Start
//                  
//              let count, results = Server.results q 0
//                            JavaScript.Log count
//                            let lis =
//                                results
//                                |> Array.map (fun (docId, name, desc, pack) ->
//                                    LI [Attr.Class "list-group-item"] -< [
//                                        H3 [
//                                            A [HRef <| "/function/" + docId] -< [
//                                                Text name
//                                            ]
//                                        ]
//                                        Div [Text desc]
//                                        H4 [Text <| "Package: " + pack]
//                                    ])
//                            let ul = UL [] -< lis
//                            JQuery.Of("#results").Empty().Append ul.Dom |> ignore
                            )

//                            Window.Self.Location.Href <- "/search/" + q + "/1")
                    ]
                ]
                Script [Attr.Src "/Scripts/AutoComplete.js"]
//                datalist
            ]
        ]
type Control() =
    inherit Web.Control()

    [<JavaScript>]
    override __.Body = Client.main() :> _

