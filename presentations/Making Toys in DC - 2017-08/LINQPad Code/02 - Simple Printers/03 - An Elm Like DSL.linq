<Query Kind="FSharpProgram" />


type Tag =
    | Tag of Name:string * Attributes:Map<string, string> * Body:Tag list
    | Text of string

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