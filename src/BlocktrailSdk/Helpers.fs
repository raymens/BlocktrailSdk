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

/// Do a HTTP DELETE request given a specifc api_key, json encoded data and optinal additional headers
let requestDelete url key stringData headers =
    let body = HttpRequestBody.TextRequest(stringData)
    Http.RequestString(apiEndpoint + url, query = [ "api_key", key ], httpMethod = "DELETE", headers = [ HttpRequestHeaders.UserAgent userAgent ] @ headers, body = body)

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