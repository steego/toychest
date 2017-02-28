<Query Kind="FSharpProgram">
  <NuGetReference>FSharp.Data.SqlClient</NuGetReference>
</Query>

//  Add the NuGet package - FSharp.Data.SqlClient

open FSharp.Data

module DAL = 
  [<Literal>]
  let conn = @"Data Source=.\SQL2014;Integrated Security=SSPI;Initial Catalog=AdventureWorks2014"
  
  type AdventureWorks = SqlProgrammabilityProvider<conn>
  
  let getCustomers() =
    use cmd = new SqlCommandProvider<"select * from Sales.Customer", conn>(conn)
    [for c in cmd.Execute() -> c.CustomerID]
    
DAL.getCustomers().Dump()