(*

This is my cheap imitation of LINQPad's dump

1. There's an object printer
2. You can execute the whole file by hitting F5

*)

//  Loading this twice breaks it
//  Comment out after the first load
// #load "web-server.fsx"
//  I love LINQPad's dump
#load "dumper.fsx"

open System
open System.IO
open Dumper

type File(path:string) =
    let info = FileInfo(path)
    member this.Name = info.Name
    member this.Size = info.Length

let link href text = Html.makeTag "a" [("href", href)] [ Html.Text(text) ]

type Folder(path:string) =
  member this.Path = link path path
  member this.Files =
    Directory.EnumerateFiles(path)
    |> Seq.map File
    |> Seq.toList
   member this.SubFolders =
     Directory.EnumerateDirectories(path)
     |> Seq.map Folder
     |> Seq.toList

//let path = @"C:\Projects\github.com\steego\toychest"
let rootPath = "/Users/sgoguen/"

let walkObject (path:string list) (value:'a) : obj = 
  value :> obj

let getPath(path:string list) = 
  let selectedPath = String.Join("/", (path |> List.toArray))
  Folder(Path.Combine(rootPath, selectedPath))

type NavTree<'a>(value:'a, getChildren:'a -> seq<'a>) = 
  member this.Value = value
  member this.Children = seq { for c in getChildren value do
                                  yield NavTree(c, getChildren) }

//  My dump method.  Take an object and the max depth
//getPath |> dumpPath 2

let root = "/Users/sgoguen/projects/github.com/"
// IO.Directory.EnumerateDirectories(root)

// "Hello" |> dump 1

NavTree(root, fun d -> IO.Directory.EnumerateDirectories(d)) 
  |> dump 3

// TypeInfo.Tests.tests |> dump 3

open System.Threading

for i in 1..10000 do
  printfn "Heartbeat - %s" (DateTime.Now.ToString())
  Thread.Sleep(10000)

Console.WriteLine("Restarting")
