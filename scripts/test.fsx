open System.Threading

printfn "Begin Counting"
for i in 1..100 do
  printfn "Counting %i" i
  Thread.Sleep(1000)
printfn "End Counting"