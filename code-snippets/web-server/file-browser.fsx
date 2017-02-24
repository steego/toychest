(*

This is my cheap imitation of LINQPad's dump

1. There's an object printer
2. You can execute the whole file by hitting F5

*)

//  Loading this twice breaks it
//  Comment out after the first load
//#load "web-server.fsx"
//  I love LINQPad's dump
#load "dumper.fsx"


open System.IO
open System
open Dumper

type File(path:string) =
    let info = new FileInfo(path)
    member this.Name = info.Name
    member this.Size = info.Length

type Folder(path:string) =
  member this.Path = path
  member this.Files =
    Directory.EnumerateFiles(path)
    |> Seq.map File
    |> Seq.toList
   member this.SubFolders =
     Directory.EnumerateDirectories(path)
     |> Seq.map Folder
     |> Seq.toArray

let path = @"C:\Projects\github.com\steego\toychest"

//  My dump method.  Take an object and the max depth
dump(Folder(path), 7)

//dump("Hello", 4)
