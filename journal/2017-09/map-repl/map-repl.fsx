module App = begin
    let initialState = Map.empty<string, string>

    let set map key value = 
        map |> Map.add<string,string> key value
end

let rec start() = 
    loop(App.initialState)
and loop(map) : unit = 
    let line = System.Console.ReadLine()
    let cmd = line.Split(' ') |> List.ofArray
    match cmd with
    | ["set"; key; value] -> set map key value
    | ["get"; key] -> get map key
    | _ -> invalidInput map line
and set map key value = 
    map |> Map.add key value |> loop
and get map key =
    match map.TryFind(key) with
    | Some(value) -> printfn "Value: %s" value
    | None -> printfn "Empty"        
    map |> loop
and invalidInput map line = 
    printfn "Invalid Input: %s" line
    map |> loop

start()