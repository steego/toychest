<Query Kind="FSharpProgram">
  <Connection>
    <ID>51c42400-28cd-449f-b326-720e08cda739</ID>
    <Persist>true</Persist>
    <Server>.\SQL2014</Server>
    <IncludeSystemObjects>true</IncludeSystemObjects>
    <Database>AdventureWorks2014</Database>
    <ShowServer>true</ShowServer>
  </Connection>
  <Output>DataGrids</Output>
</Query>

let dc = new TypedDataContext()

type SalesPersonView(p:SalesPerson) = 
    let person = p.Employee.Person
    member this.Name = sprintf "%s %s" person.FirstName person.LastName
    member this.JobTitle = p.Employee.JobTitle
    member this.Orders = p.SalesOrderHeaders
    member this.Territory = Util.OnDemand(p.SalesTerritory.Name, fun () -> p.SalesTerritory)

type SalesPerson with
    member this.ToDump() = SalesPersonView(this)

type App() = 
    member this.DB = dc
    member this.SalesPeople = dc.SalesPersons |> Seq.map SalesPersonView

    
    
App().Dump()