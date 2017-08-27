<Query Kind="FSharpProgram" />


type Tag =
    | Tag of Name:string * Attributes:Map<string, string> * Body:Tag list
    | Text of string

let myBlog =
    Tag("div", Map.empty, [
        Tag("h1", Map.empty, [Text("My Taylor Swift Fan Page")])
        Tag("p", Map.empty, [Text("She's simply amazing...")])
    ])
    
myBlog.Dump()