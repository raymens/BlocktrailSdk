namespace BlocktrailSdk.Models


open System
open BlocktrailSdk.Helpers
open System.Collections.Generic
open System.Collections.ObjectModel
open Newtonsoft.Json
open FSharp.Data

type TransactionInput = 
    { index : int
      output_hash : string
      output_index : int
      value : int64
      address : string
      ``type`` : string
      multisig : Object
      script_signature : string }

type TransactionOutput = 
    { index : int
      value : int64
      address : string
      ``type`` : string
      multisig : Object
      script : string
      script_hex : string
      spent_hash : string
      spent_index : int }

type Output = 
    { [<JsonProperty("hash")>]
      Hash : string
      [<JsonProperty("height")>]
      Height : int
      [<JsonProperty("block_time")>]
      BlockTime : string
      [<JsonProperty("difficulty")>]
      Difficulty : int64
      [<JsonProperty("merkleroot")>]
      Merkleroot : string
      [<JsonProperty("is_orphan")>]
      IsOrphan : bool
      [<JsonProperty("byte_size")>]
      ByteSize : int
      [<JsonProperty("confirmations")>]
      Confirmations : int
      [<JsonProperty("transactions")>]
      Transactions : int
      [<JsonProperty("value")>]
      Value : int64
      [<JsonProperty("miningpool_name")>]
      MiningpoolName : string
      [<JsonProperty("miningpool_url")>]
      MiningpoolUrl : string
      [<JsonProperty("miningpool_slug")>]
      MiningpoolSlug : string }

type Paging<'T> = 
    { [<JsonProperty("current_page")>]
      CurrentPage : int
      [<JsonProperty("per_page")>]
      PerPage : int
      [<JsonProperty("total")>]
      Total : int64
      [<JsonProperty("data")>]
      Data : array<'T> }


type PagingResponse<'T>(id, sortDir, data : Paging<'T>, reqfunc) =
    inherit ReadOnlyCollection<'T>(data.Data)

    member val private Id = id with get
    member val private Raw = data with get
    member val private SortDir = sortDir with get

    member val Limit = data.PerPage with get
    member val Total = data.Total with get
    member val Page = data.CurrentPage with get

    member x.PageAvailable page =
        let modulusResult = int (x.Total % int64 x.Limit)

        if modulusResult > 1 then
            let totalPages = int (x.Total / int64 x.Limit) + 1

            page <= totalPages
        else
            true

    member x.NextPageAvailable() =
        x.PageAvailable (x.Page + 1)

    member x.PrevPageAvailable() =
        x.PageAvailable (x.Page - 1)

    member x.GetPage page : PagingResponse<'T> = 
        printfn "Requesting page %i" page

        if x.PageAvailable page = false then raise (ArgumentOutOfRangeException("page", "Requested page is out of range"))

        let response = reqfunc x.Id page x.Raw.PerPage x.SortDir
        let convertedResponse = convertToObject<Paging<'T>> response

        if Array.isEmpty convertedResponse.Data then raise (NullReferenceException("No data returned."))

        new PagingResponse<'T>(x.Id, x.SortDir, convertedResponse, reqfunc)

    member x.PrevPage() : PagingResponse<'T> = 
        x.GetPage (x.Raw.CurrentPage - 1)

    member x.NextPage() : PagingResponse<'T> = 
        x.GetPage (x.Raw.CurrentPage + 1)


type Address() = 
    [<JsonProperty("address")>]
    member val Address = "" with get, set
    [<JsonProperty("hash160")>]
    member val Hash160 = "" with get, set
    [<JsonProperty("balance")>]
    member val Balance = 0L with get, set
    [<JsonProperty("received")>]
    member val Received = 0L with get, set
    [<JsonProperty("sent")>]
    member val Sent = 0L with get, set
    [<JsonProperty("unconfirmed_received")>]
    member val UnconfirmedReceived = 0 with get, set
    [<JsonProperty("unconfirmed_sent")>]
    member val UnconfirmedSent = 0 with get, set
    [<JsonProperty("unconfirmed_transactions")>]
    member val UnconfirmedTransactions = 0 with get, set
    [<JsonProperty("total_transactions_in")>]
    member val TotalTransactionsIn = 0 with get, set
    [<JsonProperty("total_transactions_out")>]
    member val TotalTransactionsOut = 0 with get, set
    [<JsonProperty("category")>]
    member val Category = "" with get, set
    [<JsonProperty("tag")>]
    member val Tag = "" with get, set

