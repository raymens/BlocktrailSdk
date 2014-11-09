namespace BlocktrailSdk

/// [omit]
module internal Helpers = 
    open Newtonsoft.Json
    open FSharp.Data
    
    let apiEndpoint = "https://api.blocktrail.com/v1/BTC"
    let requestGet url key args = 
        Http.RequestString
            ("https://api.blocktrail.com/v1/btc/" + url, query = [ "api_key", key ] @ args, httpMethod = "GET")
    let convertToObject<'T> json = JsonConvert.DeserializeObject<'T>(json)

open Helpers
open BlocktrailSdk.Models
open System

/// <summary>
/// https://www.blocktrail.com/api/docs#api_data
/// </summary>
type BlocktrailDataClient(apiKey : string) = 
    member val APIKey = apiKey with get, set

    member private x.Request url args = requestGet url x.APIKey args
    
    member x.AllBlocks page limit sort_dir = 
        convertToObject<Paging<Block>> (x.Request "all-blocks" [ "page", string page
                                                                 "limit", string limit
                                                                 "sort_dir", sort_dir ])
    
    member x.GetTransaction transaction = 
        convertToObject<Transaction> (x.Request (sprintf "transaction/%s" transaction) [])

    member x.LatestBlock = convertToObject<Block> (x.Request ("block/latest") [])

    member x.BlockTransactions block = 
        convertToObject<Paging<Transaction>> (x.Request (sprintf "block/%s/transactions" block) [])

    member x.GetBlock block = convertToObject<Block> (x.Request (sprintf "block/%s" block) [])

    member x.AddressUnspentOutputs addr = 
        convertToObject<Paging<Output>> (x.Request (sprintf "address/%s/unspent-outputs" addr) [])

    member x.AddressUnconfirmedTransactions addr = 
        convertToObject<Paging<Transaction>> (x.Request (sprintf "address/%s/unconfirmed-transactions" addr) [])

    member x.AddressTransactions addr = 
        convertToObject<Paging<Transaction>> (x.Request (sprintf "address/%s/transactions" addr) [])

    member x.Address addr = convertToObject<Address> (x.Request (sprintf "address/%s" addr) [])

module BlocktrailClient = 
    
    [<Measure>]
    type sat
    
    [<Measure>]
    type btc
    
    let toBTC (num : int<sat>) : decimal<btc> = ((decimal) num / 100000000m) * 1.0m<btc>
    let toSatoshi (num : decimal<btc>) : int<sat> = Decimal.ToInt32((decimal) num * 100000000m) * 1<sat>
