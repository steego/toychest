#load "html.fsx"
#load "browser.fsx"

open System
open Html
open Html.Tags
open Browser

//Browser.start()

let dump(o:obj) = 
    match o with
    | :? bool as b -> span [] [Text(b.ToString())]
    | _ -> Text("Unknown")

let page = 
    div [] [
        h1 [] [Text("My Taylor Swift Page")]
        p [] [Text("She's amazing.....")]
        p [] [Text("The current time: " + (DateTime.Now.ToString()))]
        dump true
    ]


Browser.sendHtml(page.ToString())
