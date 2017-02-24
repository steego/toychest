module Dumper

//#load "web-server.fsx"
#load "printer.fsx"

open System
open Suave

WebServer.start()

[<Literal>]
let HeadTemplate = @"
  <head>
    <meta http-equiv='refresh' content='2' />
    <link href=""https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css"" rel=""stylesheet"" integrity=""sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u"" crossorigin=""anonymous"">
    <script src=""https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"" integrity=""sha384-Tc5IQib027qvyjSMfHjOMaLkfuWVxZxUPnCJA7l2mCWNIpG9mGCD8wGNIcPD7Txa"" crossorigin=""anonymous""></script>
  </head>
"

let dump(value:'a, level) = 
  WebServer.update(fun (ctx:HttpContext) -> async {
        let path = ctx.request.path
        let print = Printer.print
        let o = value :> obj
        let html = sprintf "<html>%s<body>%s</body></html>" HeadTemplate (print(o, level))
        return! Successful.OK html ctx
    })

