<Query Kind="FSharpProgram">
  <NuGetReference>EntityFramework</NuGetReference>
</Query>

open System
//open System.
open System.Data.Entity


type [<CLIMutable>] User = { Id:int; Name: string; DB: DateTime; Tasks: Task[]; Projects: Project[] }
and [<CLIMutable>] Task = { Id:int; Short: string; Due: DateTime; Complete: bool; Creator: User; Assignee: User }
and [<CLIMutable>] Project = { Id:int; Owner: User; Tasks: Task[]; Complete: bool }

type ProjectDB = 
  inherit DbContext
  new() = { inherit DbContext() }
  
  member this.Users = Unchecked.defaultof<DbSet<User>> with get, set
  member this.Tasks = Unchecked.defaultof<DbSet<Task>> with get, set
  member this.Projects = Unchecked.defaultof<DbSet<Project>> with get, set
  
//  [<DefaultValue>] val mutable users : DbSet<User>
//  member this.Users with get() = this.users and set v = this.users <- v
//  [<DefaultValue>] val mutable tasks : DbSet<Task>
//  member this.Tasks with get() = this.tasks and set v = this.tasks <- v
//  [<DefaultValue>] val mutable projects : DbSet<Project>
//  member this.Projects with get() = this.projects and set v = this.projects <- v


let initializer = 
  { new IDatabaseInitializer<ProjectDB> with
      member this.InitializeDatabase(ctx) = 
        let conn = ctx.Database.Connection
        let databaseName = conn.Database
        printfn "DB: %s %A %s" databaseName
        use sqlConn = SqlConnection(conn.ConnectionString)
        sqlConn.Open()
        sqlConn.ChangeDatabase("master")
        
        SqlCommand((sprintf "ALTER DATABASE [%s] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;" databaseName), sqlConn).ExecuteNonQuery() |> ignore
        
        SqlCommand((sprintf "DROP DATABASE [%s];" databaseName), sqlConn).ExecuteNonQuery() |> ignore
        printfn "Dropped"
        ctx.Database.CreateIfNotExists()
        printfn "Created"
        ()
  }


Database.SetInitializer(initializer)
  
let test() = 
  let db = new ProjectDB()
  db.Users.Count().Dump()

test()

//CreateDatabase().Dump()