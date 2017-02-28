<Query Kind="FSharpProgram">
  <Reference>&lt;ProgramFilesX86&gt;\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.3.0.0\FSharp.Core.dll</Reference>
  <NuGetReference>FSharp.Data</NuGetReference>
  <Namespace>FSharp.Data</Namespace>
</Query>

open System.IO

open FSharp.Data

[<Literal>]
let file = @"C:\Temp\SavedPages\www.blueapron.com\recipes\800"

type Recipe = HtmlProvider<file>

Recipe.GetSample().Lists.List14.V

Recipe.GetSample().Html.CssSelect("ul.ingredients-list").Dump()

let recipeUrl = @"https://www.blueapron.com/recipes/800"

let urlToFilename(url:string) = 
  let uri = Uri(url)
  sprintf "C:\Temp\SavedPages\%s%s" uri.Host (uri.PathAndQuery.Replace("/", "\\"))
  
let fetchPage(url) =
  let filename = urlToFilename url
  if File.Exists(filename) then File.ReadAllText(filename)
  else 
    let client = new System.Net.WebClient()
    let content = client.DownloadString(url)
    Directory.CreateDirectory(Path.GetDirectoryName(filename))
    File.WriteAllText(filename, content)
    content

recipeUrl |> fetchPage |> Dump