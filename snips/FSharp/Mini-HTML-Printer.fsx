//  Let's make our own HTML printer

open System

type Person = { Name: string; Salary: int }
let bob = { Name = "Bob"; Salary = 95000 }

let makePrinter<'a>() = 
    let properties = typeof<'a>.
    fun (o:'a) -> 
        String.Join("\n", [|
            yield "<table>"
            //for p 
            yield "</table>"

        |])

let testData = [
    
    { Name = "Larry"; Salary = 120000 }
    { Name = "Jim"; Salary = 320000 }
]

open System.Collections

let printHtml(o:obj) = 
    if o = null then "<null>"
    elif o :? string then string(o)
    elif o :? IEnumerable then "List"
    else "Unknown Type..."

printHtml(testData)

let testFile = @"C:\Temp\output.html"

IO.File.WriteAllText(testFile, "<h1>Hello World!</h1>")

Diagnostics.Process.Start(testFile);

//  In Ionide I can send F# statements to the interactive
//  Window with a simple Alt-Enter after I select the Code
//  Alternatively, you can use Alt+/ to send a single line