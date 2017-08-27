<Query Kind="FSharpProgram">
  <NuGetReference>System.Reactive</NuGetReference>
</Query>

open System.IO
open System.Reactive.Linq
open System.Reactive.Concurrency

let rootPath = @"C:\Projects"

type Progress = { FileCount: int; TotalBytes: int64 }

let addToCount(p:Progress)(f:FileInfo) : Progress = 
    {
        FileCount = p.FileCount + 1
        TotalBytes = p.TotalBytes + f.Length
    }

let enumerateFile(path:string) = 
    Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
    |> Seq.map System.IO.FileInfo
    |> Seq.scan addToCount ({ FileCount = 0; TotalBytes = 0L; })

let lazyObserver(o:Lazy<IObservable<string>>) = 
    let container = LINQPad.DumpContainer("Starting Observer")
    Async.Start(async {
        o.Value
            .Subscribe(fun x -> 
                container.UpdateContent(x)

            )
    })
    container


type Folder(path:string) = 
    member this.Path = path
    member this.SubFolders = 
        lazy [ for f in Directory.EnumerateDirectories(path) -> Folder(f) ]
    member this.Files = 
        lazy (Directory.EnumerateFiles(path) |> Seq.map FileInfo |> Seq.toArray)
    member this.FileCounts = 
        lazy lazyObserver(lazy enumerateFile(path).ToObservable().Select(fun f -> f.FileCount.ToString()))

Folder(@"C:\Projects\").Dump()