<Query Kind="Program">
  <NuGetReference>Rx-Main</NuGetReference>
  <Namespace>System</Namespace>
  <Namespace>System.Reactive</Namespace>
  <Namespace>System.Reactive.Concurrency</Namespace>
  <Namespace>System.Reactive.Disposables</Namespace>
  <Namespace>System.Reactive.Joins</Namespace>
  <Namespace>System.Reactive.Linq</Namespace>
  <Namespace>System.Reactive.PlatformServices</Namespace>
  <Namespace>System.Reactive.Subjects</Namespace>
  <Namespace>System.Reactive.Threading.Tasks</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Collections.Concurrent</Namespace>
</Query>

//  This is results of an interview with a candidate.  

void Main() {

  IPullable<string> messageQueue = null;
  
  var mq1 = new MessageQueue<string>();
  mq1.Push("Hello");
  mq1.Pull().Dump();
  mq1.Pull().Dump();
}

//  add:       (int, int) -> int
//  makeAdder: int -> (int -> int)

//(x,y) return x + y
//(x) => (y) => x + y



interface IPullable<T> {
  Task<T> Pull();
}

public class SimplestPuller<T> : IPullable<T> {
  private Func<T> _Pull;
  public SimplestPuller(Func<T> pull) {
      _Pull = pull;
  }

  public async Task<T> Pull() {
    return _Pull();
  }


}

public class MessageQueue<T> : IPullable<T> {
  ConcurrentQueue<T> mq;

  public MessageQueue() {
    mq = new ConcurrentQueue<T>();
  }

  public async Task<T> Pull() {
    T tmp;
    while (!mq.TryDequeue(out tmp)) {
      await Task.Delay(5000);
      Console.WriteLine("Checking again");
    } 
    return tmp;
  }

  public void Push(T message) {
    mq.Enqueue(message);
  }
  
}
// interface IPullable<T> : IDisposable
// properties
// stream
// keepAlive
// methods
// <T> pull
// Disposable { if(keepAlive){return;} else {do something to close the stream}}