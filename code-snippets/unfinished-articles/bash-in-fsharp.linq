<Query Kind="FSharpProgram">
  <NuGetReference>Rx-Main</NuGetReference>
  <Namespace>System</Namespace>
  <Namespace>System.Reactive</Namespace>
  <Namespace>System.Reactive.Concurrency</Namespace>
  <Namespace>System.Reactive.Disposables</Namespace>
  <Namespace>System.Reactive.Joins</Namespace>
  <Namespace>System.Reactive.Linq</Namespace>
  <Namespace>System.Reactive.PlatformServices</Namespace>
  <Namespace>System.Reactive.Subjects</Namespace>
  <Namespace>System.Reactive.Threading.Tasks</Namespace>
</Query>

(*
The Unix command line is a productive tool for composing programs
together.  At the command line, it's very easy to run multiple 
programs and sequence the output of one program into the input
of another by using the pipe operator.

For example, reading a file and looking for a keyword is as easy
as running the cat program and piping into grep to file the output
like so:

  cat Bash-in-FSharp.linq | grep Namespace

In F#, we have similar analogs that allow us to create small programs
as functions, and we can reuse F#'s forward pipe operator to compose the blocks
in the same way.  Keep reading and we'll implement a basic cat and grep.

  cat Util.CurrentQueryPath |> grep "Namespace" |> Dump
    
Unlike Bash, the F#'s forward pipe operator isn't a special operator that's
fundamentally built into the language.  We can define it very simply in F#.
In fact, here's the definition:

  let inline (|>) x f = f(x)

The forward operator here is simply an inlined function.  That means that
writing the following code:

  "bob" |> ToUpper |> Reverse
  
Compiles to something like:

  Reverse(ToUpper("bob"))
  
The pipeline operator simple gives us a simple way to sequence our function
calls so as you're reading from left to right, you can follow the transformations.

## Implementing cat and grep

Like many other languages, creating something resembling a cat and grep can be done
with a few lines of code.  In our example, I want to implement these methods as 
iterator methods.
*)

let cat (path:string) : seq<string> = 
  seq { 
    use stream = System.IO.File.OpenRead(path)
    use reader = new System.IO.StreamReader(stream)
    while reader.EndOfStream = false do
      yield reader.ReadLine()
  }
  
(*
Cat is a function that takes a filepath as a parameter and returns an iterable 
sequence of strings.  The function body is enclosed in braces with the "seq" 
identifier.  Here we're telling F# that we want it to rewrite the body of the code
so it behaves like a lazy iterable.  It will only read the next line when the iterator
asks it to read the next line.

Notice how I'm using the "use" keyword to define the stream and stream reader instead of the
"let" keyword?  The reason I'm doing this is I want this function to close the file and stream
when I'm finished with it.  The "use" keyword ensures this happens when the flow exits the scope. 
*)
  
let grep (pattern:string) (lines:seq<string>) = 
  seq {
    let regex = new Regex(pattern)
    for line in lines do
      if regex.IsMatch(line) then
        yield line
  }
  
(*  

*)


    
(*