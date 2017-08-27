<Query Kind="FSharpProgram">
  <Reference Relative="..\..\..\..\..\github.com\steego\Steego.NET\src\Steego.Demo\bin\Release\Fasterflect.dll">C:\Projects\github.com\steego\Steego.NET\src\Steego.Demo\bin\Release\Fasterflect.dll</Reference>
  <Reference Relative="..\..\..\..\..\github.com\steego\Steego.NET\src\Steego.Demo\bin\Release\Steego.Demo.dll">C:\Projects\github.com\steego\Steego.NET\src\Steego.Demo\bin\Release\Steego.Demo.dll</Reference>
  <Reference Relative="..\..\..\..\..\github.com\steego\Steego.NET\src\Steego.Demo\bin\Release\Suave.dll">C:\Projects\github.com\steego\Steego.NET\src\Steego.Demo\bin\Release\Suave.dll</Reference>
</Query>

open Steego.Printer

type Folder(path:string) = 
    member this.Path = path
    member this.SubFolders = Directory.EnumerateDirectories(path) |> Seq.map Folder
    //  File Viewer???
    
let DumpHtml(html:Steego.Html.Tag) = 
    Util.RawHtml(html.ToString()) |> Dump

Folder(@"C:\Projects\") 
    |> printHtml 2 
    |> DumpHtml

