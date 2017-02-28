<Query Kind="FSharpProgram" />

open System.IO
open System.Collections.Concurrent

type Tree<'a>(value:'a, getChildren: 'a -> seq<'a>) = 
  let children = lazy [ for item in getChildren(value) do
                          yield Tree(item, getChildren) ]
  member this.Value = value
  member this.Children = children.Value
  member this.Where(test) = Tree(value, getChildren >> Seq.filter test)
  member this.Select(transform) = 
    Tree(transform value, getChildren >> Seq.map transform)
  member this.SelectMany(getAddlChildren: 'a -> seq<'a>) =
    Tree(value, fun x -> Seq.concat[getChildren x;  getAddlChildren value])

let memoize (f:'a -> 'b) = 
  let cache = ConcurrentDictionary<'a,'b>(HashIdentity.Structural)
  fun x -> cache.GetOrAdd(x, f)
  
let SHAFile = memoize <| fun (path:string) ->
  use sha1 = System.Security.Cryptography.SHA1.Create()
  use stream = File.OpenRead(path)
  let hash = sha1.ComputeHash(stream)
  printfn "Computed"
  BitConverter.ToString(hash).Replace("-", "")



let isDirectory(path:string) = 
  let attr = File.GetAttributes(path)
  attr.HasFlag(FileAttributes.Directory)
  
let getSubElements(path) = seq {
    if isDirectory(path) then 
      yield! Directory.EnumerateFiles(path)
      yield! Directory.EnumerateDirectories(path)
  }

let path = @"C:\Projects\github.com\steego\dotnetcasts"

Tree(path, getSubElements)
  //.Where(fun path -> isDirectory(path) || isSource(path))
  .Dump()
