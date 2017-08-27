<Query Kind="FSharpProgram">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
</Query>

type Result<'a>(value:'a) = 
    member this.Value = Util.ToHtmlString(value)

type Info(line:string) = 
    member this.Line = line
    member this.Length = line.Length

let rec doLoop() = begin
    let input = Info(Console.ReadLine())
    
    let json = Newtonsoft.Json.JsonConvert.SerializeObject(Result(input))
    
    json.Dump()

    doLoop()
end

Console.WriteLine("Started")

doLoop()