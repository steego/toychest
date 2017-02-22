module Dumper

#load "web-server.fsx"
#load "printer.fsx"

open System
open Suave

WebServer.start()

[<Literal>]
let HeadTemplate = @"
  <head>
    <meta http-equiv='refresh' content='2' />
  </head>
"

let dump(value:'a) = WebServer.update(fun (ctx:HttpContext) -> async {
                            let path = ctx.request.path
                            let print = Printer.makePrinter<'a>()
                            let html = sprintf "<html>%s<body>%s</body></html>" HeadTemplate (print value)
                            return! Successful.OK html ctx
                        })

