## About F# Dump

This is my little prototype of an exploratory programming tool for the Ionide environment inspired 
by LINQPad.  It's very much a work-in-progress and it's part of a talk that I'm preparing where 
the focus is on building unpolished, unprofessional prototypes and deploying them early.

This is also a personal experiment.  This year, I wanted to join an online community and learn how
to play with others online.  So consider this an open invitation to play with me and my toys.  If 
you think something is lousy, tell me.  If you want to work on something together, let me know.

I accept pull requests, invitations to pair-program, half-baked ideas, passionate criticism and distractions.

## What do I need to do?

1. Clone the repo.
2. Run Paket Install to fetch the Suave Nuget Package
3. Open file-browser.fsx.  Read it and run it.

## What are the components?

| File | What it does |
|------|--------------|
| [file-browser.fsx](file-browser.fsx) | An example prototype |
| [dumper.fsx](dumper.fsx) | A simple module that binds the printer and web server |
| [web-server.fsx](web-server.fsx) | A simple *mutable* web server that can be updated by the REPL |
| [printer.fsx](printer.fsx) | Some hacky object printing code |
| [type-info.fsx](type-info.fsx) | Some reflection code |
| [rules.fsx](rules.fsx) | An experiment that combines recursive combinators |

##  Why is everything an .FSX file and not an .FS file?

I want to keep it simple and fun to experiment.  I'm open to alternative suggestions.



