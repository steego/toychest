<Query Kind="FSharpProgram">
  <Reference>&lt;RuntimeDirectory&gt;\System.Web.dll</Reference>
</Query>

open System

//  If I had an arbitrary type like:
type Project = { Summary: string; DueDate: DateTime }

type Person = { Name:string; Salary: int; Projects: Project list }


//  I'd like to be able to create a function at runtime
//  that would print these objects as simple HTML tables
let primitiveTypes = [typeof<int>; typeof<string>; typeof<DateTime>]

let isPrimitive(o:obj) = primitiveTypes.Contains(o.GetType())
let isList(o:obj) = o :? IEnumerable
let isObject(o:obj) = true


let getProperties(t:Type) = 
  [for p in t.GetProperties() do
    if p.CanRead && p.GetIndexParameters().Length = 0 then
      let safeGet(o:obj) = if o = null then null else p.GetValue(o)
      yield (p.Name, safeGet) ]


let makePrinter<'a>() = 
  //  Up here we'll precompute and prereflect what we need to
  //  print our type... 'a

  let rec printAny(level:int)(o:obj) = 
    if o = null then "<null>"
    elif isPrimitive(o) then printValue(o)
    elif isList(o) && level > 0 then printList level o
    elif isObject(o) && level > 0 then printObject level o
    else "..."
  and printValue(o:obj) = System.Web.HttpUtility.HtmlEncode(o.ToString())
  and printObject(level:int) (input:obj) = 
    let properties = getProperties(input.GetType())
    let sb = new StringBuilder()
    sb.AppendLine("<table>")
    for (propName, safeGet) in properties do
      sb.AppendLine("<tr>")
      sb.AppendLine(sprintf "<th>%s</th>" propName)
      let propValue = input |> safeGet |> printAny (level - 1)
      sb.AppendLine(sprintf "<td>%s</td>" propValue)
      sb.AppendLine("</tr>")
    sb.AppendLine("</table>")
    sb.ToString()
  and printList(level)(o) = 
    let list = o :?> IEnumerable
    let sb = new StringBuilder()
    sb.AppendLine("<table>")
    for item in list do
      sb.AppendLine(sprintf "<tr><td>%s</td></tr>" (item |> printAny (level - 1)))
    sb.AppendLine("</table>")
    sb.ToString()
    
  fun (input:'a) -> input |> printAny 5

    
//  Let's test it
let printPersonList = makePrinter<Person list>()
let people = [
  { 
    Name = "Bob"; Salary = 1000000; 
    Projects = 
      [
        { Summary = "Make Printer"; DueDate = DateTime.Now.AddDays(-1.0) }
        { Summary = "List Projects"; DueDate = DateTime.Now.AddDays(-2.0) }
      ]
  }
  { Name = "Larry"; Salary = 1000000; Projects = [] }
]

Util.RawHtml(printPersonList(people)).Dump()


//  We reflect once, and print many times.

//  FIN