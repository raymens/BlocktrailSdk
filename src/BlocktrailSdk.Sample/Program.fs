open BlocktrailSdk

[<EntryPoint>]
let main argv = 
    
    let apiKey = "INSERT_YOUR_API_KEY_HERE";

    let client = new BlocktrailDataClient(apiKey)


    let transaction = client.Transaction "c326105f7fbfa4e8fe971569ef8858f47ee7e4aa5e8e7c458be8002be3d86aad"
    printfn "%s" transaction.block_hash
    printfn "%i" transaction.confirmations

    let block = client.Block transaction.block_hash
    let transactions = client.BlockTransactions block.hash 0 100 "asc"
    
    printfn "%i transactions in block %s" transactions.Total block.hash
    printfn "Current page: %i. input value: %i" transactions.Page transactions.Data.[0].total_input_value

    let nextTransactions = transactions.NextPage()

    printfn "Current page: %i. input value: %i" nextTransactions.Page nextTransactions.Data.[0].total_input_value

    let allblocks = client.AllBlocks 0 100 "asc"
    printfn "block %s is the first on page %i"  allblocks.Data.[0].hash allblocks.Page

    let nextrow = allblocks.NextPage()

    printfn "block %s is the first on page %i"  nextrow.Data.[0].hash nextrow.Page


    System.Console.ReadLine() |> ignore

    0 // return an integer exit code
