// ******************** 1. Define out HtmlTag Type ******************

type TagName = string
type Attributes = Map<string, string>

type HtmlTag = 
  | HtmlTag of Name:TagName * Attributes:Attributes * Body:HtmlTag list
  | HtmlText of string
  
// ******************** 2. A Simple Page to Test It ******************
  
let myFanPage1 =
    HtmlTag("div", Map.empty, 
        [
            HtmlTag("h1", Map.empty, [HtmlText("My Taylor Swift Fan Page")])
            HtmlTag("p", Map.empty, [HtmlText("She's simply amazing...")])
        ])

// *************** 3. Let's create a teeny tiny DSL ******************

// Can we even really call it a DSL?

let makeTag name attributes body = 
    HtmlTag(name, (Map.ofList attributes), body)    
    
let div = makeTag "div"
let h1 = makeTag "h1"
let p = makeTag "p"

// *************** 4. Test out the cleaned up version ******************

let myFanPage =
    div [] [
        h1 [] [HtmlText("My Taylor Swift Fan Page")]
        p [] [HtmlText("She's simply amazing...")]
    ]
    
// **************** 5. Let's add code to print it ************************

module Encoders = begin
    open System.Web
    let inline html(s:string) = HttpUtility.HtmlEncode(s)
    let inline attribute(s:string) = HttpUtility.HtmlAttributeEncode(s)
    let inline url(s:string) = HttpUtility.UrlEncode(s)
end

open System.IO

let rec writeToTextWriter (w:TextWriter) (t:HtmlTag) = begin
    match t with
    | HtmlText(s) -> 
        w.Write("<span>")
        w.WriteLine(Encoders.html(s))
        w.Write("</span>")        
    | HtmlTag(name, attributes, body) -> 
        w.Write(sprintf "<%s" name)
        //  Print the attributes
        for (name,value) in attributes |> Map.toList do
            w.Write(sprintf " %s=\"%s\"" (Encoders.attribute name) (Encoders.attribute value))
        w.WriteLine(">")
        //  Print the body of our tag
        for child in body do
            child |> writeToTextWriter w
        w.WriteLine(sprintf "</%s>" name)
end

let toString(t:HtmlTag) = 
    use sw = new StringWriter()
    t |> writeToTextWriter sw
    sw.ToString()
    
        
// ******************** Let's print it *************************

myFanPage |> toString |> printfn "%s"
