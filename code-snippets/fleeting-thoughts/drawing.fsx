#r @"..\..\packages\Eto.Forms\lib\net45\Eto.dll"
#r @"..\..\packages\Eto.Platform.Windows\lib\net45\Eto.WinForms.dll"

open System
open Eto.Drawing
open Eto.Forms

let app = new Application()

module Graphics =

  type Instruction = 
    | DrawLine of Color * PointF * PointF
    | DrawText of Font * Color * PointF * string
    
  let Draw (g:Graphics) = function
    | DrawLine(c, s, e) -> g.DrawLine(c, s, e)
    | DrawText(f, c, p, s) -> g.DrawText(f, c, p, s)

  let DrawRect(color, p1:PointF, p2:PointF) = seq {
    yield DrawLine(color, PointF(p1.X, p1.Y), PointF(p1.X, p2.Y))
    yield DrawLine(color, PointF(p1.X, p2.Y), PointF(p2.X, p2.Y))
    yield DrawLine(color, PointF(p1.X, p1.Y), PointF(p2.X, p1.Y))
    yield DrawLine(color, PointF(p2.X, p1.Y), PointF(p2.X, p2.Y))
  }
  
  let drawSeq (g:Graphics) (instructions:seq<Instruction>) = 
      for i in instructions do
        Draw g i
  let drawMe() = 
    seq {
      let blue = Color.Parse("blue")
      let black = Color.Parse("black")
      let font = new Font(FontFamilies.Sans, 30.0f)
      yield! DrawRect(blue, PointF(0.0f, 0.0f), PointF(200.0f, 200.0f))
      let text = sprintf "It's %s" (DateTime.Now.ToString())
      yield DrawText(font, black, PointF(200.0f, 200.0f), text)
    }

type MyForm(instructions:unit -> seq<Graphics.Instruction>) as this = 
  inherit Form()
  do
    this.Title <- "Hello"
    this.ClientSize <- new Size(600, 600);
    this.Maximize()
    let canvas = new Drawable()
    this.Content <- canvas
    let draw (o:obj) (e:PaintEventArgs) = Graphics.drawSeq e.Graphics (instructions())
    canvas.Paint.AddHandler(new EventHandler<PaintEventArgs>(draw))
  member this.Update(draw) = 

    this
  
  static member Main() = 
    let app = new Application()
    app.Run(new MyForm(Graphics.drawMe))
    ()
    
let app = MyForm.Main()