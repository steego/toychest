(*


*)  

//  Loading this twice breaks it
//  Comment out after the first load
//#load "web-server.fsx"
//  I love LINQPad's dump
#load "dumper.fsx"

open System
open System.IO
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
dump(Folder(path), 7)

//dump("Hello", 4)