module CustomBip32

open Bitcoin.BitcoinUtilities
open System

let generateEntropy (size : int) : byte array = 
    if size % 32 <> 0 then failwith "Entropy must be in a multiple of 32"
    Bitcoin.BitcoinUtilities.Utilities.GetRandomBytes(size)

let HMACSHA512 (key : string) (data : byte array) : byte array =
    let hashKey = System.Text.Encoding.Unicode.GetBytes(key)
    NBitcoin.Crypto.Hashes.HMACSHA512(hashKey, data);

let master_key() =
    let key = "Bitcoin seed"
    let seed = generateEntropy 512
    let I = HMACSHA512 key seed

    let Il = Array.sub I 0 32
    let Ir = Array.sub I 32 32

    
    printfn "\nS  %A" (Bitcoin.BitcoinUtilities.Utilities.BytesToHexString(seed))
    printfn "I  %A" (Bitcoin.BitcoinUtilities.Utilities.BytesToHexString(I))
    printfn "Il %A" (Bitcoin.BitcoinUtilities.Utilities.BytesToHexString(Il))
    printfn "Ir %A" (Bitcoin.BitcoinUtilities.Utilities.BytesToHexString(Ir))

    0

master_key() |> ignore