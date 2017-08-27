<Query Kind="FSharpProgram" />

type Tag = 
    | Tag of Name:string * Attributes:Map<string, string> * Body:Tag list
    | Text of string

//  We need helpers to encode html, attributes and urls
module Encoders =
    open System.Web
    let inline html(s:string) = HttpUtility.HtmlEncode(s)
    let inline attribute(s:string) = HttpUtility.HtmlAttributeEncode(s)
    let inline url(s:string) = HttpUtility.UrlEncode(s)

//  Lets add some rendering code
type Tag with
    
    //  Our Create Method will Send to a TextWriter
    member this.Write(w:TextWriter) = 
        match this with
        | Text(s) -> 
            w.Write("<span>")
            w.WriteLine(Encoders.html(s))
            w.Write("</span>")        
        | Tag(name, attributes, body) -> 
            w.Write(sprintf "<%s" name)
            for (name,value) in attributes |> Map.toList do
                w.Write(sprintf " %s=\"%s\"" (Encoders.attribute name) (Encoders.attribute value))
            w.WriteLine(">")
            for child in body do
                child.Write(w)
            w.WriteLine(sprintf "</%s>" name)
            
    //  We can override the ToString
    override this.ToString() = 
        use sw = new StringWriter()
        this.Write(sw)
        sw.ToString()
        
    //  Let's tell LINQPad how we want it to render our object
    //  Custom object rendering can be incredibly helpful
    member this.ToDump() = Util.RawHtml(this.ToString())

//  This let's use create shortcuts
let makeTag (name:string) = 
    let tag(attributes:(string * string) list) (body:Tag list) = 
        Tag(name, (Map.ofList attributes), body)    
    tag
    
let div = makeTag "div"
let h1 = makeTag "h1"
let p = makeTag "p"

let myBlog =
    div [] [
        h1 [] [Text("My Taylor Swift Fan Page")]
        p [] [Text("She's simply amazing...")]
    ]
    
myBlog.Dump()