module HuffmanTree

open System.Linq

type Node<'a> = Value of Value:'a | Node of Left:Node<'a> * Right:Node<'a>
type HuffmanTree<'a> = HuffmanNode of Frequency:int * Node:Node<'a>

let getCount = function HuffmanNode(count, node) -> count
let getNode = function HuffmanNode(count, node) -> node
  
let countChars (input:string) = 
  input 
  |> Seq.groupBy(fun c -> c) 
  |> Seq.map(fun (char, seq) -> HuffmanNode(seq |> Seq.length, Value(char)))
  |> Seq.sortBy(getCount)
  |> Seq.toList

let rec createTree = function
  | HuffmanNode(x, a)::HuffmanNode(y, b)::rest -> 
      let node = HuffmanNode(x + y, Node(a, b))
      node::rest |> List.sortBy(getCount) |> createTree
  | [node] -> node
  | [] -> HuffmanNode(0, Unchecked.defaultof<_>)

let toLookup (node:HuffmanTree<'a>) = 
  let rec toLookupInner (path:string) (node:Node<'a>) = 
    seq {
      match node with
      | Value(value) -> yield (path, value)
      | Node(left, right) -> 
          yield! toLookupInner (path + "0") left
          yield! toLookupInner (path + "1") right
    }
  let subNode = getNode node
  toLookupInner "" subNode

let getHuffmanTree = countChars >> createTree >> toLookup >> Seq.toList >> List.sortBy(fun (i, c) -> i.Length)

let x = "Hello World" |> getHuffmanTree

let mine = System.IO.File.ReadAllText(@".\code-snippets\exercises\HuffmanTree.fs") |> getHuffmanTree
           