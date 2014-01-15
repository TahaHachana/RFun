module Website.Controller

open IntelliFactory.WebSharper.Sitelets
open Model

//let protect view =
//    match UserSession.GetLoggedInUser() with
//    | None -> Content.Redirect <| Login None
//    | _ -> view

let logout() =
    UserSession.Logout()
    Content.Redirect Home
    
let controller =
    let handle =
        function
        | About -> Views.about
        | Error -> Views.error
        | Function id -> Views.``function`` id
        | Home -> Views.home

    { Handle = handle }