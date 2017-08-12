//  A simple mutable web server

module WebServer

#r "System.Net.dll"
#r "../../packages/Suave/lib/net40/Suave.dll"

//  Encapsulate everything in 
open Suave
open System.Net

let mainWebPart = Suave.Successful.OK "Welcome"

let start() = 
        async {
                startWebServer defaultConfig (fun(ctx : HttpContext) ->
                    async {
                        return! mainWebPart ctx
                    })
        } |> Async.Start

start()
