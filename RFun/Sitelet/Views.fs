module Website.Views

open Content
open Model
open Skin

let home =
    withTemplate<Action>
        Templates.home
        "RFun"
        "A search engine for looking up R functions arguments when working with RProvider."
        <| fun ctx -> Home.body ctx

let about =
    withTemplate<Action>
        Templates.about
        "About RFun"
        "About RFun, the R functions arguments search engine."
        <| fun ctx -> About.body ctx

let ``function`` id =
    let f = Mongo.byId id
    withTemplate<Action>
        Templates.``function``
        (Function.title f)
        (Function.metaDesc f)
        <| fun ctx -> Function.body ctx f

let error =
    withTemplate<Action>
        Templates.error
        "Error - Page Not Found"
        ""
        <| fun ctx -> Error.body ctx