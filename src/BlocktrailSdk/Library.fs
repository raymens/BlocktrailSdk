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

type Response<'T>(id, sortDir, data : Paging<'T>, reqfunc) =
    
    member val private Id = id with get
    member val Data = data.data with get
    member val private Raw = data with get
    member val private SortDir = sortDir with get
    member val Limit = data.per_page with get
    member val Total = data.total with get
    member val Page = data.current_page with get

    member x.PrevPage() : Response<'T> = reqfunc id (x.Raw.current_page - 1) x.Raw.per_page x.SortDir

    member x.NextPage() : Response<'T> = reqfunc id (x.Raw.current_page + 1) x.Raw.per_page x.SortDir

    member x.GetPage page : Response<'T> = reqfunc id page x.Raw.per_page x.SortDir

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
        let url = "all-blocks"
        let response = convertToObject<Paging<Block>> (x.Request "all-blocks" [ "page", string page
                                                                                "limit", string limit
                                                                                "sort_dir", sort_dir ])
        
        new Response<Block>("", sort_dir, response, fun q w e r -> x.AllBlocks w e r)

    
    /// <summary>
    /// Get a specific transaction
    /// </summary>
    member x.Transaction transaction = 
        convertToObject<Transaction> (x.Request (sprintf "transaction/%s" transaction) [])
    
    /// <summary>
    /// Get the latest block
    /// </summary>
    member x.LatestBlock = convertToObject<Block> (x.Request ("block/latest") [])
    
    /// <summary>
    /// Get the transactions of a specific block
    /// </summary>
    member x.BlockTransactions block page limit sort_dir = 
        let url = sprintf "block/%s/transactions" block
        let response = convertToObject<Paging<Transaction>> 
                                            (x.Request (url) [ "page", string page
                                                               "limit", string limit
                                                               "sort_dir", sort_dir ])
        new Response<Transaction>(block, sort_dir, response, x.BlockTransactions)
        
    
    /// <summary>
    /// Get a specific block
    /// </summary>
    member x.Block block = convertToObject<Block> (x.Request (sprintf "block/%s" block) [])
    
    /// <summary>
    /// Get the unspent outputs of a specific address
    /// </summary>
    member x.AddressUnspentOutputs addr page limit sort_dir = 
        let url = sprintf "address/%s/unspent-outputs" addr
        let response = convertToObject<Paging<Output>> (x.Request (url) [ "page", string page
                                                                          "limit", string limit
                                                                          "sort_dir", sort_dir ])
        new Response<Output>(addr, sort_dir, response, x.AddressUnspentOutputs)

    
    /// <summary>
    /// Get the uncomfirmed transactions of a specific address
    /// </summary>
    member x.AddressUnconfirmedTransactions addr page limit sort_dir = 
        let url = sprintf "address/%s/unconfirmed-transactions" addr
        let response = convertToObject<Paging<Transaction>> 
                        (x.Request (url) [ "page", string page
                                           "limit", string limit
                                           "sort_dir", sort_dir ])
        new Response<Transaction>(addr, sort_dir, response, x.AddressUnconfirmedTransactions)
    
    /// <summary>
    /// Get the transactions of a specific address
    /// </summary>
    member x.AddressTransactions addr page limit sort_dir = 
        let url = sprintf "address/%s/transactions" addr
        let response = convertToObject<Paging<Transaction>> 
                        (x.Request (url) [ "page", string page
                                           "limit", string limit
                                           "sort_dir", sort_dir ])
        new Response<Transaction>(addr, sort_dir, response, x.AddressTransactions)
        
    
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
