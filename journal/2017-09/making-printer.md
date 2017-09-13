# Making Toys in F# - How to Make an Object Printer

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

<table>
<tr><th>Name</th><td>Bob Jobes</td></tr>
<tr><th>Position</th><td>Developer</td></tr>
<tr><th>DateStarted</th><td>2017-09-13 12:34 PM</td></tr>
</table>

In that spirit, I want to show you how you can make your own toy object dumper and 
then I can show you how we can make it better. 

## Let's Start by Abstracting and Modeling Our Printer's Rules

In order to create an object dumper, we know we're going to need to use some type of reflection.  But what type of information do we want to get?  At this point, I think it makes sense to list out a few use cases to help guide us so we can start prototyping and playing.

Here's where I want to start:

* For simple objects, I want to simply show the property names and values
* For list-like objects, I want to print them in a grid with property names as the header
* Primitive types should be printed with their .ToString method for now.
* Our printer should be recursive, but always limit the depth to some maximum.

## Abstracting our Requirements with Types

First, we know we're making a function.  That prints some HTML.  Let's write that down in F# just 
for our edification.  Here's a first stab

```fsharp
// We'll just assume Html is a string for now, but we may switch it something more structured later.
type Html = string

//  A printer is just a function that takes any System.Object and returns some Html
type Printer = obj -> Html
```

That makes sense, but how do we abstract our list?

Well, we know there are two steps to printing an object.  

1. Get the description the object we're printing
2. Format our object's description in HTML

## Describing an Object

There are many ways we can describe an object.  Rather than getting hung up on a perfect way, let's start 
someplace and improve on it.

```fsharp
type PropertyName = string
type ValueAsString = string

type DescribedObject = 
  | Primitive of string  // Primitives are always convertable to strings
  | SimpleObject of Map<PropertyName, DescribedObject>
  | EnumerableObject of Set<PropertyName> * seq<DescribedObject>
  
type ObjectDescriber = obj -> DescribedObject
```

Here's our starting point.  We're basically saying, there's a type of function called an ObjectDescriber
that takes an obj (System.Object) as an input, and returns a DescribedObject as the output.  Our simple described
object will either say:

* It's a primitive type, and here's the string representation of it.
* It's a simple object with a map of property names to DescribedObjects
* It's an enumerable object with a set of common property names and a sequence of described objects

Again, this isn't complete.  It's a starting point.  We're making a toy to prototype an idea.

## 


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

