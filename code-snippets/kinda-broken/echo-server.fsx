open System
open System.IO
open System.Threading
open System.Net
open System.Net.Sockets

type AsyncStreamReader(stream:Stream) = 
  let reader = new StreamReader(stream)
  member this.EndOfStream = reader.EndOfStream
  member this.ReadLineAsync() = reader.ReadLineAsync() |> Async.AwaitTask
  interface IDisposable with
    member this.Dispose() = reader.Dispose()
    
type AsyncStreamWriter(stream:Stream) = 
  let writer = new StreamWriter(stream)
  member this.WriteLineAsync(line:string) = writer.WriteLineAsync(line) 
                                            |> Async.AwaitIAsyncResult 
                                            |> Async.Ignore
  interface IDisposable with
    member this.Dispose() = writer.Dispose()  

type TcpServer(port:int) = 
  let tcpListener = new TcpListener(IPAddress.Any, port)
  member this.Address = sprintf "localhost:%i" port
  member this.ListenForClients() = 
    //  First we start listening
    let rec startListening() = async {
        return! waitForNextClient([])
      }
    //  
    and waitForNextClient(clientList) = async {
        tcpListener.Start()
        let! client = tcpListener.AcceptTcpClientAsync() |> Async.AwaitTask
        let task = this.HandleClientComm(client) |> Async.Start        
        return! cleanOutDisconnectedClients(client::clientList)
      }
    and cleanOutDisconnectedClients(clientList:TcpClient list) = async {
        let cleanedList = clientList |> List.filter (fun c -> c.Connected)
        return! waitForNextClient(cleanedList)
      }
    startListening() |> Async.Start
    
    
  member this.StartServer() = 
    try
      let listenThread = new Thread(new ThreadStart(this.ListenForClients))
      listenThread.Start()
      listenThread.Join()
    with 
      | ex -> 
          Console.WriteLine("Stopping Listening")
          tcpListener.Stop()
          
  member this.HandleClientComm(tcpClient:TcpClient) = async {
    use clientStream = tcpClient.GetStream();
    
    use reader = new AsyncStreamReader(clientStream);
    use writer = new StreamWriter(clientStream);
    Console.WriteLine("New Connection: {0}", tcpClient.Client.RemoteEndPoint.ToString());
    try
      while not(reader.EndOfStream) do
        let! line = reader.ReadLineAsync() 
        //incomingLines.OnNext(line);
        Console.WriteLine(line)
        writer.WriteLine(line.ToUpper())
        writer.Flush()
      done
    finally
      tcpClient.Close();
      Console.WriteLine("Closed Connection")
  }
  
let server1 = TcpServer(9111)
//server1.Dump()
server1.StartServer()
Console.WriteLine("Press enter to stop the process");
Console.ReadLine()