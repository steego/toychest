module Tasks

type Task(name:string, priority:int, complete:bool) = 
    member this.Name = name
    member this.Priority = priority
    member this.Complete = complete

