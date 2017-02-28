<Query Kind="FSharpProgram">
  <NuGetReference>FsCheck</NuGetReference>
</Query>

//  If a prospective employer asks you to
//  invert a tree.  Ask if you can do it in F#

//  So Brian Mitchell pointed out a *glaring* error in my
//  tree that FsCheck didn't catch.

//  I encoded it wrong!!

//  Prepare for a major refactor

//  That's good enough for me for now, but let this be a 
//  lesson in properly defining and encoding the problem.

//  I guess I won't be getting that job at Google now :'(

type Tree<'a> = 
  | Empty
  | Branch of Value:'a * Left:Tree<'a> * Right:Tree<'a>
 
let Leaf(a) = Branch(a, Empty, Empty)

let testTree = 
  Branch("root",
    Branch("a", Leaf("a1"), Leaf("a2")),
    Branch("b", Leaf("b1"), Leaf("b2"))
  )
  
let rec invert tree = 
  match tree with
  | Branch(value, left, right) ->
      Branch(value, invert right, invert left)
  | Empty -> Empty
  
testTree |> invert |> printfn "%A"

//  How do I know this thing really works?  I mix up
//  my left/right all the time.  I *am* fallible
//  despite what my mother tells me.

let doesInvertingTwiceAlwaysReturnOriginal tree =
  //tree |> printfn "%A"
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