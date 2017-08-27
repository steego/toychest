<Query Kind="FSharpProgram">
  <NuGetReference>Suave</NuGetReference>
</Query>

//  Let's create a simple web server

module WebServer = begin

    open Suave
    
    let app2(ctx: HttpContext) = 
        async {
            let message = "You requested the page: " + ctx.request.path
            return! Successful.OK message ctx
        }
    
    let url = defaultConfig.bindings.First().ToString()
    
    System.Diagnostics.Process.Start(url)
    
    startWebServer defaultConfig app2

end