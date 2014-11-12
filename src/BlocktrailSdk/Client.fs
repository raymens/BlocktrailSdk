namespace BlocktrailSdk

module Client =
    open BlocktrailSdk.Models
    open BlocktrailSdk.Helpers
    open System

    /// <summary>
    /// Get a specific block
    /// </summary>
    let public GetBlock block = 
        let response = getBlockResponse block
        convertToObject<BlockRequest> response

    /// <summary>
    /// Get the latest block
    /// </summary>
    let public GetLastBlock() = 
        let response = getLastBlockResponse()
        convertToObject<BlockRequest> response

    /// <summary>
    /// Get all blocks
    /// </summary>
    let public GetAllBlocks(page : int, limit : int) = 
        let sort_dir = "asc"

        let response = getAllBlocksResponse page limit sort_dir
        let convertedResponse = convertToObject<Paging<BlockRequest>> response

        // create altered function because PagingResponse requires too many args
        let altered = fun x y z b -> getAllBlocksResponse y z b

        new PagingResponse<BlockRequest>(String.Empty, sort_dir, convertedResponse, altered)

    /// <summary>
    /// Get a specific transaction
    /// </summary>
    let public GetTransaction trans =
        let response = getTransactionResponse trans
        convertToObject<TransactionRequest> response   