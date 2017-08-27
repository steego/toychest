<Query Kind="FSharpProgram">
  <Connection>
    <ID>51c42400-28cd-449f-b326-720e08cda739</ID>
    <Persist>true</Persist>
    <Server>.\SQL2014</Server>
    <IncludeSystemObjects>true</IncludeSystemObjects>
    <Database>AdventureWorks2014</Database>
    <ShowServer>true</ShowServer>
  </Connection>
  <NuGetReference>Suave</NuGetReference>
</Query>

let dc = new TypedDataContext()



module WebServer = begin

    open Suave
    
    let app2(ctx: HttpContext) = 
        async {
            //  Let's use LINQPad to automatically print objects
            let html = Util.ToHtmlString(dc.Products)
            
            return! Successful.OK html ctx
        }
    
    let url = defaultConfig.bindings.First().ToString()
    
    System.Diagnostics.Process.Start(url)
    
    startWebServer defaultConfig app2

end