module Html

open System
open System.IO
//open System.Web

let inline htmlEncode(s) = s

type Tag = 
    | Tag of Name:string * Attributes:Map<string, string> * Body:Tag list
    | Text of string
    member this.Write(w:TextWriter) = 
        match this with
        | Text(s) -> 
            w.Write("<span>")
            w.WriteLine(htmlEncode(s))
            w.Write("</span>")        
        | Tag(name, attributes, body) -> 
            w.Write(sprintf "<%s" name)
            for (name,value) in attributes |> Map.toList do
                w.Write(sprintf " %s=\"%s\"" (htmlEncode name) (htmlEncode value))
            w.WriteLine(">")
            for child in body do
                child.Write(w)
            w.WriteLine(sprintf "</%s>" name)
    override this.ToString() = 
        use sw = new StringWriter()
        this.Write(sw)
        sw.ToString()
        
let makeTag (name:string) = 
    let tag(attributes:(string * string) list) (body:Tag list) = 
        // let newBody = [ for child in body do
        //                     match child with
        //                     | null -> ()
        //                     | :? Tag as t -> yield t
        //                     | :? list<Tag> as list -> yield! list
        //                     | :? string as s -> yield Text(s)
        //                     | o -> yield Text(o.ToString())
        //             ]
        Tag(name, (Map.ofList attributes), body)    
    tag