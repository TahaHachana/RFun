module Indexden

#r "..\packages\TankTop.1.1.26\lib\TankTop.dll"
#r @"..\packages\ServiceStack.Text.3.9.71\lib\net35\ServiceStack.Text.dll"
#load "Credentials.fsx"

open System.Collections.Generic
open TankTop
open TankTop.Dto

let client = TankTopClient Credentials.indexdenUrl

let addDoc id name package description =
//    async {
        try
            let doc = Document id
            let dict = Dictionary()
            dict.Add("name", name)
            dict.Add("package", package)
            dict.Add("description", description)
            doc.Fields <- dict
            client.AddDocument("RFun", doc)
        with _ -> () //}