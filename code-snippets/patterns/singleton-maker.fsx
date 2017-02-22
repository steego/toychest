//  Singletons made easy
let makeSingleton f =
  let instance = lazy f()
  fun () -> instance.Value