#if INTERACTIVE
#r "System.Net.dll"
#r "../../../packages/Suave/lib/net40/Suave.dll"
#endif


open Suave
open Suave.Http
open Suave.Operators
open Suave.Filters
open Suave.Successful
open Suave.Files
open Suave.RequestErrors
open Suave.Logging
open Suave.Utils

open System
open System.Net

open Suave.Sockets
open Suave.Sockets.Control
open Suave.WebSocket

let keepHandling (ws: WebSocket) (context: HttpContext) =
    
    let inbox = MailboxProcessor.Start (fun inbox -> async {
        let mutable close = false
        while not close do
            printfn "Waiting to receive"
            let! op, data, fi = inbox.Receive()
            printfn "Received!"
            let! _ = ws.send op data fi
            printfn "Send!"
            if op = Close then
                printfn "Closed!"
            close <- op = Close                    
    })

    let mutable loop = true

    socket {
        while loop do
            let! m = ws.read()
            printfn "Socket was read"
            match m with
            | Text,data,true -> 
                let response = sprintf "response to %O" (DateTime.Now)

                let byteResponse =
                  response
                  |> System.Text.Encoding.ASCII.GetBytes
                  |> ByteSegment

                inbox.Post(Opcode.Text, byteResponse, true)
                //printfn "Sent Respon"
                () //process incoming message
            | Ping,_,_ -> 
                printfn "Ping"
                inbox.Post (Pong, ArraySegment([||]), true)
                printfn "Sent Pong"
            | Close,_,_ -> 
                //inbox.Post (Close, ArraySegment([||]), true)
                //printfn "Sent Close"
                //loop <- false
                ()
            | _ -> ()
        printfn "Socket done"
    }

let ws (webSocket : WebSocket) (context: HttpContext) =

  let sendText(response:string) = 
        let byteResponse =
          response
          |> System.Text.Encoding.ASCII.GetBytes
          |> ByteSegment
        
        // the `send` function sends a message back to the client
        webSocket.send Text byteResponse true

  let job = 
    async {
      while true do
        do! Async.Sleep 200
        let! resp = sendText (sprintf "Data: %O" (DateTime.Now))
        ()
    }

  Async.Start(job)

  socket {
    printfn "Socket connected"
    let mutable loop = true

    while loop do
      // the server will wait for a message to be received without blocking the thread
      printfn "Waiting for socket"
      let! msg = webSocket.read()
      printfn "Request received"

      match msg with
      | (Text, data, true) ->
        let str = UTF8.toString data
        let response = sprintf "response to %s" str

        do! sendText response
        printfn "Response sent"

      | (Close, _, _) ->
        let emptyResponse = [||] |> ByteSegment
        do! webSocket.send Close emptyResponse true
        printfn "Socket closed"

        // after sending a Close message, stop the loop
        loop <- false

      | _ -> ()
    }

/// An example of explictly fetching websocket errors and handling them in your codebase.
let wsWithErrorHandling (webSocket : WebSocket) (context: HttpContext) = 
   
   let exampleDisposableResource = { new IDisposable with member __.Dispose() = printfn "Resource needed by websocket connection disposed" }
   let websocketWorkflow = ws webSocket context
   
   async {
    let! successOrError = websocketWorkflow
    match successOrError with
    // Success case
    | Choice1Of2() -> ()
    // Error case
    | Choice2Of2(error) ->
        // Example error handling logic here
        printfn "Error: [%A]" error
        exampleDisposableResource.Dispose()
        
    return successOrError
   }

let app : WebPart = 
  choose [
    // path "/websocket" >=> handShake keepHandling
    path "/websocket" >=> handShake ws
    
    path "/websocketWithError" >=> handShake wsWithErrorHandling
    GET >=> choose [ path "/" >=> file "index.html"; browseHome ]
    NOT_FOUND "Found no handlers." ]

// [<EntryPoint>]
// let main _ =
//   startWebServer { defaultConfig with logger = Targets.create Verbose [||] } app
//   0

startWebServer { defaultConfig with logger = Targets.create Verbose [||] } app

//
// The FIN byte:
//
// A single message can be sent separated by fragments. The FIN byte indicates the final fragment. Fragments
//
// As an example, this is valid code, and will send only one message to the client:
//
// do! webSocket.send Text firstPart false
// do! webSocket.send Continuation secondPart false
// do! webSocket.send Continuation thirdPart true
//
// More information on the WebSocket protocol can be found at: https://tools.ietf.org/html/rfc6455#page-34
//