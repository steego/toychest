#if INTERACTIVE
#r "System.Net.dll"
#r "../../../packages/Suave/lib/net40/Suave.dll"
#endif

 

module Browser = begin

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

    type Id = string

    module Sockets = begin

        let sendText (webSocket : WebSocket) (response:string) = begin
            let byteResponse =
              response
              |> System.Text.Encoding.ASCII.GetBytes
              |> ByteSegment

            // the `send` function sends a message back to the client
            webSocket.send Text byteResponse true
        end   

        type Commands = 
            | ReceiveString of Id * WebSocket * string
            | SendAll of string
            | CloseSocket of Id

        let rec private startMailbox(inbox:MailboxProcessor<Commands>) = begin
            
            let rec doLoop(sockets:Map<Id,WebSocket>) = async {
                let! input = inbox.Receive()
                match input with
                | ReceiveString(id, sock, input) -> 
                    return! sockets |> Map.add id sock |> doLoop
                | SendAll(text) ->
                    for (id, sock) in sockets |> Map.toSeq do
                        let! res = text |> sendText sock
                        ()
                    return! doLoop(sockets)
                | CloseSocket(id) ->
                    let ws = sockets.Item id
                    let emptyResponse = [||] |> ByteSegment
                    ws.send Close emptyResponse true |> ignore
                    return! sockets |> Map.remove id |> doLoop
            }

            doLoop(Map.empty)
        end

        let private inbox = MailboxProcessor.Start(startMailbox)

        let onConnect(id, socket, data) = inbox.Post(ReceiveString(id, socket, data))
        let sendAll(text) = inbox.Post(SendAll(text))
        let closeSocket(id) = inbox.Post(CloseSocket(id))

    end

    let private socketHandler (ws : WebSocket) (context: HttpContext) =

      socket {
        let mutable loop = true
        let id : Id = Guid.NewGuid().ToString()

        while loop do
          let! msg = ws.read()

          match msg with
          | (Text, data, true) ->
            
            let str = UTF8.toString data
            Sockets.onConnect(id, ws, str)
            do! str |> Sockets.sendText ws

          | (Close, _, _) ->
            
            Sockets.closeSocket(id)
            //do! ws.send Close emptyResponse true
            loop <- false

          | _ -> ()
        }


    let app : WebPart = 
      choose [
        path "/websocket" >=> handShake socketHandler
        GET >=> choose [ path "/" >=> file "index.html"; browseHome ]
        NOT_FOUND "Found no handlers." ]


    let start() = startWebServer { defaultConfig with logger = Targets.create Verbose [||] } app

    let sendHtml(html:string) = Sockets.sendAll(html)

end

open System

Async.Start(async {
    while true do 
        let time = sprintf "<b>Current Time</b>: <i>%s</i>" (DateTime.Now.ToString())
        Browser.sendHtml(time)
        do! Async.Sleep(1000)
})

do Browser.start()