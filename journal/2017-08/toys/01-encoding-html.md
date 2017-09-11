# Making Toys with F# - Encoding HTML Documents

I've been working on a talk about the virtues of building toy examples for the purpose
of communicating ideas with simply interactive examples.

The toys I talk about in my presentation are based my interest in tools that allow 
programmers to quickly build web applications that allow them to explore their 
architecture.  So to kickstart this series off, I want to introduce a simple 
component we can use to build other ideas.

## Encoding HTML

HTML tags can be encoded simply with F#, because a basic tag is made up of:

1. A tag name (Let's ignore namespaces)
2. A set of attributes that are basically key-value pairs
3. A list of subtags or text.

We can encode it like this:

````fsharp
type TagName = string
type Attributes = Map<string, string>

type HtmlTag = 
  | HtmlTag of Name:TagName * Attributes:Attributes * Body:Tag list
  | HtmlText of string
````

If we wanted to make a page, an example page might look like this:

````fsharp
let myFanPage =
    HtmlTag("div", Map.empty, [
        HtmlTag("h1", Map.empty, [HtmlText("My Taylor Swift Fan Page")])
        HtmlTag("p", Map.empty, [HtmlText("She's simply amazing...")])
    ])
````

## Cleaning up our F# Flavored Html

Right off the bat, we can agree this example is not only a little ugly, it's also a little verbose.
While I'd love to be able to use an HTML like syntax in my F# ala React, I simply don't know how at this point.
However, we can make our document look a little like Elm with a few helpful functions:

````fsharp
let makeTag name attributes body = 
    Tag(name, (Map.ofList attributes), body)    
    
let div = makeTag "div"
let h1 = makeTag "h1"
let p = makeTag "p"
````

Adding this little bit of code allows use to change our Taylor Swift fan page to look much more respectful:

````fsharp
let myFanPage =
    div [] [
        h1 [] [Text("My Taylor Swift Fan Page")]
        p [] [Text("She's simply amazing...")]
    ]
````

## Using Partial Application to Create Specialized Functions

What's important to note is the makeTag function takes three parameters, but our example only shows us passing 
in one parameter when we called it in our example.  What gives?

In F#, we can call a multiparameter function with less than the expected parameters.  When we do that, we get back a
new function that expects the remaining parameters.  So in our example, we create three specialized functions for creating
HTML tags with each one specialized for each tag.

## Printing our Page

All this is nice, but we have a fan page to publish so we're going to need some code to convert our fan page to HTML.

First, we'll need a few helper functions to encode string into proper HTML.

````fsharp
module Encoders =
    open System.Web
    let inline html(s:string) = HttpUtility.HtmlEncode(s)
    let inline attribute(s:string) = HttpUtility.HtmlAttributeEncode(s)
    let inline url(s:string) = HttpUtility.UrlEncode(s)
````

Next, I want a simple toy function that lets me write my HTML object to a TextWriter class.

````fsharp
let writeToTextWriter (w:TextWriter) (t:Tag) =
    match t with
    | Text(s) -> 
        w.Write("<span>")
        w.WriteLine(Encoders.html(s))
        w.Write("</span>")        
    | Tag(name, attributes, body) -> 
        w.Write(sprintf "<%s" name)
        //  Print the attributes
        for (name,value) in attributes |> Map.toList do
            w.Write(sprintf " %s=\"%s\"" (Encoders.attribute name) (Encoders.attribute value))
        w.WriteLine(">")
        //  Print the body of our tag
        for child in body do
            child |> writeToTextWriter w
        w.WriteLine(sprintf "</%s>" name)
````

Finally, if we want to convert our Tag into a string, we can write a method against our Tag type like so:

````fsharp
type Tag with
    override this.ToString() = 
        use sw = new StringWriter()
        this.Write(sw)
        sw.ToString()
````

That should be enough to get us started.  If we were to take our example and run it:

````fsharp
let myFanPage =
    div [] [
        h1 [] [Text("My Taylor Swift Fan Page")]
        p [] [Text("She's simply amazing...")]
    ]

printfn(myFanPage.ToString())
````

We would expect an output that loosely resembles:

````html
<div>
  <h1>My Taylor Swift Fan Page</h1>
  <p>She's simply amazing...</p>
</div>
````

There's nothing particularly exciting about this example, but stay tuned because 
I'm going to show you how we use this as a building block to create some useful  
components.
