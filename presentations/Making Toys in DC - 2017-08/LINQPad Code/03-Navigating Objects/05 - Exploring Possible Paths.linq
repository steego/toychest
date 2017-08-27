<Query Kind="FSharpProgram">
  <NuGetReference>fasterflect</NuGetReference>
</Query>

// ******  LET'S MAKE A PROPERTY EXPLORER  *************
        
type PropertyExplorer(t:Type) = 
    member this.Type = t.Name
    member this.Properties = 
        [| for p in Fasterflect.PropertyExtensions.Properties(t) do
              yield Property(p.Name, p.PropertyType) |]
and Property(name:string, t:Type) = 
    member this.Name = name
    member this.Type = Util.OnDemand(t.Name, (fun() -> PropertyExplorer(t)))



//  ************* EXAMPLE ******************

type Name = { First: string; Last: string }
type Person = { Name: Name; DOB: DateTime }

PropertyExplorer(typeof<Person>).Dump()