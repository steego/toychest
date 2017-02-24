//  An experiment in combining recursive functions

module Rules

type Rule<'a,'b> = ('a -> 'b) -> 'a -> 'b option

let rule<'a>(r:Rule<'a,'b>) = r

let rec combineMax(max:int)(defaultRule:obj -> 'a)(allRules:Rule<obj,'a> list)(o:obj) = 
    if max = 0 then defaultRule obj
    else 
        let recurse = combineMax (max - 1) defaultRule allRules
        let result = allRules |> List.tryPick(fun rule -> rule recurse o)
        match result with
        | Some(result) -> result
        | None -> defaultRule(o)

let rec combine (defaultRule:'a -> 'b)(allRules:Rule<'a,'b> list) = 
    fun (o:'a) ->
        let recurse = combine defaultRule allRules
        let result = allRules |> List.tryPick(fun rule -> rule recurse o)
        match result with
        | Some(result) -> result
        | None -> defaultRule(o)