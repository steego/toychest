module GlobalRepo

open Microsoft.FSharp.Quotations

type Reference = string
type Json = string
type Hash = string

//  An F# expression whose identifiers have been normalized
//  so expressions with different binding names of the same
//  structure can be compared
type NormalizedExpr = Expr
type Normalizer = Expr -> NormalizedExpr

type SerializedExpr = { Id:Hash; Body: Json; References: Reference list }

type Serializer = Expr -> Json
type Deserializer = Expr -> Json


