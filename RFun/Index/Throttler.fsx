﻿module Throttler

open System
open System.Collections.Generic

type Message = Work | Quit

let throttle asyncs limit f =
 
    let q = Queue()
    let dequeue() = try q.Dequeue() |> Some with _ -> None
    asyncs |> Seq.iter (fun x -> q.Enqueue x)
    
    let agent =
        MailboxProcessor.Start(fun x ->
            let rec loop count =
                async {
                    let! msg = x.Receive()
                    match msg with
                    | Work ->
                        let work = dequeue()
                        match work with
                        | Some work' ->
                            async {
                                try
                                    do! work'
                                finally
                                    x.Post Work
                                } |> Async.Start
                            return! loop count
                        | None ->
                            x.Post Quit
                            return! loop (count + 1)
                    | Quit ->
                        match count with
                        | y when y = limit ->
                            f |> Async.Start
                            (x:> IDisposable).Dispose()
                        | _ -> return! loop count
                }
            loop 0
        )
    [1 .. limit] |> List.iter (fun x -> agent.Post Work)