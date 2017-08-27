<Query Kind="FSharpProgram">
  <NuGetReference>fasterflect</NuGetReference>
</Query>

open Fasterflect

let safeGet(name:string)(o:obj) = 
    try PropertyExtensions.GetPropertyValue(o, name)
    with _ -> null

let makePrinter<'a>() : ('a -> string) = 
    let properties = [for p in typeof<'a>.Properties() do
                        yield (p.Name, safeGet) ]

    //  THIS IS WHAT'S RETURN
    fun (input:'a)  -> 
        let sb = new StringBuilder()
        sb.AppendLine("<table border='1' cellspacing='20'>")
        for (propName, safeGet) in properties do
            sb.AppendLine("<tr>")
            sb.AppendLine(sprintf "<th>%s</th>" propName)
            let propValue = input |> safeGet propName
            sb.AppendLine(sprintf "<td>%s</td>"  (propValue.ToString()))
            sb.AppendLine("</tr>")
        sb.AppendLine("</table>")
        sb.ToString()
        
// *******************  EXAMPLE *********************
        
type Person = { Name: string; Age: int }
type Employee = { Name: string; Salary: int }

let personPrinter = makePrinter<Person>()

let bob = { Name = "Bob"; Age = 31 } 

bob 
    |> personPrinter 
    |> Util.RawHtml 
    |> Dump