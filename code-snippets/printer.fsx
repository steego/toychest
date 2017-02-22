module Printer

open System
open System.Collections
open System.Linq
open System.Text

let primitiveTypes = [typeof<int>; typeof<string>; typeof<DateTime>]

let isPrimitive(o:obj) = primitiveTypes.Contains(o.GetType())
let isList(o:obj) = o :? IEnumerable
let isObject(o:obj) = true

let isSeq(t:System.Type) = t.GetInterface(typeof<System.Collections.IEnumerable>.Name) |> isNull |> not
let isGenericSeq(t:Type) = isSeq(t) && t.GenericTypeArguments.Length >= 1



let getProperties(t:Type) = 
  [for p in t.GetProperties() do
    if p.CanRead && p.GetIndexParameters().Length = 0 then
      let safeGet(o:obj) = if isNull o then null else p.GetValue(o)
      yield (p.Name, safeGet) ]



let makePrinter<'a>() = 
  //  Up here we'll precompute and prereflect what we need to
  //  print our type... 'a

  let rec printAny(level:int)(o:obj) = 
    if isNull o then "<null>"
    elif isPrimitive(o) then printValue(o)
    elif isList(o) && level > 0 then printList level o
    elif isObject(o) && level > 0 then printObject level o
    else "..."
  and printValue(o:obj) = System.Web.HttpUtility.HtmlEncode(o.ToString())
  and printObject(level:int) (input:obj) = 
    let properties = getProperties(input.GetType())
    let sb = new StringBuilder()
    sb.AppendLine("<table border='1' cellpadding='3' cellspacing='0'>") |> ignore
    for (propName, safeGet) in properties do
      sb.AppendLine("<tr>") |> ignore
      sb.AppendLine(sprintf "<th>%s</th>" propName) |> ignore
      let propValue = input |> safeGet |> printAny (level - 1)
      sb.AppendLine(sprintf "<td>%s</td>" propValue) |> ignore
      sb.AppendLine("</tr>") |> ignore
    sb.AppendLine("</table>") |> ignore
    sb.ToString()
  and printList(level)(o) = 
    let list = o :?> IEnumerable
    let sb = new StringBuilder()
    sb.AppendLine("<table>") |> ignore
    for item in list do
      sb.AppendLine(sprintf "<tr><td>%s</td></tr>" (item |> printAny (level - 1))) |> ignore
    sb.AppendLine("</table>") |> ignore
    sb.ToString()
    
  fun (input:'a) -> input |> printAny 5
