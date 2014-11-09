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
/// Client to explore the blockchain using the Blocktrail Data API
/// https://www.blocktrail.com/api/docs#api_data
/// </summary>
type BlocktrailDataClient(apiKey : string) = 
    member val APIKey = apiKey with get, set

    /// <summary>
    /// Do a HTTP request using GET that includes the authorization for the Blocktrail API
    /// </summary>
    member private x.Request url args = requestGet url x.APIKey args
    
    /// <summary>
    /// Get all the blocks
    /// </summary>
    member x.AllBlocks page limit sort_dir = 
        convertToObject<Paging<Block>> (x.Request "all-blocks" [ "page", string page
                                                                 "limit", string limit
                                                                 "sort_dir", sort_dir ])
    
    /// <summary>
    /// Get a specific transaction
    /// </summary>
    member x.GetTransaction transaction = 
        convertToObject<Transaction> (x.Request (sprintf "transaction/%s" transaction) [])

    /// <summary>
    /// Get the latest block
    /// </summary>
    member x.LatestBlock = convertToObject<Block> (x.Request ("block/latest") [])

    /// <summary>
    /// Get the transactions of a specific block
    /// </summary>
    member x.BlockTransactions block = 
        convertToObject<Paging<Transaction>> (x.Request (sprintf "block/%s/transactions" block) [])

    /// <summary>
    /// Get a specific block
    /// </summary>
    member x.GetBlock block = convertToObject<Block> (x.Request (sprintf "block/%s" block) [])

    /// <summary>
    /// Get the unspent outputs of a specific address
    /// </summary>
    member x.AddressUnspentOutputs addr = 
        convertToObject<Paging<Output>> (x.Request (sprintf "address/%s/unspent-outputs" addr) [])

    /// <summary>
    /// Get the uncomfirmed transactions of a specific address
    /// </summary>
    member x.AddressUnconfirmedTransactions addr = 
        convertToObject<Paging<Transaction>> (x.Request (sprintf "address/%s/unconfirmed-transactions" addr) [])
    
    /// <summary>
    /// Get the transactions of a specific address
    /// </summary>
    member x.AddressTransactions addr = 
        convertToObject<Paging<Transaction>> (x.Request (sprintf "address/%s/transactions" addr) [])

    /// <summary>
    /// Get a specific address
    /// </summary>
    member x.Address addr = convertToObject<Address> (x.Request (sprintf "address/%s" addr) [])

module BlocktrailClient = 
    
    [<Measure>]
    type sat
    
    [<Measure>]
    type btc
    
    let toBTC (num : int<sat>) : decimal<btc> = ((decimal) num / 100000000m) * 1.0m<btc>
    let toSatoshi (num : decimal<btc>) : int<sat> = Decimal.ToInt32((decimal) num * 100000000m) * 1<sat>
