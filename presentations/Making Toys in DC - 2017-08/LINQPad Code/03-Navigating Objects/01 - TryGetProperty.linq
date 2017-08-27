<Query Kind="FSharpProgram">
  <NuGetReference>fasterflect</NuGetReference>
</Query>

//  *******************  TRY TO GET A PROPERTY ***********************


let TryGetProperty (name:string) (o: obj) : obj option = 
    try Some(Fasterflect.PropertyExtensions.GetPropertyValue(o, name))
    with ex -> None

//  *******************  EXAMPLES  ***********************
    
type Person = { Name: string; DOB: DateTime }
let taylor = { Name = "Taylor"; DOB = new DateTime(1989, 12, 13) }

Dump(taylor |> TryGetProperty "Name")
Dump(taylor |> TryGetProperty "DOB")
Dump(taylor |> TryGetProperty "UnknownProperty")

