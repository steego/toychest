<Query Kind="FSharpProgram">
  <NuGetReference>FSharp.Control.Reactive</NuGetReference>
  <NuGetReference>System.Reactive</NuGetReference>
</Query>

open System.IO
open System.Reactive.Linq

// 1.  We need a function that enumerates files
let enumerateFiles(path:string) = 
    Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)

// 2. Let's convert our IEnumerable to IObservable so we can observe it
let sampleSequence (timeSpan:TimeSpan) (source:seq<_>) = 
    source.ToObservable().Sample(timeSpan)

open FSharp.Control.Reactive

enumerateFiles(@"C:\Projects\")
    |> Seq.scan (fun x y -> x + 1) 0
    |> sampleSequence(TimeSpan.FromSeconds(1.0))    
    |> Dump