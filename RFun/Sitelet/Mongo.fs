module Website.Mongo

open System
open System.Globalization
open System.Linq
open MongoDB.Bson
open MongoDB.Driver
open MongoDB.Driver.Builders

let client = MongoClient Credentials.mongo
let server = client.GetServer()
let db = server.GetDatabase "RFun"
let collection<'T> (name : string) = db.GetCollection<'T> name

[<CLIMutable>]
type Function =
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
        
and [<CLIMutable>] Arg =
    {
        Name : string
        Description : string
    }

let functions  = collection<Function> "functions"
let queryable = functions.FindAll().AsQueryable()

let byId id =
    query {
        for x in queryable do
            where (x.FunId = id)
            head
    } 