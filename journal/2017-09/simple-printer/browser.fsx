#if INTERACTIVE
#r "System.Net.dll"
#r "../../../packages/Suave/lib/net40/Suave.dll"
#endif

 

module Browser = begin

    let pageContent = """
<!DOCTYPE html>

<meta charset="utf-8" />

<title>WebSocket Test</title>

<script language="javascript" type="text/javascript">

  var wsUri = "ws://localhost:8080/websocket";
  var output;

  function init()
  {
    output = document.getElementById("output");
    testWebSocket();
  }

  function testWebSocket()
  {
    websocket = new WebSocket(wsUri);
    websocket.onopen = function(evt) { onOpen(evt) };
    websocket.onclose = function(evt) { onClose(evt) };
    websocket.onmessage = function(evt) { onMessage(evt) };
    websocket.onerror = function(evt) { onError(evt) };
  }

  function onOpen(evt)
  {
    writeToScreen("CONNECTED");
    doSend("Connected");
  }

  function onClose(evt)
  {
    writeToScreen("DISCONNECTED");
    setTimeout(function() {
      //  Try to connect again in 5 seconds
      init();
    }, 5000);
  }

  function onMessage(evt)
  {
    writeToScreen(evt.data);
  }

  function onError(evt)
  {
    writeToScreen('<span style="color: red;">ERROR:</span> ' + evt.data);
  }

  function doSend(message)
  {
    writeToScreen("SENT: " + message); 
    websocket.send(message);
  }

  function writeToScreen(message)
  {
    var element = document.getElementById("output");
    element.innerHTML = message;
  }

  window.addEventListener("load", init, false);

</script>

<div id="output"></div>    
"""

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

    let (|Message|_|) msg = 
        match msg with
        | (Text, data, true) -> Some(UTF8.toString data)
        | _ -> None

    let private socketHandler (ws : WebSocket) (context: HttpContext) =

      socket {
        let mutable loop = true
        let id : Id = Guid.NewGuid().ToString()

        while loop do
          let! msg = ws.read()
          match msg with
          | Message(str) -> Sockets.onConnect(id, ws, str)
          | (Close, _, _) -> Sockets.closeSocket(id)
                             loop <- false
          | _ -> ()
        done
      }


    let app : WebPart = 
      choose [
        path "/websocket" >=> handShake socketHandler
        GET >=> choose [ path "/" >=> Successful.OK(pageContent) ]
        //GET >=> choose [ path "/" >=> file "index.html"; browseHome ]
        NOT_FOUND "Found no handlers." ]


    let start() = 
        Async.Start(async {
            startWebServer { defaultConfig with logger = Targets.create Verbose [||] } app
        })

    let sendHtml(html:string) = Sockets.sendAll(html)

end