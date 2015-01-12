namespace BlocktrailSdk

/// [omit]
module internal Helpers = 
    open Newtonsoft.Json
    open FSharp.Data

    [<Literal>]
    let apiEndpoint = "https://api.blocktrail.com/v1/BTC/"

    let requestGet url key args = 
        Http.RequestString(apiEndpoint + url, query = [ "api_key", key ] @ args, httpMethod = "GET")
    let convertToObject<'T> json = JsonConvert.DeserializeObject<'T>(json)
    let populateObject json obj = JsonConvert.PopulateObject(json, obj)
