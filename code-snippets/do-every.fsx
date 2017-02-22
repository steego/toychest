//  This little snippet creates an agent that performs a job on a
//  schedule.  You can preempt the schedule by posting a unit value to 
//  mailbox.

//  How would you fix or improve this?

open System

type Agent<'a> = MailboxProcessor<'a>

//  Courtesy of Tomas Petricek - http://fssnip.net/8x
type Microsoft.FSharp.Control.Async with
  /// Creates an asynchronous workflow that non-deterministically returns the 
  /// result of one of the two specified workflows (the one that completes
  /// first). This is similar to Task.WaitAny.
  static member Choose(a, b) : Async<'T> = begin
    Async.FromContinuations(fun (cont, econt, ccont) ->
      // Results from the two 
      let result1 = ref (Choice1Of3())
      let result2 = ref (Choice1Of3())
      let handled = ref false
      let lockObj = new obj()
      let synchronized f = lock lockObj f

      // Called when one of the workflows completes
      let complete () = 
        let op =
          synchronized (fun () ->
            // If we already handled result (and called continuation)
            // then ignore. Otherwise, if the computation succeeds, then
            // run the continuation and mark state as handled.
            // Only throw if both workflows failed.
            match !handled, !result1, !result2 with 
            | true, _, _ -> ignore
            | false, (Choice2Of3 value), _ 
            | false, _, (Choice2Of3 value) -> 
                handled := true
                (fun () -> cont value)
            | false, Choice3Of3 e1, Choice3Of3 e2 -> 
                handled := true; 
                (fun () -> 
                    econt (new AggregateException
                                ("Both clauses of a choice failed.", [| e1; e2 |])))
            | false, Choice1Of3 _, Choice3Of3 _ 
            | false, Choice3Of3 _, Choice1Of3 _ 
            | false, Choice1Of3 _, Choice1Of3 _ -> ignore )
        op() 

      // Run a workflow and write result (or exception to a ref cell
      let run resCell workflow = async {
        try
          let! res = workflow
          synchronized (fun () -> resCell := Choice2Of3 res)
        with e ->
          synchronized (fun () -> resCell := Choice3Of3 e)
        complete() }

      // Start both work items in thread pool
      Async.Start(run result1 a)
      Async.Start(run result2 b) )
end

let doEvery (interval:TimeSpan) (block:unit -> Async<unit>) = 
  let rec waitForTrigger(timeStarted:DateTime)(mailbox:Agent<_>) = async {
        //  Wait for the next message
        let! _ = mailbox.TryReceive(int(interval.TotalMilliseconds))        
        //  Wait for the one of these things to happen first
        let r1 = Async.Sleep(int(interval.TotalMilliseconds))
        let r2 = block()
        let! _ = Async.Choose(r1, r2)
        
        //  Clear the mailbox to ensure a trigger backlog doesn't 
        //  cause this to execute the job in rapid succession
        do! clearMailbox mailbox
        return! waitForTrigger DateTime.Now mailbox
    }
  and clearMailbox(mailbox:MailboxProcessor<_>) = 
    async { while mailbox.CurrentQueueLength > 0 do
            let! _ = mailbox.Receive()
            () }
  
  let mailbox = new Agent<unit>(waitForTrigger(DateTime.Now))
  mailbox.Start()
  mailbox

let trigger = doEvery (TimeSpan.FromSeconds(5.0)) (fun () -> async {
    printfn "Hello %A" DateTime.Now
})

trigger.Post()
Console.ReadKey()