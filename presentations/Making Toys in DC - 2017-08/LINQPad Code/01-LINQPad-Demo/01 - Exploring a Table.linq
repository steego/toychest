<Query Kind="FSharpProgram">
  <Connection>
    <ID>51c42400-28cd-449f-b326-720e08cda739</ID>
    <Persist>true</Persist>
    <Server>.\SQL2014</Server>
    <IncludeSystemObjects>true</IncludeSystemObjects>
    <Database>AdventureWorks2014</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

let dc = new TypedDataContext()

//  Show the Products Table
//dc.Products |> Dump

//  Projected Products
//type ProductSummary = { ProductID: int; Name: string; }
//
//let summaries = 
//    query { 
//        for p in dc.Products do 
//        select { ProductID = p.ProductID; Name = p.Name } 
//    }
//    
//summaries |> Dump