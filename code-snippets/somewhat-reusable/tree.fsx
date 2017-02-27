
type tree<'a> = { 
  Value: 'a
  Children: tree<'a> list 
}

module Tree = 
  let root(value:'a) = { Value = value; Children = [] }
  let rec expand(max) (getSubTree:'a -> 'a seq) (value:'a) : tree<'a> = 
    { 
      Value = value
      Children = if max <= 0 then []
                 else [ for v in getSubTree value do
                          yield expand (max - 1) getSubTree v  ]
    }
  let rec map f (tree:tree<_>) = 
    {
      Value = f tree.Value
      Children = [ for c in tree.Children -> map f c ]
    }
  let rec filter f (tree:tree<_>) = 
    {
      Value = tree.Value
      Children = [ for c in tree.Children do
                     if f c.Value = true then
                       yield c ]
    }
  let rec bind (f:'a -> tree<'b>) (tree:tree<'a>) =
    let newTree = f tree.Value
    {
      Value = newTree.Value
      Children = [ for c in newTree.Children do
                     yield c
                   for c in tree.Children do
                     yield bind f c ]
    }
  let expandMap f = bind (expand 1 f)
  let rec walk (path:'a list) (tree:tree<'a>) =
    seq { 
      let value = tree.Value
      yield (path, tree.Value)
      let newPath = value::path
      for c in tree.Children do
        for p in walk newPath c do
          yield p
    }