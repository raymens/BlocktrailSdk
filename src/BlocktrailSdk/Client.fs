namespace BlocktrailSdk

module Client =
    open BlocktrailSdk.Models
    open BlocktrailSdk.Helpers
    open System
    open System.Runtime.InteropServices

    /// <summary>
    /// Get a specific block
    /// </summary>
    let public GetBlock block = 
        let response = getBlockResponse block
        convertToObject<Block> response

    /// <summary>
    /// Get the latest block
    /// </summary>
    let public GetLastBlock() = 
        let response = getLastBlockResponse()
        convertToObject<Block> response

    /// <summary>
    /// Get all blocks
    /// </summary>
    
    let public GetAllBlocks([<Optional;DefaultParameterValue(1)>] page : int, [<Optional;DefaultParameterValue(20)>]  limit : int) = 
        let sort_dir = "asc"

        let response = getAllBlocksResponse page limit sort_dir
        let convertedResponse = convertToObject<Paging<Block>> response

        // create altered function because PagingResponse requires too many args
        let altered = fun x y z b -> getAllBlocksResponse y z b

        new PagingResponse<Block>(String.Empty, sort_dir, convertedResponse, altered)

    /// <summary>
    /// Get a specific transaction
    /// </summary>
    let public GetTransaction trans =
        let response = getTransactionResponse trans
        convertToObject<Transaction> response   

    /// <summary>
    /// Get a specific address
    /// </summary>
    let public GetAddress addr =
        let response = getAddressResponse addr
        convertToObject<Address> response  