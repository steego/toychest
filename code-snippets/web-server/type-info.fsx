//  A reflection module to extract information about types

module TypeInfo

open System
open System.Collections
open System.Reflection
open System.Linq
let isPrimitiveType(t:Type) = t.IsValueType || t = typeof<string>
let isPrimitiveObject(o:obj) = (isNull o || isPrimitiveType(o.GetType()))
let isEnumerable(e:obj) =
    match e with
    | null -> false
    | :? string as s -> false
    | :? seq<_> -> true
    | _ -> false

let isSeq(t:System.Type) = 
    t <> typeof<string> &&
    t.GetInterface(typeof<System.Collections.IEnumerable>.Name) |> isNull |> not
let isGenericSeq(t:Type) = isSeq(t) && t.GenericTypeArguments.Length >= 1


[<Struct>]
type MemberGetter(name:string, memberType:Type, getter:Func<obj,obj>) = 
  member this.Name = name
  member this.Type = memberType
  member this.Get(o) = getter.Invoke(o)
  member this.IsEnumerable = isSeq(memberType)


type TypeInfo(t:Type) = 
    let getPropertyValue(o:obj, p:PropertyInfo) = 
        try p.GetValue(o, null)
        with ex -> null
    let getMemberGetters(t:Type) =
        let isGoodProperty (p:PropertyInfo) = 
            (not p.IsSpecialName)
            && p.GetIndexParameters().Length = 0
            && p.CanRead
            
        let flags = BindingFlags.Public ||| BindingFlags.Instance
        
        [ 
            // Return properties
            for p in t.GetProperties(flags) do
                if isGoodProperty p then
                    yield MemberGetter(p.Name, p.PropertyType, fun o -> getPropertyValue(o, p))
            // Return fields
            for f in t.GetFields() do
                if f.IsPublic && not f.IsSpecialName && not f.IsStatic then
                    yield MemberGetter(f.Name, f.FieldType, fun o -> f.GetValue(o))
        ]

    let elementType = if isNull t then null else t.GenericTypeArguments.FirstOrDefault()
    let members = if isNull t then [] else getMemberGetters(t)
    let isEnumerable = isSeq(t) && t <> typeof<string>
    member this.IsNull = isNull t
    member this.Members = members
    member this.PrimitiveMembers = members |> List.filter(fun m -> isPrimitiveType m.Type)
    member this.ObjectMembers = members |> List.filter(fun m -> (not (isPrimitiveType m.Type)) && (not (isSeq m.Type)))
    member this.EnumerableMembers = members |> List.filter(fun m -> isSeq m.Type)    
    member this.IsSeq = isEnumerable
    member this.IsGenericSeq = isGenericSeq(t)
    member this.ElementType = TypeInfo(elementType)
    new(o:obj) = TypeInfo(if isNull o then null else o.GetType())

let getObjectInfo(o:obj) = if isNull o  then [] else TypeInfo(o.GetType()).Members

let castAs<'T when 'T : null> (o:obj) = 
  match o with
  | :? 'T as res -> res
  | _ -> null

let (|IsSeq|_|) (candidate : obj) =
    if isNull candidate then None
    else begin
        let t = candidate.GetType()

        if t.IsGenericType && t.GetGenericTypeDefinition() = typedefof<seq<_>>
        then Some (candidate :?> System.Collections.IEnumerable)
        else None
    end

let (|IsNullable|_|) (candidate : obj) =
    let t = candidate.GetType()
    if t.IsGenericType && t.GetGenericTypeDefinition() = typedefof<Nullable<_>>
    then Some (candidate)
    else None
let (|IsPrimitive|_|) (candidate : obj) =
    if isPrimitiveObject(candidate) then Some(candidate)
    else None
let (|GenericList|_|)(o:obj) =
    if isNull o then None
    else
        let t = TypeInfo(o.GetType())
        if t.IsGenericSeq then
            let getters = t.ElementType.Members
            let list = o :?> IEnumerable
            Some(getters, list)
        else None

let (|Object|_|)(o:obj) = 
    if isNull o then None
    elif isEnumerable(o) then None
    elif isPrimitiveObject(o) then None
    else
        
        let members = TypeInfo(o.GetType()).Members
        let primitiveMembers = members |> List.filter(fun m -> isPrimitiveType m.Type)
        let objectMembers = members |> List.filter(fun m -> (not (isPrimitiveType m.Type)) && (not (isSeq m.Type)))
        let enumerableMembers = members |> List.filter(fun m -> isSeq m.Type)
        Some(members, primitiveMembers, objectMembers, enumerableMembers, obj)


// module Reflection

// open System
// open System.Collections
// open System.Linq
// let primitiveTypes = [typeof<int>; typeof<string>; typeof<DateTime>]

// let isPrimitive(o:obj) = primitiveTypes.Contains(o.GetType())
// let isList(o:obj) = o :? IEnumerable
// let isObject(o:obj) = true

// let isSeq(t:System.Type) = t.GetInterface(typeof<System.Collections.IEnumerable>.Name) |> isNull |> not
// let isGenericSeq(t:Type) = isSeq(t) && t.GenericTypeArguments.Length >= 1

// let getProperties(t:Type) = 
// [for p in t.GetProperties() do
//     if p.CanRead && p.GetIndexParameters().Length = 0 then
//     let safeGet(o:obj) = if isNull o then null else p.GetValue(o)
//     yield (p.Name, safeGet) ]