module Website.Model

type PageId = int

type Action =
    | About
    | Error
    | [<CompiledName("function")>] Function of PageId
    | Home