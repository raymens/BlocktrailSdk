module BlocktrailSdk.Tests

open BlocktrailSdk
open NUnit.Framework

(*
[<Test>]
let ``Test whatever``() =
    let signingString = "(request-target): get /path?query=3\ndate: today"
    let secret = Wallet.convertByteArray2String <| Wallet.readBytes "secret"

    let shahed = Wallet.sign signingString secret
    let signatureSigned = Wallet.base64 shahed

    Assert.AreEqual(signatureSigned, "SFlytCGpsqb/9qYaKCQklGDvwgmrwfIERFnwt+yqPJw=")

    //let signature = (sprintf """keyId="%s",algorithm="%s",headers="%s",signature="%s" """ key "hmac-sha256" "(request-target) date" signatureSigned).TrimEnd()
    



[<Test>]
let ``Test byte conversion``() = 
    let test1_key = "0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b"
    let test1_key_calc = Wallet.hexDecode test1_key
    let res = Wallet.convertByteArray2String test1_key_calc

    Assert.AreEqual(test1_key, res)

[<Test>]
let ``Test vector``() = 
    let test1_key = "0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b"
    let test1_data = "4869205468657265"
    let test1_digest = "b0344c61d8db38535ca8afceaf0bf12b881dc200c9833da726e9376c2e32cff7"

    let test1_key_calc = Wallet.hexDecode test1_key
    let test1_data_calc = Wallet.hexDecode test1_data

    let res = Wallet.calculateHMACSHA256 test1_data_calc test1_key_calc
    let res_string = Wallet.convertByteArray2String res

    Assert.AreEqual(res_string, test1_digest)


(*
[<Test>]
let ``5<btc> returns 500000000<sat>``() = 
    let input = 5m<BlocktrailClient.btc>
    let result = BlocktrailClient.toSatoshi input
    Assert.AreEqual(500000000, result)

[<Test>]
let ``0.000025<btc> returns 2500<sat>``() = 
    let input = 0.000025m<BlocktrailClient.btc>
    let result = BlocktrailClient.toSatoshi input
    Assert.AreEqual(2500, result)

[<Test>]
let ``0.000025<btc> returns 0.000025<btc> reconverted``() = 
    let input = 0.000025m<BlocktrailClient.btc>
    let result1 = BlocktrailClient.toSatoshi input
    let result2 = BlocktrailClient.toBTC result1
    Assert.AreEqual(0.000025m, result2)

[<Test>]
let ``330000<sat> returns 0.0033<btc>``() = 
    let input = 330000<BlocktrailClient.sat>
    let result = BlocktrailClient.toBTC input
    Assert.AreEqual(0.0033m, result)
*)

(*
BlocktrailSdk.Config.ApiKey <- "";


let ``00000000000000000b0d6b7a84dd90137757db3efbee2c4a226a802ee7be8947 has 849 results``() = 
    let specificBlock = BlocktrailSdk.Client.GetBlock("00000000000000000b0d6b7a84dd90137757db3efbee2c4a226a802ee7be8947");
    let transactions = specificBlock.GetTransactions(1, 100, "asc")

    Assert.AreEqual(849, transactions.Total)*)*)