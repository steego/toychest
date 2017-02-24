//  A simple updatable web server
//  Comment out after you load it!!
#load "web-server.fsx"
//  I love LINQPad's dump
#load "dumper.fsx"

open System
open Dumper

type Project = { 
    Summary: string
    DueDate: DateTime 
  }

type Person = { 
    Name:string
    Salary: int
    Projects: Project list 
  }
let peopleList = [
  { 
    Name = "Robert Jones"
    Salary = 1000000
    Projects = 
      [
        { Summary = "Make Printer"; DueDate = DateTime.Now.AddDays(-1.0) }
        { Summary = "List Projects"; DueDate = DateTime.Now.AddDays(-2.0) }
      ]
  }
  { Name = "Larry David"
    Salary = 1000000; 
    Projects = 
      [
        { Summary = "Write show about something"; DueDate = DateTime.Now.AddDays(-1.0) }
      ] 
  }
]

dump(peopleList)
