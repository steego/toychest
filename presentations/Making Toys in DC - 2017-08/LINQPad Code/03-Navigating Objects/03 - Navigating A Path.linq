<Query Kind="FSharpProgram">
  <NuGetReference>fasterflect</NuGetReference>
</Query>

//  *******************  TRY TO GET A PROPERTY ***********************


let TryGetProperty (name:string) (o: obj) : obj option = 
    try Some(Fasterflect.PropertyExtensions.GetPropertyValue(o, name))
    with ex -> None

//  ***************** NAVIGATE A PATH **********************

let TryNavPath (path:string list) (o:obj) : obj option = begin
    let navSegment (o:obj option) (segment:string) = 
        match o with
        | Some(o:obj) when o <> null -> TryGetProperty segment o
        | _ -> None

    path |> List.fold navSegment (Some(o))
end


//  ****************  NAVIGATE A PATH *****************

let NavigatePath(path:string)(o:obj) = 
    let path = path.Split("/") |> List.ofArray
    o |> TryNavPath path

//  *******************  EXAMPLES  ***********************

type Name = { First: string; Last: string }
type Person = { Name: Name; DOB: DateTime }
let taylor = { 
    Name = { First = "Taylor"; Last = "Swift" }
    DOB = new DateTime(1989, 12, 13) 
}

taylor |> NavigatePath "Name/First" |> Dump
taylor |> NavigatePath "Name/Middle" |> Dump
taylor |> NavigatePath "Name/Last" |> Dump
taylor |> NavigatePath "DOB" |> Dump