//  Loosly speaking, Active patterns allow you to create
//  functions that are good at extracting structured
//  data.

//  Let make one.

//  I'm going to need my Banana clips (|  |)

open System

let (|Odd|Even|String|)(s:string) = 
    let (isInt, value) = Int32.TryParse(s)
    if isInt then
       if value % 2 = 1 then Odd(value)
       else Even(value)
    else String(s)

let tellMeAbout(s:string) = 
    match s with
    | Odd(value) -> sprintf "%i is odd!" value
    | Even(value) -> sprintf "%i is even!" value
    | String(value) -> sprintf "%s is a string!" value

//  Per this example, active patterns are used in 
//  conjunction wit the match with syntax (Pattern matching)

//  Well talk more.  See you soon...