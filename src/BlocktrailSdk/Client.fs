namespace BlocktrailSdk

open BlocktrailSdk.Models
open BlocktrailSdk.Helpers
open System
open System.Runtime.InteropServices
open Newtonsoft.Json
open FSharp.Data

/// <summary>
/// Contains methods to retreive data from the blockchain using the Blocktrail API.
/// </summary>
type BlocktrailSdkClient(apiKey, apiSecret) = 
    
    let request url args = 
        let resp = requestGet url apiKey args
        System.Diagnostics.Trace.WriteLine(resp)
        resp
    
    let paginatedArgs page limit sort_dir = 
        [ "page", string page
          "limit", string limit
          "sort_dir", sort_dir ]
    
    let getLastBlockResponse() = 
        let url = sprintf "block/latest"
        request url []
    
    let getAllBlocksResponse (page, limit, sort_dir) = 
        let url = sprintf "all-blocks"
        request url (paginatedArgs page limit sort_dir)
    
    let getBlockResponse block = 
        let url = sprintf "block/%s" block
        request url []
    
    let getBlockTransactionsResponse block (page, limit, sort_dir) = 
        let url = sprintf "block/%s/transactions" block
        request url (paginatedArgs page limit sort_dir)
    
    let getAddressTransactionsResponse addr (page, limit, sort_dir) = 
        let url = sprintf "address/%s/transactions" addr
        request url (paginatedArgs page limit sort_dir)
    
    let getAddressUncomfirmedTransactionsResponse addr (page, limit, sort_dir) = 
        let url = sprintf "address/%s/transactions" addr
        request url (paginatedArgs page limit sort_dir)
    
    let getAddressUnspentOutputsResponse addr (page, limit, sort_dir) = 
        let url = sprintf "address/%s/unspent-outputs" addr
        request url (paginatedArgs page limit sort_dir)
    
    let getTransactionResponse trans = 
        let url = sprintf "transaction/%s" trans
        request url []
    
    let getAddressResponse addr = 
        let url = sprintf "address/%s" addr
        request url []
    
    let check page limit sort_dir = 
        ((if page <= 0 then 1
          else page), 
         (if limit <= 0 then 20
          else limit), 
         (if sort_dir = null then "asc"
          else sort_dir))
    
    let third (_, _, c) = c
    member val ApiKey = apiKey
    
    /// <summary>
    /// Retrieve a specific block
    /// </summary>
    member x.GetBlock block = 
        let response = getBlockResponse block
        convertToObject<Block> response
    
    /// <summary>
    /// Retrieve the latest block
    /// </summary>
    member x.GetLastBlock() = 
        let response = getLastBlockResponse()
        convertToObject<Block> response
    
    /// <summary>
    /// Get all blocks
    /// </summary>
    member x.GetAllBlocks([<Optional; DefaultParameterValue(0)>] page, [<Optional; DefaultParameterValue(0)>] limit, 
                          [<Optional; DefaultParameterValue(null)>] sort_dir) = 
        let checkedArgs = check page limit sort_dir
        let response = getAllBlocksResponse checkedArgs
        let convertedResponse = convertToObject<Paging<Block>> response
        // create altered function because PagingResponse requires too many args
        let altered = fun _ a -> getAllBlocksResponse a
        new PagingResponse<Block>(String.Empty, (third checkedArgs), convertedResponse, altered)
    
    /// <summary>
    /// Retrieve a specific address
    /// </summary>
    member x.GetTransaction trans = 
        let response = getTransactionResponse trans
        convertToObject<Transaction> response
    
    /// <summary>
    /// Will retrieve a specific address containing basic info such as the current balance, 
    /// the total amount received, and the number of transactions made. 
    /// </summary>
    member x.GetAddress addr = 
        let response = getAddressResponse addr
        convertToObject<Address> response
    
    /// <summary>
    /// Get transactions of a specific block (paginated)
    /// </summary>
    member x.GetBlockTransactions(block : string, [<Optional; DefaultParameterValue(1)>] page : int, 
                                  [<Optional; DefaultParameterValue(0)>] limit, 
                                  [<Optional; DefaultParameterValue(null)>] sort_dir) = 
        let checkedArgs = check page limit sort_dir
        let response = getBlockTransactionsResponse block checkedArgs
        let convertedResponse = convertToObject<Paging<RelatedTransaction>> response
        new PagingResponse<RelatedTransaction>(block, (third checkedArgs), convertedResponse, 
                                               getBlockTransactionsResponse)
    
    /// <summary>
    /// Retrieve transactions of a specific address (paginated)
    /// </summary>
    member x.GetAddressTransactions(address : string, [<Optional; DefaultParameterValue(1)>] page : int, 
                                    [<Optional; DefaultParameterValue(0)>] limit, 
                                    [<Optional; DefaultParameterValue(null)>] sort_dir) = 
        let checkedArgs = check page limit sort_dir
        let response = getAddressTransactionsResponse address checkedArgs
        let convertedResponse = convertToObject<Paging<RelatedTransaction>> response
        new PagingResponse<RelatedTransaction>(address, (third checkedArgs), convertedResponse, 
                                               getAddressTransactionsResponse)
    
    /// <summary>
    /// Retrieve uncomfirmed transactions of a specific address (paginated)
    /// </summary>
    member x.GetAddressUnconfirmedTransactions(address : string, [<Optional; DefaultParameterValue(1)>] page : int, 
                                               [<Optional; DefaultParameterValue(0)>] limit, 
                                               [<Optional; DefaultParameterValue(null)>] sort_dir) = 
        let checkedArgs = check page limit sort_dir
        let response = getAddressUncomfirmedTransactionsResponse address checkedArgs
        let convertedResponse = convertToObject<Paging<RelatedTransaction>> response
        new PagingResponse<RelatedTransaction>(address, (third checkedArgs), convertedResponse, 
                                               getAddressUncomfirmedTransactionsResponse)
    
    /// <summary>
    /// Retrieve unspent outputs of this address (paginated)
    /// </summary>
    member x.GetAddressUnspentOutputs(address : string, [<Optional; DefaultParameterValue(1)>] page : int, 
                                      [<Optional; DefaultParameterValue(0)>] limit, 
                                      [<Optional; DefaultParameterValue(null)>] sort_dir) = 
        let checkedArgs = check page limit sort_dir
        let response = getAddressUnspentOutputsResponse address checkedArgs
        let convertedResponse = convertToObject<Paging<TransactionUnspentOutput>> response
        new PagingResponse<TransactionUnspentOutput>(address, (third checkedArgs), convertedResponse, 
                                                     getAddressUnspentOutputsResponse)

    /// Create a new wallet
    member x.CreateNewWallet(identifier : string, password : string, key_index : int) =
        match Wallet.createNewWallet apiKey apiSecret identifier password key_index with
        | Some(w) -> w
        | None -> failwith "Error during creation of wallet"

    /// Delete an existing wallet
    member x.DeleteWallet(wallet : Wallet) =
        Wallet.deleteWallet x.ApiKey apiSecret wallet