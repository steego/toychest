//  Combines the web server with the printer

module Dumper

//#load "web-server.fsx"
#load "printer.fsx"

open System // 
open Suave //


WebServer.start()

[<Literal>]
let HeadTemplate = @"
  <head>
    <!--<meta http-equiv='refresh' content='2' /> -->
    <link href=""https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css"" rel=""stylesheet"" integrity=""sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u"" crossorigin=""anonymous"">
    <script src=""https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"" integrity=""sha384-Tc5IQib027qvyjSMfHjOMaLkfuWVxZxUPnCJA7l2mCWNIpG9mGCD8wGNIcPD7Txa"" crossorigin=""anonymous""></script>
  </head>
"

open Printer

let dump (level:int) (value:'a) = 
  WebServer.update(fun (ctx:HttpContext) -> async {
        let path = ctx.request.path
        let o = value :> obj
        let tag = o |> print level
        let html = sprintf "<html>%s<body>%s</body></html>" HeadTemplate (tag.ToString())
        return! Successful.OK html ctx
    })

let dumpPath (level:int) (getValue:string list -> 'a) = 
  WebServer.update(fun (ctx:HttpContext) -> async {
        let path : string = ctx.request.path
        let items = path.Split('/') |> List.ofArray
        let o = getValue items :> obj
        let tag = o |> print level
        let html = sprintf "<html>%s<body>%s</body></html>" HeadTemplate (tag.ToString())
        return! Successful.OK html ctx
    })
