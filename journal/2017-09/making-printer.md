# How to Make an Object Printer in F#

If you know me, you know LinqPad is one of my favorite tools of all time.  It's 
a simple code snippet tool with some pretty incredible features.  One of my
favorite features is their object dumper.

Anytime you want to want to see the contents of an object or variable, simply call 
the extension method Dump like so:

```csharp
var person = new { Name = "Bob Jones", Position = "Developer", DateStarted = DateTime.Now };

person.Dump();
```

You end up with something that looks like this:

<insert picture>

In that spirit, I want to show you how you can make your own toy object dumper and 
then I can show you how we can make it better. 

## Start with Simple Reflection

In order to create an object dumper, we’ll need to use reflection to get information about an object.  For example, is the type of thing we're printing a simple primitive type like a string or a boolean?  Is it an object with public properties?  Is it enumerable?  Is it a generic type?

All of these questions are important if we're going to use that information to figure out how we're going to print an object. 

To get started, we'll keep things simple and start with a few use cases:

* Primitives are printed using default ToString method. 
* Simple objects should show names and values of their properties 
* Enumerable types should display values in a grid.
* Our printer should be recursive, but always limit the depth to some maximum.

## Encoding our Rules into Types 

Let's spend a little time thinking about our rule by thinking about how we might represent those rules.

First, we're making a function.  Let's assume this function is going to accept any type of 
input and return some HTML output.  Here's a first stab and one way at encoding this:

```fsharp
// We'll just assume Html is a string for now, but we may switch it something more structured later.
type Html = string

//  A printer is simply an function that takes any System.Object and returns Html
type Printer = obj -> Html
```

## Breaking it Down - First Describe Our Objects, Then Print that Description

Ok.  We'll start with that as our general premise for now.  It explains what's important to the 
end user (They just want a magic function that dumps), but let's see if we can use our encoding
tools to model *how* we're going to print our object.

First, remember I listed out 3 types of objects we're going to deal with:

```fsharp

type PropertyName = string
type ValueAsString = string

type DescribedObject = 
  | Primitive of string  // Primitives are always convertable to strings
  | SimpleObject of Map<PropertyName, DescribedObject>
  | EnumerableObject of Set<PropertyName> * seq<DescribedObject>
```

This is one (not necessarily correct) way we can represent what we had in our 
requirements, especially if we think about our printer breaking up the job into
two parts:

```fsharp

type Describer = obj -> DescribedObject
type DescriptionPrinter = DescribedObject -> Html

```

By composing our Describer and DescriptionPrinter together, we get a plain old object printer.

