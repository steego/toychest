//  Converts objects to HTML

module Printer

#load "type-info.fsx"
#load "rules.fsx"

open System
open System.Text
open System.Collections
open TypeInfo

let getTypeName(o:obj) = sprintf "Not matched: %s" (o.GetType().FullName)

let printObject(render:obj -> string) (getters:MemberGetter list) (o:obj) =
    let sb = new StringBuilder()
    let typeInfo = new TypeInfo(o)
    sb.AppendLine("<table class='table table-bordered table-striped'>") |> ignore
    for g in typeInfo.PrimitiveMembers do
        let value = g.Get(o) |> render
        sb.AppendFormat("<tr><th>{0}</th><td>{1}</td></tr>", g.Name, value) |> ignore
    for g in typeInfo.ObjectMembers do
        let link = sprintf "<a href='%s'>%s</a>" g.Name g.Name
        let value = g.Get(o) |> render
        sb.AppendFormat("<tr><th>{0}</th><td>{1}</td></tr>", link, value) |> ignore
    for g in typeInfo.EnumerableMembers do
        //let link = sprintf "<a href='%s'>%s</a>" g.Name g.Name
        let value = g.Get(o) |> render
        sb.AppendFormat("<tr><th colspan='2'>{0}</th></tr>", g.Name) |> ignore
        sb.AppendFormat("<tr><td colspan='2'>{0}</td></tr>", value) |> ignore


    sb.AppendLine("</table>") |> ignore
    sb.ToString()

let printList(render:obj -> string) (list:IEnumerable) = 
    let sb = new StringBuilder()
    sb.AppendLine("<table class='table table-bordered table-striped'>") |> ignore
    for item in list do
        let value = item |> render
        sb.AppendFormat("<tr><td>{0}</td></tr>", value) |> ignore
    done
    sb.AppendLine("</table>") |> ignore
    sb.ToString()

let printGenericList(render:obj -> string) (getters:MemberGetter list) (list:IEnumerable) =
    let sb = new StringBuilder()
    sb.AppendLine("<table class='table table-bordered table-striped'>") |> ignore
    sb.AppendLine("<thead>") |> ignore
    for g in getters do
        sb.AppendFormat("<th>{0}</th>", g.Name) |> ignore
    sb.AppendLine("</thead>") |> ignore
    sb.AppendLine("<tbody>") |> ignore
    for item in list do
        sb.AppendLine("<tr>") |> ignore
        for g in getters do
            let value = g.Get(item) |> render
            sb.AppendFormat("<td>{0}</td>", value) |> ignore
        //let value = item |> render
        sb.AppendLine("</tr>") |> ignore
    done
    sb.AppendLine("</tbody>") |> ignore
    sb.AppendLine("</table>") |> ignore
    sb.ToString()   
let printBasic render (o:obj) = 
    match o with
    | null -> Some("null")
    | GenericList(getters, list) -> Some(printGenericList render getters list)
    | Object(members, prims, subObjs, enums, o1) -> Some(printObject render members o)
    | :? int as v -> Some(v.ToString())
    | :? String as s -> Some(s)
    | :? DateTime as v -> Some(v.ToShortDateString())
    | IsPrimitive(n) -> Some(n.ToString())
    | IsNullable(n) -> Some(n.ToString())
    | :? System.Collections.IEnumerable as s -> Some(printList render s)
    | _ -> None

let print(o:obj, level:int) : string = 
    let fn = [printBasic] |> Rules.combineMax level getTypeName 
    fn(o)