open System.Collections.Concurrent

type Queue<'a>(maxSize:int) = 
    let queue = new BlockingCollection<'a>(maxSize)
    member this.Enqueue(value) = queue.Add(value)
    member this.Dequeue(value) = queue.Take()

let queue = Queue<string>(5)

let watchQueue() = Async.Start <| async {
    while true do
        do! Async.Sleep(0)
        printfn "Waiting..."
        let value = queue.Dequeue()
        do! Async.Sleep(1000)
        printfn "Reading queue: %s" value

    printfn "Done"
}

watchQueue()

queue.Enqueue("Hello World")