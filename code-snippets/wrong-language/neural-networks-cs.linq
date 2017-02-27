<Query Kind="Program" />

//  A C# port of Raymond Hettinger's IAC neural networks he uses to teach kids AI
//  
//  http://code.activestate.com/recipes/496908-data-mining-with-neural-nets/
//
//  How would you translate this to F# and a functional style?

string SampleFile = @"
Art Jets        40      jh sing    pusher
Al          Jets        30      jh mar     burglar
Sam         Jets        20      col sing    bookie
Clyde       Jets        40      jh sing    bookie
Mike        Jets        30      jh sing    bookie
Jim         Jets        20      jh div     burglar
Greg        Jets        20      hs mar     pusher
John        Jets        20      jh mar     burglar
Doug        Jets        30      hs sing    bookie
Lance       Jets        20      jh mar     burglar
George      Jets        20      jh div     burglar
Pete        Jets        20      hs sing    bookie
Fred        Jets        20      hs sing    pusher
Gene        Jets        20      col sing    pusher
Ralph       Jets        30      jh sing    pusher

Phil        Sharks      30      col mar     pusher
Ike         Sharks      30      jh sing    bookie
Nick        Sharks      30      hs sing    pusher
Don         Sharks      30      col mar     burglar
Ned         Sharks      30      col mar     bookie
Karl        Sharks      40      hs mar     bookie
Ken         Sharks      20      hs sing    burglar
Earl        Sharks      40      hs mar     burglar
Rick        Sharks      30      hs div     burglar
Ol          Sharks      30      col mar     pusher
Neal        Sharks      30      hs sing    bookie
Dave        Sharks      30      hs div     pusher
";




void Main() {
  var nn = new NeuralNet();
  nn.Load(SampleFile);
  nn.Touch("Sharks 20 jh sing burglar");
  nn.Run();
}

public class NeuralNet {
  const double minact = -0.2, rest = -0.1, thresh = 0.0, decay = 0.1, maxact = 1.0;
  const double alpha = 0.1, gamma = 0.1, estr = 0.4;

  public static Dictionary<string, Unit> UnitByName = new Dictionary<string, Unit>();
  List<Unit> units = new List<Unit>();
  List<Pool> pools = new List<Pool>();

  public class Unit {
    public string Name { get; set; }
    public Pool Pool { get; set; }
    public double Activation { get; set; } = 0.0;
    public double Output { get; set; } = 0.0;
    public List<Unit> Exciters { get; set; } = new List<Unit>();
    public double ExtInp { get; set; } = 0.0;
    public double NewAct { get; set; } = 0.0;

    public Unit(string name, Pool pool) {
      Name = name;
      Pool = pool;
      UnitByName.Add(name, this);
    }

    public void AddExciter(Unit aunit) => Exciters.Add(aunit);
    public void Remove(Unit aunit) => Exciters.Remove(aunit);
    public void CommitNewAct() => SetActivation(this.NewAct);
    public void SetExt(double weight) => ExtInp = weight;

    public void SetActivation(double v) {
      Activation = v;
      Output = Math.Max(thresh, v);
    }

    public void Reset() {
      SetExt(0.0);
      SetActivation(0.0);
    }

    public void ComputeNewAct() {
      var ai = Activation;
      var plus = Exciters.Sum(e => e.Output);
      var minus = Pool.Sum - Output;
      var netinput = alpha * plus - gamma * minus + estr * ExtInp;
      if (netinput > 0.0) {
        ai = (maxact - ai) * netinput - decay * (ai - rest) + ai;
      } else {
        ai = (ai - minact) * netinput - decay * (ai - rest) + ai;
      }
      NewAct = Math.Max(Math.Min(ai, maxact), minact);
    }


  }

  public class Pool {
    public double Sum { get; set; } = 0.0;
    public List<Unit> Members { get; set; } = new List<Unit>();
    public void AddMember(Unit m) => Members.Add(m);
    public void UpdateSum() {
      Sum = Members.Sum(m => m.Output);
    }

    public void Display() {
      var result = Members
                    .Select(u => new { u.Activation, u.Name })
                    //.Where(u => u.Activation > 0)
                    .Distinct()
                    .OrderByDescending(u => u.Activation).ToArray();
      result.Dump();
    }
  }

  private string[][] ReadData(string data) {
    return (from line in Regex.Split(data, "[\r\n]")
            let values = (from v in Regex.Split(line.Trim(), @"[\s\t]+")
                          where String.IsNullOrWhiteSpace(v) == false
                          select v).ToArray()
            where values.Length > 0
            select values).ToArray();
  }

  public void Load(string data) {
    units = new List<Unit>();
    pools = new List<Pool>();
    foreach (var relatedUnits in ReadData(data)) {
      if (relatedUnits.Length > 0) {
        var key = units.Count;
        for (int poolNum = 0; poolNum < relatedUnits.Length; poolNum++) {
          var name = relatedUnits[poolNum];
          if (poolNum >= pools.Count) {
            pools.Add(new Pool());
          }
          var pool = pools[poolNum];
          Unit unit;
          if (UnitByName.ContainsKey(name)) {
            unit = UnitByName[name];
          } else {
            unit = new Unit(name, pool);
            units.Add(unit);
          }
          pool.AddMember(unit);
          if (poolNum > 0) {
            units[key].AddExciter(unit);
            unit.AddExciter(units[key]);
          }
        }
      }
    }
  }

  public void Reset() {
    foreach (var u in units) {
      u.Reset();
    }
  }

  public void Depair(string i, string j) {
    UnitByName[i].Remove(UnitByName[j]);
    UnitByName[j].Remove(UnitByName[i]);
  }

  public void Touch(string itemStr, double weight = 1.0) {
    foreach (var name in itemStr.Split(' ')) {
      UnitByName[name].SetExt(weight);
    }
  }

  public void Run(int times = 100) {
    for (int i = 0; i < times; i++) {
      foreach (var pool in pools) {
        pool.UpdateSum();
      }
      foreach (var unit in units) {
        unit.ComputeNewAct();
      }
      foreach (var unit in units) {
        unit.CommitNewAct();
      }
    }
    foreach (var pool in pools) {
      pool.Display();
    }
  }

}