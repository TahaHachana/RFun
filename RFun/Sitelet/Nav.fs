﻿module Website.Nav

open System
open IntelliFactory.Html
open IntelliFactory.WebSharper.Sitelets

let navToggle =
    Button [
        Type "button"
        Class "navbar-toggle"
        HTML5.Data "toggle" "collapse"
        HTML5.Data "target" ".navbar-ex1-collapse"
    ] -< [
        Span [Class "sr-only"] -< [Text "Toggle navigation"]
        Span [Class "icon-bar"]
        Span [Class "icon-bar"]
        Span [Class "icon-bar"]
    ]

let navHeader =
    Div [Class "navbar-header"] -< [
        navToggle
        A [Class "navbar-brand"; HRef "/"] -< [Text "RFun"]
    ]

let li activeLiOption href txt =
    match activeLiOption with
        | Some activeLi when txt = activeLi ->
            LI [Class "active"] -< [
                A [HRef href] -< [Text txt]
            ]
        | _ -> LI [A [HRef href] -< [Text txt]]

let navDiv activeLi ctx =
    Div [Class "collapse navbar-collapse navbar-ex1-collapse"] -< [
        UL [Class "nav navbar-nav"] -< [
            li activeLi "/" "Home"
            li activeLi "/about" "About"
        ]
    ]

let navElt activeLi ctx : Content.HtmlElement =
    HTML5.Nav [
        Class "navbar navbar-inverse navbar-fixed-top"
        NewAttribute "role" "navigation"
    ] -< [
        Div [Class "container"] -< [
            navHeader
            navDiv activeLi ctx
        ]
    ]