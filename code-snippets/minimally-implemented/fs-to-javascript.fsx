//  You can translate *some* F# into JavaScript in under a page

open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns

let rec toJavaScript (e:Expr) = 
    match e with
    | Application(ex1, ex2) -> sprintf "(%s)(%s)" (toJavaScript ex1) (toJavaScript ex2)
    | Let(var, ex, body) -> sprintf "(function (%s) { return %s; })(%s)" (var.Name) (toJavaScript body) (toJavaScript ex)
    | Lambda(var, expr) -> sprintf "(function (%s) { return %s; })" (var.Name) (toJavaScript expr)
    | Call(None, m, [ex1;ex2]) when m.Name = "op_Addition" -> sprintf "(%s + %s)" (toJavaScript ex1) (toJavaScript ex2)
    | Value(o, t) -> o.ToString()
    | Var(var) -> var.Name
    | _ -> "(~~ Can't translate~~~)"

let ex1 = <@ 2 + 2 @> |> toJavaScript
let ex2 = <@ let add x y = x + y in add 2 3 @> |> toJavaScript