//  Go nuts!

type sexpr<'a> = 
  | Value of 'a
  | SList of sexpr<'a> list

module SExpr = 
  type Evaluator<'a> = sexpr<'a> -> sexpr<'a>
  type Debugger<'a> = sexpr<'a> -> (Map<string, sexpr<'a>> * sexpr<'a>) seq