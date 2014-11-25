(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use 
// it to define helpers that you do not want to show in the documentation.
#I "../../bin"

(**
Introduction
========================

Blocktrail does not provide a .NET implementation of their SDK so I decided to build one on my own (feel free to contribute!).
This .NET SDK is written in F# and tries to keep it as C# friendly as possible, keep this in mind when contributing as it is a vital requirement.
The client implements some of the same methods as the official ones. The idea was to differ in some ways.

E.G.: implementing the request for `unconfirmed transactions of an address` as a method on the `Address` object instead of an API call from the client module.

We start the tutorial by setting up the API. The first thing to notice is that it's built in F# modules which are basically statis classes. 
This might change in the future, but at this moment it is **not** possible to use multiple API keys.

If you don't have a Blocktrail API key yet go get one at [Blocktrail.com][apikey], it's free!

  [apikey]: https://www.blocktrail.com/signup

Basics
======

Copy the snippet below and replace the APIKey placeholder with your api key you requested.
*)
#r "BlocktrailSdk.dll"
open BlocktrailSdk
open BlocktrailSdk.Models

BlocktrailSdk.Config.ApiKey <- "INSERT_YOUR_API_KEY";

let transaction = BlocktrailSdk.Client.GetTransaction "c326105f7fbfa4e8fe971569ef8858f47ee7e4aa5e8e7c458be8002be3d86aad"
(**
Relational properties and methods
=================================

This will retrieve the the transaction of that specific hash.
After retrieving it you can print and use it's values as shown in the next snippet.
*)
printfn "Hash: %s" transaction.Hash
printfn "Confirmations: %i" transaction.Confirmations
(**
Or do more advanced stuff and retrieve the Block where this particular transaction resides in.
*)
let block = transaction.Block
printfn "Block height: %i" block.Height
printfn "Block mined by: %s" block.MiningpoolName
(**
Pagination
==========

And why stop there... let's retrieve the next block and a page of it's transactions.
Remember that responses that contains lists are often paginated. That means the reponse
is sliced into the size you request (or the default size of 20). But don't worry, retrieving
the next page is simple!
*)
let nextBlock = BlocktrailSdk.Client.GetBlock block.NextBlock

let transactions = nextBlock.GetTransactions(1, 20, "asc")

// Print out the hashes
transactions |> Seq.iteri (fun i x -> printfn "Transaction[%i]: %s" i x.Hash)

let nextLoadOfTransactions = transactions.NextPage()

// Print out the hashes of the 2nd page of transactions
nextLoadOfTransactions |> Seq.iteri (fun i x -> printfn "Transaction[%i]: %s" i x.Hash)
(**
This was the starting point of using the SDK, there is more to be found within this project.
Go explore the blockchain on your own and be sure to file any issues or suggestions.
*)