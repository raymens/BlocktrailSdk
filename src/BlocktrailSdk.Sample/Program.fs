open BlocktrailSdk

[<EntryPoint>]
let main argv = 
    
    let apiKey = "INSERT_YOUR_API_KEY_HERE";

    let client = new BlocktrailDataClient(apiKey)


    let transaction = client.GetTransaction "c326105f7fbfa4e8fe971569ef8858f47ee7e4aa5e8e7c458be8002be3d86aad"
    printfn "%s" transaction.block_hash
    printfn "%i" transaction.confirmations

    let block = client.GetBlock transaction.block_hash
    let transactions = client.BlockTransactions block.hash
    
    printfn "%i transactions in block %s" transactions.total block.hash


    System.Console.ReadLine() |> ignore

    0 // return an integer exit code
