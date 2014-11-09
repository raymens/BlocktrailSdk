module BlocktrailSdk.Tests

open BlocktrailSdk
open NUnit.Framework

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