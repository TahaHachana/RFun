module Website.Model

type PageId = int

type Action =
    | About
    | [<CompiledName("function")>] Function of PageId
//    | Admin
    | Error
    | Home
//    | Login of Action option
//    | Logout
//    | [<CompiledName("sub")>] Sub of PageId