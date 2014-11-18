
[<EntryPoint>]
let main argv = 
    
    let apiKey = "INSERT_YOUR_API_KEY_HERE";

    BlocktrailSdk.Config.ApiKey <- apiKey
    let block = BlocktrailSdk.Client.GetBlock "00000000000000000b0d6b7a84dd90137757db3efbee2c4a226a802ee7be8947"

    let mutable transactions = block.GetTransactions(1, 20, "asc")
    let mutable i = 0

    for trans in transactions do
        i <- i + 1
        printfn "[%i/%i] %s" i transactions.Total trans.Hash


    while transactions.NextPageAvailable() do
        transactions <- transactions.NextPage()

        for trans in transactions do
            i <- i + 1
            printfn "[%i/%i] %s" i transactions.Total trans.Hash
            

        printfn "Press key to fetch next row of transactions"
        System.Console.ReadLine() |> ignore


    printfn "No more transactions available"

    System.Console.ReadLine() |> ignore

    0 // return an integer exit code
