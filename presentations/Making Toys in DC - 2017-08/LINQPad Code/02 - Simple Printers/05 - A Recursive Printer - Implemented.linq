<Query Kind="FSharpProgram">
  <Reference Relative="..\..\..\..\..\github.com\steego\Steego.NET\src\Steego.Demo\bin\Release\Fasterflect.dll">C:\Projects\github.com\steego\Steego.NET\src\Steego.Demo\bin\Release\Fasterflect.dll</Reference>
  <Reference Relative="..\..\..\..\..\github.com\steego\Steego.NET\src\Steego.Demo\bin\Release\Steego.Demo.dll">C:\Projects\github.com\steego\Steego.NET\src\Steego.Demo\bin\Release\Steego.Demo.dll</Reference>
  <Reference Relative="..\..\..\..\..\github.com\steego\Steego.NET\src\Steego.Demo\bin\Release\Suave.dll">C:\Projects\github.com\steego\Steego.NET\src\Steego.Demo\bin\Release\Suave.dll</Reference>
</Query>


open Steego
open Steego.TypeInfo
open Steego.Printer
open Steego.Printer.Patterns
open Steego.Type.Patterns

//  
    
let rec print (level:int) (o:obj) : Html.Tag =         
    
    
    match o with
    //  NULL CHECK
    | null -> Html.Text("<null>")
    
    //  PASS CUSTOM HTML AS-IS
    | :? Html.Tag as t -> t
    
    //  KNOWN PRIMITIVES
    | :? int as v -> Html.Text(v.ToString())
    | :? String as s -> Html.Text(s)
    | :? DateTime as v -> Html.Text(v.ToShortDateString())
    
    //  UNKNOWN PRIMITIVES
    | IsPrimitive(n) -> Html.Text(n.ToString())
    
    //  NULLABLES
    | IsNullable(n) -> Html.Text(n.ToString())
    
    //  PRINT GENERIC ARRAYS, LISTS, IENUMERABLES
    | GenericList(_, _) when level < 1 -> Html.Text("List...")
    | GenericList(getters, list) -> printGenericList (level - 1)  getters list
    
    //  NON-GENERIC IENUMERABLES
    | :? System.Collections.IEnumerable when level < 1 -> Html.Text("List...")
    | :? System.Collections.IEnumerable as s -> printList (level - 1) s
    
    //  REGULAR KEY-VALUE OBJECTS
    | Object(_, _, _, _, _) when level < 1 -> Html.Text("Object...")
    | Object(members, _, _, _, _) -> printObject (level - 1) members o
    
    //  UKNOWN TYPES
    | _ -> Html.Text("...")


and printObject (level:int) (getters:MemberGetter list) =
    fun (o:obj) ->
        let typeInfo = new TypeInfo(o)
        let primativeMembers = typeInfo.PrimitiveMembers
        //  TABLE
        table 
            //  Our CSS Attributes
            [("class", "table table-bordered table-striped")]
            [   
                //  SHOW OBJECT TYPE
                yield th [("colspan", "2")] [ Html.Text(typeInfo.Name) ] 
                //  Show primitive members - strings, integers, bools, etc.
                for g in primativeMembers do
                    let value = g.Get(o) |> print level
                    yield keyValueRow(g.Name, value)
                //  Show object members - 
                for g in typeInfo.ObjectMembers do
                    let value = g.Get(o) |> print level
                    yield keyValueRow(g.Name, value)
                //  Show enumerable members
                for g in typeInfo.EnumerableMembers do
                    let value = g.Get(o) |> print level
                    yield tr [] [ 
                        th [("colspan", "1")] [ Html.Text(g.Name) ] 
                        td [("colspan", "1")] [ value ]
                    ]
            ]

//  Prints a simple list
and printList (level:int) (list:IEnumerable) = 
    let list = seq { for o in list -> o }
    table [("class", "table table-bordered table-striped")]
          [  for item in list.Take(maxTake) do
                let value = item |> print level
                yield tr [] [ td [] [ value ] ] 
          ]



//  Prints an IEnumerable<T>
and printGenericList (level:int) (getters:MemberGetter list) (list:IEnumerable) =
    let list = seq { for o in list -> o }
    table 
        [("class", "table table-bordered table-striped")]
        [ thead [] [ for g in getters -> th [] [ Html.Text(g.Name) ] ]
          tbody [] [ for item in list.Take(maxTake) do
                        yield tr [] [ for g in getters do
                                        let value = g.Get(item) |> print level 
                                        yield td [] [ value ]
                                    ] 
                   ]
        ]

