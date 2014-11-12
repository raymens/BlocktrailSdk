namespace BlocktrailSdk

/// [omit]
module internal Helpers = 
    open Newtonsoft.Json
    open FSharp.Data
    open BlocktrailSdk.Config
    
    let apiEndpoint = "https://api.blocktrail.com/v1/BTC"
    let requestGet url key args = 
        Http.RequestString
            ("https://api.blocktrail.com/v1/btc/" + url, query = [ "api_key", key ] @ args, httpMethod = "GET")
    let convertToObject<'T> json = JsonConvert.DeserializeObject<'T>(json)
    let populateObject json obj = JsonConvert.PopulateObject(json, obj)

    let request url args = requestGet url ApiKey args

    let getLastBlockResponse() = 
        let url = sprintf "block/latest"
        request url []

    let getAllBlocksResponse page limit sort_dir = 
        let url = sprintf "all-blocks"
        request url [ "page", string page
                      "limit", string limit
                      "sort_dir", sort_dir ]

    let getBlockResponse block = 
        let url = sprintf "block/%s" block
        request url []

    let getBlockTransactionsResponse block page limit sort_dir = 
        let url = sprintf "block/%s/transactions" block
        request url [ "page", string page
                      "limit", string limit
                      "sort_dir", sort_dir ]