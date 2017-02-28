<Query Kind="FSharpProgram">
  <NuGetReference>Serilog</NuGetReference>
  <NuGetReference>System.Reactive</NuGetReference>
</Query>

open System.Linq
open System.Reactive.Linq

let logEvent = Event<(string * string)>()
let observable = logEvent.Publish :> IObservable<string * string> 

let split(s:string) = s.Split('.') |> List.ofArray |> List.filter(fun s -> String.IsNullOrWhiteSpace(s) = false)
let head(s) = match s with
              | head::rest -> head
              | _ -> ""

type Splitter<'a>(obs:IObservable<string list * 'a>, key:string) = 
  let reduce = function
    | (path::rest, item) -> (rest, item)
    | ([], item) -> ([], item)
  member this.Key = key
  member this.All = obs.Where(fun (path,o) -> List.isEmpty path).Select(snd)
  member this.Children = obs.Where(fun (path,o) -> head path <> "" )
                            .GroupBy(fun (path,o) -> head path )
                            .Select(fun o -> Splitter(o.Select(reduce), o.Key))

let obs = observable.Select(fun (path, o) -> (split path, o))
  
Splitter(obs, "").Dump()

Async.Start(async {
  while true do
    logEvent.Trigger("", "One")
    logEvent.Trigger("Main", sprintf "%O" (DateTime.Now))
    logEvent.Trigger("Main.New", "Bob")
    logEvent.Trigger("Users.New", "Bob")
    logEvent.Trigger("Users.WEST.New", "Jim")
    logEvent.Trigger("Users.WEST.Updated", "Jim")
    do! Async.Sleep(1000)
})


//type KeyTree(map:Map<string,KeyTree>) = 
//  member this.Map = map
//  member this.Add(key, value) = KeyTree(null,  (map |> Map.add key value))
//  new() = KeyTree(null, Map.ofList<string,KeyTree>([]))
    
  //let parts = key.Split(".") |> List.ofArray
  

//let tree = KeyTree()
//
//tree.Add("Bob", "Larry").Dump()

//fromKeyValue "Name" "Bob"
//|> Map.add "Name" "Jim"
//|> Map.add "Name" "Larry"
//|> Map.remove "Name"
//|> Dump


//Dump [
//  Node("Main", 2, [Node("Test", 3, [])])
//]

//log "Main" "Event1"
//log "Main.Test" "Event1"

//module Logger = 
//  let logEvent = Event<string>()
//  
//  logEvent.Publish.Dump()
//  
//  let mySink = 
//    { new Serilog.Core.ILogEventSink with
//        member this.Emit(e) = 
//          e.Dump()
//          logEvent.Trigger(e.RenderMessage())
//    }
//
//  let loggerConfig = Serilog.LoggerConfiguration()
//                      .WriteTo.Sink(mySink)
//  
//  let logger : Serilog.ILogger = loggerConfig.CreateLogger() :> Serilog.ILogger
//
//module Test =
//  let log = Logger.logger
//
//  do 
//    Async.Start(async {
//      log.Information("Hello {Name}", "Bob")
//      do! Async.Sleep 1000
//      log.Information("Hello {Name}", "Jim")
//    })

//Map.ofList [("One",Util.Di("http://www.google.com"))] |> Dump
