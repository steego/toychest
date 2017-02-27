<Query Kind="FSharpProgram" />

//  If I had an arbitrary type like:
type Person = { Name:string; Salary: int }
//  or 
type Project = { Summary: string; DueDate: System.DateTime }

//  I'd like to be able to create a function at runtime
//  that would print these objects as simple HTML tables

let makePrinter<'a>() = 
  //  Up here we'll precompute and prereflect what we need to
  //  print our type... 'a
  let properties = [for p in typeof<'a>.GetProperties() do
                      let safeGet(o:obj) = if o = null then "<null>"
                                           else
                                             let value = p.GetValue(o)
                                             if value = null then "<null>"
                                             else value.ToString()
                      yield (p.Name, safeGet) ]
  printfn "Done precomputing"

  //  We'll return a function
  fun (input:'a) -> 
    let sb = new StringBuilder()
    sb.AppendLine("<table>")
    for (propName, safeGet) in properties do
      sb.AppendLine("<tr>")
      sb.AppendLine(sprintf "<th>%s</th>" propName)
      let propValue = safeGet input
      sb.AppendLine(sprintf "<td>%s</td>" propValue)
      sb.AppendLine("</tr>")
    sb.AppendLine("</table>")
    sb.ToString()
    
//  Let's test it
let printPerson = makePrinter<Person>()
Util.RawHtml(printPerson({ Name = "Bob"; Salary = 1000000 })).Dump()
Util.RawHtml(printPerson({ Name = "Larry"; Salary = 1000000 })).Dump()

//  We reflect once, and print many times.

//  FIN