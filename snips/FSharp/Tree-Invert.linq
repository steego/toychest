<Query Kind="FSharpProgram">
  <NuGetReference>FsCheck</NuGetReference>
</Query>

//  If a prospective employer asks you to
//  invert a tree.  Ask if you can do it in F#

type Tree<'a> = 
  | Leaf of Value:'a
  | Branch of Left:Tree<'a> * Right:Tree<'a>
  

let testTree = 
  Branch(
    Branch(Leaf("a1"), Leaf("a2")),
    Branch(Leaf("b1"), Leaf("b2"))
  )
  
let rec invert tree = 
  match tree with
  | Branch(left, right) ->
      Branch(invert right, invert left)
  | Leaf(value) -> Leaf(value)
  
testTree |> invert |> printfn "%A"

//  How do I know this thing really works?  I mix up
//  my left/right all the time.  I *am* fallible
//  despite what my mother tells me.

let doesInvertingTwiceAlwaysReturnOriginal tree =
  tree |> printfn "%A"
  tree |> invert |> invert = tree
  
FsCheck.Check.Quick doesInvertingTwiceAlwaysReturnOriginal

//  If you're lazy like me and prefer to use tools 
//  that do the tedious work for you.  Well...  

//  Come on!  How can you not be excited about the computer
//  automatically testing your code!?!?!?!?!?!?!?

//  Disclaimer:  This is not a rigorous proof my function 
//  worked.  These are just demos.


//  VOILA - Just don't mix up your left and
//  right like me.