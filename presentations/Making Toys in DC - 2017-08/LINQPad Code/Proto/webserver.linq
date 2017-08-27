<Query Kind="FSharpProgram">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <NuGetReference>Suave</NuGetReference>
</Query>

//  Starts a process and returns a Process object
let startProcess(filename:string, arguments:string) = begin
    let startinfo = begin
        let startInfo = 
            new ProcessStartInfo(
                fileName = filename,
                arguments = arguments
            )
        startInfo.UseShellExecute <- false
        startInfo.RedirectStandardInput <- true
        startInfo.RedirectStandardOutput <- true
        startInfo
    end
    
    new Process(StartInfo = startinfo)
end

type Response = { Value: string }

type ReqResponseProcess(filename:string, arguments:string) = class
    let proc = startProcess(filename, arguments)
    let started = proc.Start()
    
    let inStream = proc.StandardInput
    let outStream = proc.StandardOutput

    let rec readUntilStarted() = 
        let line = outStream.ReadLine()
        if line <> "Started" then readUntilStarted()
        
    do readUntilStarted()

    member this.Filename = filename
    member this.Send(input:string) = 
        inStream.WriteLine(input)
        let output = outStream.ReadLine()
        let response = Newtonsoft.Json.JsonConvert.DeserializeObject<Response>(output)
        response.Value
    
    member this.Close() = 
//        inStream.Dispose()
//        outStream.Dispose()
//        proc.Dispose()
        proc.Kill()
end
        

let lprun = @"C:\apps\LINQPad\LPRun.exe"
let script = @"C:\Projects\LINQPad\Journal\2017-08\Presentation\Proto\Scripts\test.linq"

let proc1 = ReqResponseProcess(lprun, script)

let rec doLoop() = begin
    let input = Console.ReadLine()
    if input <> "quit" then begin
        let output = proc1.Send(input) 
        Util.RawHtml(output).Dump()
        doLoop()
    end
end



doLoop()

proc1.Close()

//module WebServer = begin
//
//    open Suave
//    
//    let app2(ctx: HttpContext) = 
//        async {
//            let message = "You requested the page: " + ctx.request.path
//            return! Successful.OK message ctx
//        }
//    
//    let url = defaultConfig.bindings.First().ToString()
//    
//    System.Diagnostics.Process.Start(url)
//    
//    startWebServer defaultConfig app2
//
//end