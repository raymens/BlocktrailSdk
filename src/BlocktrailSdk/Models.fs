namespace BlocktrailSdk

module Models =

    open System

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

    type Transaction = 
        { hash : string
          first_seen_at : string
          last_seen_at : string
          block_height : int
          block_time : string
          block_hash : string
          confirmations : int
          is_coinbase : bool
          estimated_value : int64
          total_input_value : int64
          total_output_value : int64
          total_fee : int
          estimated_change : Nullable<int64>
          estimated_change_address : string
          high_priority : bool
          enough_fee : bool
          contains_dust : bool
          inputs : TransactionInput array
          outputs : TransactionOutput array }

    type Address = 
        { address : string
          hash160 : string
          balance : int
          received : int
          sent : int
          unconfirmed_received : int
          unconfirmed_sent : int
          unconfirmed_transactions : int
          total_transactions_in : int
          total_transactions_out : int
          category : string
          tag : string }

    type Block = 
        { hash : string
          height : int
          block_time : string
          difficulty : int64
          merkleroot : string
          is_orphan : bool
          prev_block : string
          next_block : string
          byte_size : int
          confirmations : int
          transactions : int
          value : int64
          miningpool_name : string
          miningpool_url : string
          miningpool_slug : string }

    type Output = 
        { hash : string
          height : int
          block_time : string
          difficulty : int64
          merkleroot : string
          is_orphan : bool
          byte_size : int
          confirmations : int
          transactions : int
          value : int64
          miningpool_name : string
          miningpool_url : string
          miningpool_slug : string }

    type Paging<'T> = 
        { current_page : int
          per_page : int
          total : int64
          data : array<'T> }

    type Request<'T> = 
        { url : string
          page : int
          limit : int
          sort_dir : string }


    open BlocktrailSdk.Helpers
    open System.Collections.Generic
    open System.Collections.ObjectModel
    open Newtonsoft.Json

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

        [<JsonProperty("block_hash")>]
        member val BlockHash = "" with get, set
        member val confirmations = 0 with get, set
        member val is_coinbase = false with get, set
        member val estimated_value = 0L with get, set
        member val total_input_value = 0L with get, set
        member val total_output_value = 0L with get, set
        member val total_fee = 0 with get, set
        member val estimated_change = Nullable.op_Implicit 0L : Nullable<int64> with get, set
        member val estimated_change_address = "" with get, set
        member val high_priority = Nullable.op_Implicit false with get, set
        member val enough_fee = Nullable.op_Implicit false with get, set
        member val contains_dust = Nullable.op_Implicit false with get, set
        member val inputs = null : TransactionInput array with get, set
        member val outputs = null : TransactionOutput array with get, set

    type BlockTransactionRequest() =
        member val hash = "" with get, set
        member val time = "" with get, set
        member val confirmations = 0 with get, set
        member val is_coinbase = false with get, set
        member val estimated_value = 0L with get, set
        member val total_input_value = 0L with get, set
        member val total_output_value = 0L with get, set
        member val total_fee = 0 with get, set
        member val estimated_change = Nullable.op_Implicit 0L : Nullable<int64> with get, set
        member val estimated_change_address = "" with get, set
        member val inputs = null : TransactionInput array with get, set
        member val outputs = null : TransactionOutput array with get, set

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

            //let response = BTClientBase.getBlockTransactionsResponse x.hash page limit sort_dir
            let response = getBlockTransactionsResponse x.hash page limit sort_dir

            let convertedResponse = convertToObject<Paging<BlockTransactionRequest>> response

            new PagingResponse<BlockTransactionRequest>(x.hash, sort_dir, convertedResponse, getBlockTransactionsResponse)

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
            //let response = BTClientBase.getBlockResponse x.next_block
            let response = getBlockResponse x.next_block
            convertToObject<BlockRequest> response

        /// <summary>
        /// Get the previous block
        /// </summary>
        member x.PrevBlock() =
            //let response = BTClientBase.getBlockResponse x.prev_block
            let response = getBlockResponse x.prev_block
            convertToObject<BlockRequest> response


    type TransactionRequest with 
        member x.Block =
            let response = getBlockResponse x.BlockHash
            convertToObject<BlockRequest> response