type Transaction() =
    [<JsonProperty("hash")>]
    member val Hash = "" with get, set
    [<JsonProperty("first_seen_at")>]
    member val FirstSeenAt = "" with get, set
    [<JsonProperty("last_seen_at")>]
    member val LastSeenAt = "" with get, set
    [<JsonProperty("block_height")>]
    member val BlockHeight = 0 with get, set
    [<JsonProperty("block_time")>]
    member val BlockTime = "" with get, set
    [<JsonProperty("block_hash")>]
    member val BlockHash = "" with get, set
    [<JsonProperty("confirmations")>]
    member val Confirmations = 0 with get, set
    [<JsonProperty("is_coinbase")>]
    member val IsCoinbase = false with get, set
    [<JsonProperty("estimated_value")>]
    member val EstimatedValue = 0L with get, set
    [<JsonProperty("total_input_value")>]
    member val TotalInputValue = 0L with get, set
    [<JsonProperty("total_output_value")>]
    member val TotalOutputValue = 0L with get, set
    [<JsonProperty("total_fee")>]
    member val TotalFee = 0 with get, set
    [<JsonProperty("estimated_change")>]
    member val EstimatedChange = Nullable.op_Implicit 0L : Nullable<int64> with get, set
    [<JsonProperty("estimated_change_address")>]
    member val EstimatedChangeAddress = "" with get, set
    [<JsonProperty("high_priority")>]
    member val HighPriority = Nullable.op_Implicit false with get, set
    [<JsonProperty("enough_fee")>]
    member val EnoughFee = Nullable.op_Implicit false with get, set
    [<JsonProperty("contains_dust")>]
    member val ContainsDust = Nullable.op_Implicit false with get, set
    [<JsonProperty("inputs")>]
    member val Inputs = null : TransactionInput array with get, set
    [<JsonProperty("outputs")>]
    member val Outputs = null : TransactionOutput array with get, set

type BlockTransaction() =
    [<JsonProperty("hash")>]
    member val Hash = "" with get, set
    [<JsonProperty("time")>]
    member val Time = "" with get, set
    [<JsonProperty("confirmations")>]
    member val Confirmations = 0 with get, set
    [<JsonProperty("is_coinbase")>]
    member val IsCoinbase = false with get, set
    [<JsonProperty("outputsestimated_value")>]
    member val EstimatedValue = 0L with get, set
    [<JsonProperty("total_input_value")>]
    member val TotalInputValue = 0L with get, set
    [<JsonProperty("total_output_value")>]
    member val TotalOutputValue = 0L with get, set
    [<JsonProperty("total_fee")>]
    member val TotalFee = 0 with get, set
    [<JsonProperty("estimated_change")>]
    member val EstimatedChange = Nullable.op_Implicit 0L : Nullable<int64> with get, set
    [<JsonProperty("estimated_change_address")>]
    member val EstimatedChangeAddress = "" with get, set
    [<JsonProperty("inputs")>]
    member val Inputs = null : TransactionInput array with get, set
    [<JsonProperty("outputs")>]
    member val Outputs = null : TransactionOutput array with get, set

type Block() =
    [<JsonProperty("hash")>]
    member val Hash = "" with get, set
    [<JsonProperty("height")>]
    member val Height = 0 with get, set
    [<JsonProperty("block_time")>]
    member val BlockTime = "" with get, set
    [<JsonProperty("difficulty")>]
    member val Difficulty = 0L with get, set
    [<JsonProperty("merkleroot")>]
    member val Merkleroot = "" with get, set
    [<JsonProperty("is_orphan")>]
    member val IsOrphan = false with get, set
    [<JsonProperty("prev_block")>]
    member val PrevBlock = "" with get, set
    [<JsonProperty("next_block")>]
    member val NextBlock = "" with get, set
    [<JsonProperty("byte_size")>]
    member val ByteSize = 0 with get, set
    [<JsonProperty("confirmations")>]
    member val Confirmations = 0 with get, set
    [<JsonProperty("transactions")>]
    member val Transactions = 0 with get, set
    [<JsonProperty("value")>]
    member val Value = 0L with get, set
    [<JsonProperty("miningpool_name")>]
    member val MiningpoolName = "" with get, set
    [<JsonProperty("miningpool_url")>]
    member val MiningpoolUrl = "" with get, set
    [<JsonProperty("miningpool_slug")>]
    member val MiningpoolSlug = "" with get, set

    /// <summary>
    /// Get transactions of this block (paginated)
    /// </summary>
    member x.GetTransactions(?page0 : int, ?limit0 : int, ?sort_dir0 : string) =
        let page = defaultArg page0 1
        let limit = defaultArg limit0 20
        let sort_dir = defaultArg sort_dir0 "asc" 

        //let response = BTClientBase.getBlockTransactionsResponse x.hash page limit sort_dir
        let response = getBlockTransactionsResponse x.Hash page limit sort_dir

        let convertedResponse = convertToObject<Paging<BlockTransaction>> response

        new PagingResponse<BlockTransaction>(x.Hash, sort_dir, convertedResponse, getBlockTransactionsResponse)

    /// <summary>
    /// Get transactions of this block (paginated) (C# interop)
    /// </summary>
    member x.GetTransactions(page : int) =
        x.GetTransactions(page0 = page)

    /// <summary>
    /// Get transactions of this block (paginated) (C# interop)
    /// </summary>
    member x.GetTransactions(page : int, limit : int) =
        x.GetTransactions(page0 = page, limit0 = limit)

    /// <summary>
    /// Get the next block
    /// </summary>
    member x.GetNextBlock() = 
        //let response = BTClientBase.getBlockResponse x.next_block
        let response = getBlockResponse x.NextBlock
        convertToObject<Block> response

    /// <summary>
    /// Get the previous block
    /// </summary>
    member x.GetPrevBlock() =
        //let response = BTClientBase.getBlockResponse x.prev_block
        let response = getBlockResponse x.PrevBlock
        convertToObject<Block> response


type Transaction with 
    member x.Block =
        let response = getBlockResponse x.BlockHash
        convertToObject<Block> response
