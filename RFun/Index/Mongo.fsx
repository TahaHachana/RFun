module Mongo

#r """..\packages\mongocsharpdriver.1.8.3\lib\net35\MongoDB.Bson.dll"""
#r """..\packages\mongocsharpdriver.1.8.3\lib\net35\MongoDB.Driver.dll"""
#load "Credentials.fsx"

open System
open System.Globalization
open System.Linq
open MongoDB.Bson
open MongoDB.Driver
open MongoDB.Driver.Builders

let culture = CultureInfo.CreateSpecificCulture "en-US"
CultureInfo.DefaultThreadCurrentCulture <- culture

let client = MongoClient Credentials.mongo
let server = client.GetServer()
let db = server.GetDatabase "RFun"
let collection<'T> (name : string) = db.GetCollection<'T> name

[<CLIMutable>]
type MongoFunction =
    {
        _id : ObjectId
        FunId : int
        Name : string
        Args : Arg []
        Description : string
        Reference : string
        Package : string
        Usage : string
    }

    static member New id name args description reference package usage =
        {
            _id = ObjectId.GenerateNewId()
            FunId = id
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

let functions  = collection<MongoFunction> "functions"
//let queryable = functions.FindAll().AsQueryable()

//let insert f = functions.Insert f |> ignore
//
//let args = [|{Name = "x"; Description = ""}|]
//let testf = MongoFunction.New 1 "plot" args "desc" "ref" "pack"
//
//insert testf
