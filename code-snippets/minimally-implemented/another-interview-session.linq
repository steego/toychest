<Query Kind="Program" />

void Main() {
  var list1 = Enumerable.Range(1, 10).Select(x => x + 1);
  
  var list2 = from x in Enumerable.Range(1, 10)
              select x + 1;

  var counter = 0;
  
  var source = new PipeLine<int>(() => {
    return counter++;
  });
  //var chain = source.Where(x => x % 3 == 0).Select(x => x + 1).Select(x => x * 3).Select(x => x.ToString());
  
  var chain = from x in source
              where x % 3 == 0
              let add1 = x + 1
              let doubled = add1 * 2
              select new {
                x, 
                add1,
                doubled
              };
  
  chain.Pull().Dump();
  chain.Pull().Dump();
  chain.Pull().Dump();
  chain.Pull().Dump();
  chain.Pull().Dump();

}

//  Source --> Puller --> Puller --> Puller --> Puller

//  Way #1:  Give Func<T>
//  

public class PipeLine<T> {
  private readonly Func<T> PullFunction;
  
  public PipeLine(Func<T> pullFunction) {
    PullFunction = pullFunction;
  }
  
  public T Pull() {
    return PullFunction();
  }

  public PipeLine<U> Select<U>(Func<T, U> map) {
    return new PipeLine<U>(() => {
      var x = this.Pull();
      return map(x);
    });
  }

  public PipeLine<T> Where(Func<T, bool> test) {
    return new PipeLine<T>(() => {
      var success = false;
      do {
        var x = this.Pull();
        success = test(x);
        if(success) return x;
      } while(!success);
      return default(T);
//      var x = this.Pull();
//      if (test(x)) {
//        return x;
//      } else {
//        return default(T);
//      }
    });
  }
}