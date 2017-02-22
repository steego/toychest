module WebServer

#if INTERACTIVE
#r "../packages/Suave/lib/net40/Suave.dll"
#endif

//  Encapsulate everything in 
open Suave

let private mainWebPart = ref (Successful.OK "Welcome")

let update(newPart) = lock mainWebPart (fun () -> mainWebPart := newPart)

let start() = 
    async {
            startWebServer defaultConfig (fun(ctx : HttpContext) ->
                async {
                    let part = lock mainWebPart (fun () -> mainWebPart.Value)
                    return! part ctx
                })
    } |> Async.Start

