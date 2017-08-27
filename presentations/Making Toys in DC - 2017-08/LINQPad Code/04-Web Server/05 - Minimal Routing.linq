<Query Kind="FSharpProgram">
  <Connection>
    <ID>51c42400-28cd-449f-b326-720e08cda739</ID>
    <Persist>true</Persist>
    <Server>.\SQL2014</Server>
    <IncludeSystemObjects>true</IncludeSystemObjects>
    <Database>AdventureWorks2014</Database>
    <ShowServer>true</ShowServer>
  </Connection>
  <Reference Relative="..\..\..\..\..\github.com\steego\Steego.NET\src\Steego.Demo\bin\Release\Fasterflect.dll">C:\Projects\github.com\steego\Steego.NET\src\Steego.Demo\bin\Release\Fasterflect.dll</Reference>
  <Reference Relative="..\..\..\..\..\github.com\steego\Steego.NET\src\Steego.Demo\bin\Release\Steego.Demo.dll">C:\Projects\github.com\steego\Steego.NET\src\Steego.Demo\bin\Release\Steego.Demo.dll</Reference>
  <Reference Relative="..\..\..\..\..\github.com\steego\Steego.NET\src\Steego.Demo\bin\Release\Suave.dll">C:\Projects\github.com\steego\Steego.NET\src\Steego.Demo\bin\Release\Suave.dll</Reference>
</Query>

//  I believe we should be able to create web applications simply
//  by using simple functions as building blocks.

//  
module UrlNavigation = begin

  //  When we match
  type MatchResult = Option<obj * string list>
  type SegmentNavigator = string list -> obj -> MatchResult
  
  let Matched(rest:string list)(x) = Some((x :> obj), rest)

  let (|Int|_|) value = match Int32.TryParse(value) with 
                        | true,y -> Some(y) 
                        | _ -> None
  
  let matchTypedPath (f:'a -> string list -> MatchResult) (o:obj,path:string list) = 
    match o,path with
    | :? 'a as value, path -> f value path
    | _ -> None  

  let tracePath (navSegment: SegmentNavigator) = fun (path) (obj) ->
    let rec navigatePath path obj = [
        yield path, obj
        if List.isEmpty path then ()
        else
          match navSegment path obj with
          | Some(result, []) -> yield [], result
          | Some(result, rest) -> yield! navigatePath rest result
          | None -> ()
      ]
    navigatePath path obj  
  
end


module AdventureWorksExample = begin
  let dc = new TypedDataContext()
  
  open UrlNavigation
  
  let (|Property|_|) = matchTypedPath <| fun (o:obj) -> function
    | name::rest -> 
        try Fasterflect.PropertyExtensions.GetPropertyValue(o, name) |> Matched rest
        with _ -> None
    | _ -> None
  
  let (|Products|_|) = matchTypedPath <| fun (products:IQueryable<Product>) -> function
    | Int(id)::rest -> 
          products.FirstOrDefault(fun p -> p.ProductID = id) |> Matched rest
    | _ -> None
    
  let (|IQueryable|_|) = matchTypedPath <| fun (query:IQueryable<obj>) -> function
    | "$first"::rest -> query.FirstOrDefault() |> Matched rest
    | "$count"::rest -> query.Count() |> Matched rest
    | "$take"::Int(n)::rest -> query.Take(n) |> Matched rest
    | "$skip"::Int(n)::rest -> query.Skip(n) |> Matched rest
    | _ -> None
  
  
  let rec navigateSegment : SegmentNavigator = fun path o ->
    match o, path with    
    | Products(products, urlTail) -> Some(products, urlTail)
    | IQueryable(query, urlTail) -> Some(query, urlTail)
    | Property(property, urlTail) -> Some(property, urlTail)
    | _ -> None
  
  let navigatePath path o = tracePath navigateSegment path o
  
  
end

open AdventureWorksExample

let testUrls = [
                  "http://test/WorkOrders/$skip/10/$take/5/$first/Product/ProductDocument"
               ]

Steego.Web.Server.start()


open Suave

let app2(ctx: HttpContext) = 
    async {

        let segments = ctx.request.path.Split('/')

        let segments = [ for s in segments -> s.Replace("/", "") ]
                       |> List.filter (String.IsNullOrEmpty >> not)
      
        let (path, obj) = (dc |> navigatePath segments).Last()
        
        //  Let's use LINQPad to automatically print objects
        let html = Util.ToHtmlString(obj)
        
        return! Successful.OK html ctx
    }

Steego.Web.Server.update(app2)