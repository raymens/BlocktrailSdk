namespace NewImplementation

open BlocktrailSdk.Models
open System

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

/// [omit]
module internal BTClientBase =
    
    let mutable apiKey = ""

    let request url args = requestGet url apiKey args

    let getLastBlockResponse() = 
        let url = sprintf "block/latest"
        request url []

    let getBlockResponse block = 
        let url = sprintf "block/%s" block
        request url []

    let getBlockTransactionsResponse block page limit sort_dir = 
        let url = sprintf "block/%s/transactions" block
        request url [ "page", string page
                      "limit", string limit
                      "sort_dir", sort_dir ]

open BTClientBase
open System.Collections.Generic
open System.Collections.ObjectModel

type PagingResponse<'T>(id, sortDir, data : Paging<'T>, reqfunc) =
    inherit ReadOnlyCollection<'T>(data.data)

    member val private Id = id with get
    member val private Data = data.data with get
    member val private Raw = data with get
    member val private SortDir = sortDir with get

    member val Limit = data.per_page with get
    member val Total = data.total with get
    member val Page = data.current_page with get

    member x.PageAvailable page =
        let result = (int64 x.Limit * int64 (page - 1)) < x.Total && x.Total > (int64 x.Limit * int64 page)
        
        printfn "(%i * %i) < %i > (%i * %i) = %b" x.Limit (page - 1) x.Total x.Limit page result

        result


    member x.NextPageAvailable() =
        x.PageAvailable (x.Page + 1)

    member x.PrevPageAvailable() =
        x.PageAvailable (x.Page - 1)

    member x.GetPage page : PagingResponse<'T> = 
        printfn "Requesting page %i" page

        if x.PageAvailable page = false then raise (ArgumentOutOfRangeException("page", "Requested page is out of range"))

        let response = reqfunc x.Id page x.Raw.per_page x.SortDir
        let convertedResponse = convertToObject<Paging<'T>> response

        if Array.isEmpty convertedResponse.data then raise (NullReferenceException("No data returned."))

        new PagingResponse<'T>(x.Id, x.SortDir, convertedResponse, reqfunc)

    member x.PrevPage() : PagingResponse<'T> = 
        x.GetPage (x.Raw.current_page - 1)

    member x.NextPage() : PagingResponse<'T> = 
        x.GetPage (x.Raw.current_page + 1)


type TransactionRequest() =
    member val hash = "" with get, set
    member val first_seen_at = "" with get, set
    member val last_seen_at = "" with get, set
    member val block_height = 0 with get, set
    member val block_time = "" with get, set
    member val block_hash = "" with get, set
    member val confirmations = 0 with get, set
    member val is_coinbase = false with get, set
    member val estimated_value = 0L with get, set
    member val total_input_value = 0L with get, set
    member val total_output_value = 0L with get, set
    member val total_fee = 0 with get, set
    member val estimated_change = Nullable.op_Implicit 0L : Nullable<int64> with get, set
    member val estimated_change_address = "" with get, set
    member val high_priority = false with get, set
    member val enough_fee = false with get, set
    member val contains_dust = false with get, set

type BlockRequest() =
    member val hash = "" with get, set
    member val height = 0 with get, set
    member val block_time = "" with get, set
    member val difficulty = 0L with get, set
    member val merkleroot = "" with get, set
    member val is_orphan = false with get, set
    member val prev_block = "" with get, set
    member val next_block = "" with get, set
    member val byte_size = 0 with get, set
    member val confirmations = 0 with get, set
    member val transactions = 0 with get, set
    member val value = 0L with get, set
    member val miningpool_name = "" with get, set
    member val miningpool_url = "" with get, set
    member val miningpool_slug = "" with get, set


    /// <summary>
    /// Get transactions of this block (paginated)
    /// </summary>
    member x.Transactions(?page0 : int, ?limit0 : int, ?sort_dir0 : string) =
        let page = defaultArg page0 0
        let limit = defaultArg limit0 10
        let sort_dir = defaultArg sort_dir0 "asc" 

        let response = BTClientBase.getBlockTransactionsResponse x.hash page limit sort_dir

        let convertedResponse = convertToObject<Paging<Transaction>> response

        new PagingResponse<Transaction>(x.hash, sort_dir, convertedResponse, BTClientBase.getBlockTransactionsResponse)

    /// <summary>
    /// Get transactions of this block (paginated) (C# interop)
    /// </summary>
    member x.Transactions(page : int) =
        x.Transactions(page0 = page)

    /// <summary>
    /// Get transactions of this block (paginated) (C# interop)
    /// </summary>
    member x.Transactions(page : int, limit : int) =
        x.Transactions(page0 = page, limit0 = limit)

    /// <summary>
    /// Get the next block
    /// </summary>
    member x.NextBlock() = 
        let response = BTClientBase.getBlockResponse x.next_block
        convertToObject<BlockRequest> response

    /// <summary>
    /// Get the previous block
    /// </summary>
    member x.PrevBlock() =
        let response = BTClientBase.getBlockResponse x.prev_block
        convertToObject<BlockRequest> response

type BlocktrailDataClient(apiKey : string) = 
    //HACK -- what's the best way to have access to the api key from everywhere?
    do
        BTClientBase.apiKey <- apiKey

    member val APIKey = apiKey with get, set

    /// <summary>
    /// Get a specific block
    /// </summary>
    member public x.GetBlock block = 
        let response = BTClientBase.getBlockResponse block
        convertToObject<BlockRequest> response

    /// <summary>
    /// Get the latest block
    /// </summary>
    member public x.GetLastBlock() = 
        let response = BTClientBase.getLastBlockResponse()
        convertToObject<BlockRequest> response