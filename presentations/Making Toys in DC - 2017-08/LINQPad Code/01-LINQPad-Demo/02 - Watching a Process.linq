<Query Kind="FSharpProgram" />

open System.IO

let root = @"C:\Projects\"

let files = Directory.EnumerateFiles(root, "*.*", SearchOption.AllDirectories)

for f in files do
    Console.WriteLine(f)