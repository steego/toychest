# Why are we still writing a lot of code to create web applications?

If I wanted to start prototyping a web application, or event a mobile application, why can't I start creating a user interface by defining a programmatic interface?


```fsharp
    type Projects(projects:IQueryable<Project>) = 
        member this.Get() = SearchView(projects)
        member this.Get(id:int) = projects.FirstOrDefault(fun p -> p.Id = id)
        member this.Save(p:Project) = ()
```

```csharp
    interface MyProjectTracker {
        public User Users { get; }
    }

    interface User {
        PostResult Login(string username, string password);
        GetResult Logout();
    }

    interface Projects {
        SearchView<Project> Get();
        GetResult<Project> Get(int id);
        PostResult Save(Project);
    }

    interface SearchView<T> {
        SearchGrid<T> 
    }
```
