<Query Kind="FSharpProgram">
  <NuGetReference>fasterflect</NuGetReference>
</Query>

open System


type SimplePropFetcher = string -> obj -> obj option

//  A property fetcher is simply a function that allows
//  you to try to fetch a property from an object.

//  IT DOESN'T HAVE TO BE RESTRICTED TO PROPERTIES

//  You can resolve indexers
//  


type TypedPropFetcher = Type -> SimplePropFetcher