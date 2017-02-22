#load "dumper.fsx"

open System
open Dumper

type Project = { Summary: string; DueDate: DateTime }

type Person = { Name:string; Salary: int; Projects: Project list }

let printPersonList = Printer.makePrinter<Person list>()
let people = [
  { 
    Name = "Robert Jones"; Salary = 1000000; 
    Projects = 
      [
        { Summary = "Make Printer"; DueDate = DateTime.Now.AddDays(-1.0) }
        { Summary = "List Projects"; DueDate = DateTime.Now.AddDays(-2.0) }
      ]
  }
  { Name = "Larry"; Salary = 1000000; Projects = [] }
]

dump(people)