
[<EntryPoint>]
let main argv = 
    
    let apiKey = "INSERT_YOUR_API_KEY_HERE";

    let client = new NewImplementation.BlocktrailDataClient(apiKey);
    let block = client.GetBlock "00000000000000000b0d6b7a84dd90137757db3efbee2c4a226a802ee7be8947"
    //let block = oldblock.NextBlock()
    //let block = oldblock
    

    let mutable transactions = block.Transactions(limit0 = 400)
    let mutable i = 0

    for trans in transactions do
        i <- i + 1
        printfn "[%i/%i] %s" i transactions.Total trans.hash


    while transactions.NextPageAvailable() do
        transactions <- transactions.NextPage()

        for trans in transactions do
            i <- i + 1
            printfn "[%i/%i] %s" i transactions.Total trans.hash

        printfn "Press key to fetch next row of transactions"
        System.Console.ReadLine() |> ignore


    printfn "No more transactions available"

    System.Console.ReadLine() |> ignore

    0 // return an integer exit code
