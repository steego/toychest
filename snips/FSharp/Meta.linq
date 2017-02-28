<Query Kind="FSharpProgram">
  <Reference>&lt;RuntimeDirectory&gt;\WPF\PresentationCore.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\WPF\PresentationFramework.dll</Reference>
</Query>

open LINQPad
open System.ComponentModel
open System.Collections.ObjectModel
open System.Collections.Specialized

let toObservable(data:seq<'a>) = new ObservableCollection<'a>(data)

module UI = 
  open System.Windows
  open System.Windows.Forms
  
  
  let makeGrid<'a>(data:ObservableCollection<'a>) = 
    let grid = new System.Windows.Controls.DataGrid()
    
    grid.ItemsSource <- new System.Collections.ObjectModel.ObservableCollection<'a>(data)
    grid


type Info(x:int) = 
  let ev = new Event<_,_>()
  let mutable x = x
  member this.X 
    with get() = x
    and set(v) = 
      x <- v
      ev.Trigger(this, PropertyChangedEventArgs("X"))
      ev.Trigger(this, PropertyChangedEventArgs("Squared"))
      
  member this.Squared = x * x
  new() = Info(0)
  interface INotifyPropertyChanged with
    [<CLIEvent>]
    member x.PropertyChanged = ev.Publish
  
  

let obs = toObservable([for x in 1..10 -> Info(x)])


let collectionChanged = obs :> INotifyCollectionChanged
collectionChanged.CollectionChanged.Dump("Collection Changed")
let changed = obs :> INotifyPropertyChanged
changed.PropertyChanged.Dump("Property Changed")

let grid = UI.makeGrid(obs).Dump()
  
//  
//Async.Start(async {
//  for x in 1..100 do
//    do! Async.Sleep(1000)
//    printfn "%i" x
//    obs.Add(Info(x))
//
//
//})