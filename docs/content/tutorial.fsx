(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use 
// it to define helpers that you do not want to show in the documentation.
#I "../../bin"

(**
Introducing your project
========================

Say more

*)
#r "BlocktrailSdk.dll"
open BlocktrailSdk

let client = new BTClient()

let transaction = client.GetTransaction "c326105f7fbfa4e8fe971569ef8858f47ee7e4aa5e8e7c458be8002be3d86aad"

printfn "%s" transaction.block_hash
printfn "%i" transaction.confirmations
(**
Some more info
*)
