module BlocktrailSdk.Tests

open BlocktrailSdk
open NUnit.Framework
open BlocktrailSdk.Models

(*
[<Test>]
let ``Test whatever``() =
    let signingString = "(request-target): get /path?query=3\ndate: today"
    let secret = Wallet.convertByteArray2String <| Wallet.readBytes "secret"

    let shahed = Wallet.sign signingString secret
    let signatureSigned = Wallet.base64 shahed

    Assert.AreEqual(signatureSigned, "SFlytCGpsqb/9qYaKCQklGDvwgmrwfIERFnwt+yqPJw=")

    //let signature = (sprintf """keyId="%s",algorithm="%s",headers="%s",signature="%s" """ key "hmac-sha256" "(request-target) date" signatureSigned).TrimEnd()
*)



[<Test>]
let ``Test byte conversion``() = 
    let test1_key = "0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b"
    let test1_key_calc = hexDecode test1_key
    let res = convertByteArray2String test1_key_calc

    Assert.AreEqual(test1_key, res)

[<Test>]
let ``Test RFC 4231 HMAC-SHA256 vectors``() = 
    let tests = [("0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b", 
                  "4869205468657265", 
                  "b0344c61d8db38535ca8afceaf0bf12b881dc200c9833da726e9376c2e32cff7");
                 ("4a656665", 
                  "7768617420646f2079612077616e7420666f72206e6f7468696e673f", 
                  "5bdcc146bf60754e6a042426089575c75a003f089d2739839dec58b964ec3843");
                 ("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", 
                  "dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd", 
                  "773ea91e36800e46854db8ebd09181a72959098b3ef8c122d9635514ced565fe");
                 ("0102030405060708090a0b0c0d0e0f10111213141516171819",
                  "cdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcd",
                  "82558a389a443c0ea4cc819899f2083a85f0faa3e578f8077a2e3ff46729665b") ]

    let check key data (result : string) =
        let decodedKey = hexDecode key
        let decodedData = hexDecode data
        let calculatedResult = calculateHMACSHA256 decodedData decodedKey 
        Assert.AreEqual(result, convertByteArray2String calculatedResult)

    tests |> List.iter (fun (key, data, result) -> check key data result)

