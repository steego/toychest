<Query Kind="FSharpProgram" />

type Folder(path:string) = 
    member this.Path = path
    member this.SubFolders = 
        lazy (Directory.EnumerateDirectories(path) 
                |> Seq.map Folder
                |> Seq.toArray)
    member this.Files = Directory.EnumerateFiles(path) |> Seq.map File

and File(file:string) = 
    member this.Filename = file
    member this.Open = Util.OnDemand("Open", (fun() -> File.ReadAllText(file)))
    

Folder(@"C:\Projects\").Dump()