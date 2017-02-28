<Query Kind="FSharpProgram" />

open System.IO
open System.Collections.Concurrent

type FNode =
  | File of Path:string
  | Folder of Path:string * Children:Lazy<list<FNode>>

let isDirectory(path:string) = 
  let attr = File.GetAttributes(path)
  attr.HasFlag(FileAttributes.Directory)
      
let rec fromPath(path) = 
  if isDirectory path then Folder(path, lazy [for e in getSubElements(path) -> fromPath(e)])
                      else File(path)
and getSubElements(path) = seq {
    if isDirectory(path) then 
      yield! Directory.EnumerateFiles(path)
      yield! Directory.EnumerateDirectories(path)
  }

let memoize (f:'a -> 'b) = 
  let cache = ConcurrentDictionary<'a,'b>(HashIdentity.Structural)
  fun x -> cache.GetOrAdd(x, f)

let SHA1Stream(stream:Stream) = 
  use sha1 = System.Security.Cryptography.SHA1.Create()
  let hash = sha1.ComputeHash(stream)
  printfn "Computed"
  BitConverter.ToString(hash).Replace("-", "")
  
let SHAFile = memoize <| fun (path:string) ->
  use stream = File.OpenRead(path)
  SHA1Stream(stream)
  
let rec getSize = 
  memoize <|      fun path -> if isDirectory path = false then 
                                FileInfo(path).Length
                              else
                                List.sum [for f in getSubElements(path) -> getSize(f)]
                                    
  
type FNode with
  member this.Size = match this with
                     | File(path) -> lazy getSize(path)
                     | Folder(path, _) -> lazy getSize(path)
//  member this.Hash = match this with
//                     | File(path) -> lazy SHAFile(path)
//                     | Folder(_, _) -> lazy ""

let path = @"C:\Projects\github.com\"

fromPath(path).Dump()



