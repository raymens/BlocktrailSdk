/// [omit]
[<AutoOpen>]
module internal BlocktrailSdk.Helpers

open Newtonsoft.Json
open FSharp.Data

/// Base address for (most/all) HTTP requests
[<Literal>]
let apiEndpoint = "https://api.blocktrail.com/v1/tBTC/"

/// Useragent to make this SDK identifiable by Blocktrail
[<Literal>]
let userAgent = "raymens-blocktrail-sdk-" + System.AssemblyVersionInformation.Version

/// Do a HTTP GET request given a specific api_key and other query arguments
let requestGet url key args = 
    Http.RequestString(apiEndpoint + url, query = [ "api_key", key ] @ args, httpMethod = "GET", headers = [ HttpRequestHeaders.UserAgent userAgent ])

/// Convert json string to object 'T
let convertToObject<'T> json = JsonConvert.DeserializeObject<'T>(json)

/// Convert object to json string
let jsonEncode t = JsonConvert.SerializeObject(t)

/// Add values from json string to object
let populateObject json obj = JsonConvert.PopulateObject(json, obj)

/// Convert byte array to a HEX string
let convertByteArray2String (bytes : byte array) = 
    //TODO: check performance and reliability
    bytes |> Array.fold (fun a x -> a + x.ToString("X2").ToLower()) ""
    //System.BitConverter.ToString(bytes).Replace("-", "").ToLower();

/// Convert Unicode string to byte array
let readUnicodeBytes (str : string) : byte array = 
    System.Text.Encoding.Unicode.GetBytes(str)

/// Convert ASCII string to byte array
let readAsciiBytes (str : string) : byte array = 
    System.Text.Encoding.ASCII.GetBytes(str)

/// Convert HEX string to byte array
let hexDecode (str : string) : byte array =
    str
    |> Seq.windowed 2
    |> Seq.mapi (fun i j -> (i , j))
    |> Seq.filter (fun (i, j) -> i % 2 = 0)
    |> Seq.map (fun (_, j) -> System.Byte.Parse(new System.String(j),System.Globalization.NumberStyles.AllowHexSpecifier))
    |> Array.ofSeq 

/// Convert byte array to BASE64
let base64 (bytes : byte array) : string = 
    System.Convert.ToBase64String(bytes)

/// Calulate MD5 hash of a string
let calculateMD5 (str : string) : string = 
    use md5 = System.Security.Cryptography.MD5.Create()
    let bts = readAsciiBytes str
    let retVal = md5.ComputeHash(bts)
    convertByteArray2String retVal

/// Calculate the HMAC-SHA256 hash of a byte array using a key
let calculateHMACSHA256 (data : byte array) (key : byte array) = 
    use hmac = new System.Security.Cryptography.HMACSHA256(key)
    hmac.ComputeHash(data)

/// Convert a DateTime to RFC-1123 for HTTP request headers
let rfc1123 (date : System.DateTime) = date.ToString("R")

let testRequest : System.Net.Http.HttpRequestMessage =
    let req = new System.Net.Http.HttpRequestMessage()
    req.Headers.Host <- "blocktrail.com"
    req.Headers.Add("Date", rfc1123 System.DateTime.UtcNow)
    req.Content <- new System.Net.Http.StringContent("""{ "Name": "Raymen" }""")
    req.Content.Headers.Add("ContentType", "application/json")

    req

let signatureString (request : System.Net.Http.HttpRequestMessage) (headers : string list) =
    
    let getValue (req : System.Net.Http.HttpRequestMessage) h =
        let x = ref Seq.empty
        let success = req.Headers.TryGetValues(h, x)
        x.Value |> Seq.head

    let signatureList = headers |> List.map (fun h -> sprintf "%s: %s" h (getValue request h))

    let signatureCollected = System.String.Join(System.Environment.NewLine, List.toArray signatureList)

    printfn "%s" signatureCollected
    
signatureString (testRequest) [ "Date"; "Host" ]

/// 
//TODO: generic request
let httpRequest url (query : (string * string) list) (httpMethod : string) (headers : (string * string) list) body =
    let httpRequest = new System.Net.Http.HttpRequestMessage()
    
    let messageHeaders = headers |> List.filter (fun (name, _) -> name.StartsWith("Content") = false)
    let contentHeaders = headers |> List.filter (fun (name, _) -> name.StartsWith("Content"))

    messageHeaders |> List.iter (fun (name, value) -> httpRequest.Headers.Add(name, value))
    
    match httpMethod with
    | "DELETE" -> httpRequest.Method <- System.Net.Http.HttpMethod.Delete
    | "GET" -> httpRequest.Method <- System.Net.Http.HttpMethod.Get
    | "POST" -> httpRequest.Method <- System.Net.Http.HttpMethod.Post
    | s -> failwithf "HTTP Method not implemented: %s" s
    
    httpRequest.RequestUri <- new System.Uri(url)
    httpRequest.Content <- new System.Net.Http.StringContent(body)
    
    contentHeaders |> List.iter (fun (name, value) -> httpRequest.Content.Headers.Add(name, value))
    
    use client = new System.Net.Http.HttpClient()
    let response = (client.SendAsync(httpRequest)).Result
    
    (response.IsSuccessStatusCode, (int) response.StatusCode, response.Content.ReadAsStringAsync().Result)