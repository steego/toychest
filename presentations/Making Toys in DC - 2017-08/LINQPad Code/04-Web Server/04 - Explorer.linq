<Query Kind="FSharpProgram">
  <Connection>
    <ID>51c42400-28cd-449f-b326-720e08cda739</ID>
    <Persist>true</Persist>
    <Server>.\SQL2014</Server>
    <IncludeSystemObjects>true</IncludeSystemObjects>
    <Database>AdventureWorks2014</Database>
    <ShowServer>true</ShowServer>
  </Connection>
  <Reference Relative="..\..\..\..\..\github.com\steego\Steego.NET\src\Steego.Demo\bin\Release\Fasterflect.dll">C:\Projects\github.com\steego\Steego.NET\src\Steego.Demo\bin\Release\Fasterflect.dll</Reference>
  <Reference Relative="..\..\..\..\..\github.com\steego\Steego.NET\src\Steego.Demo\bin\Release\Steego.Demo.dll">C:\Projects\github.com\steego\Steego.NET\src\Steego.Demo\bin\Release\Steego.Demo.dll</Reference>
  <Reference Relative="..\..\..\..\..\github.com\steego\Steego.NET\src\Steego.Demo\bin\Release\Suave.dll">C:\Projects\github.com\steego\Steego.NET\src\Steego.Demo\bin\Release\Suave.dll</Reference>
</Query>

let dc = new TypedDataContext()

Steego.Web.Explorer.explore 1 dc