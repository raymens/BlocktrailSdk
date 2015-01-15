namespace BlocktrailSdk.Models

open System
open BlocktrailSdk.Helpers
open Newtonsoft.Json

/// <summary>
/// Input of a transaction.
/// </summary>
type TransactionInput = 
    { [<JsonProperty("index")>]
      Index : int
      [<JsonProperty("output_hash")>]
      OutputHash : string
      [<JsonProperty("output_index")>]
      OutputIndex : int
      [<JsonProperty("value")>]
      Value : int64
      [<JsonProperty("address")>]
      Address : string
      [<JsonProperty("type")>]
      Type : string
      [<JsonProperty("multisig")>]
      Multisig : Object
      [<JsonProperty("multisig_addresses")>]
      MultisigAddresses : string []
      [<JsonProperty("script_signature")>]
      ScriptSignature : string }

/// <summary>
/// Output of a transaction
/// </summary>
type TransactionOutput = 
    { [<JsonProperty("index")>]
      Index : int
      [<JsonProperty("value")>]
      Value : int64
      [<JsonProperty("address")>]
      Address : string
      [<JsonProperty("type")>]
      Type : string
      [<JsonProperty("multisig")>]
      Multisig : string
      [<JsonProperty("multisig_addresses")>]
      MultisigAddresses : string []
      [<JsonProperty("script")>]
      Script : Object
      [<JsonProperty("script_hex")>]
      ScriptHex : string
      [<JsonProperty("spent_hash")>]
      SpentHash : string
      [<JsonProperty("spent_index")>]
      SpentIndex : int }

/// <summary>
/// Unspent output of a transaction
/// </summary>
type TransactionUnspentOutput = 
    { [<JsonProperty("hash")>]
      Hash : string
      [<JsonProperty("time")>]
      Time : string
      [<JsonProperty("confirmations")>]
      Confirmations : int
      [<JsonProperty("is_coinbase")>]
      IsCoinbase : bool
      [<JsonProperty("value")>]
      Value : int64
      [<JsonProperty("index")>]
      Index : int
      [<JsonProperty("address")>]
      Address : string
      [<JsonProperty("type")>]
      Type : string
      [<JsonProperty("multisig")>]
      Multisig : string
      [<JsonProperty("script")>]
      Script : string
      [<JsonProperty("script_hex")>]
      ScriptHex : string }

/// <summary>
/// Type that is returned upon a paginated request by the Blocktrail API.
/// </summary>
type Paging<'T> = 
    { [<JsonProperty("current_page")>]
      CurrentPage : int
      [<JsonProperty("per_page")>]
      PerPage : int
      [<JsonProperty("total")>]
      Total : int64
      [<JsonProperty("data")>]
      Data : array<'T> }

/// <summary>
/// Transaction returned from the Blocktrail client.
/// </summary>
type Transaction = { 
    [<JsonProperty("hash")>]
    Hash : string
    [<JsonProperty("first_seen_at")>]
    FirstSeenAt : string
    [<JsonProperty("last_seen_at")>]
    LastSeenAt : string 
    [<JsonProperty("block_height")>]
    BlockHeight : int 
    [<JsonProperty("block_time")>]
    BlockTime : string 
    [<JsonProperty("block_hash")>]
    BlockHash : string 
    [<JsonProperty("confirmations")>]
    Confirmations : int
    [<JsonProperty("is_coinbase")>]
    IsCoinbase : bool
    [<JsonProperty("estimated_value")>]
    EstimatedValue : int64
    [<JsonProperty("total_input_value")>]
    TotalInputValue : int64
    [<JsonProperty("total_output_value")>]
    TotalOutputValue : int64
    [<JsonProperty("total_fee")>]
    TotalFee : int
    [<JsonProperty("estimated_change")>]
    EstimatedChange : Nullable<int64> 
    [<JsonProperty("estimated_change_address")>]
    EstimatedChangeAddress : string 
    [<JsonProperty("high_priority")>]
    HighPriority : Nullable<bool>
    [<JsonProperty("enough_fee")>]
    EnoughFee : Nullable<bool>
    [<JsonProperty("contains_dust")>]
    ContainsDust : Nullable<bool> 
    [<JsonProperty("inputs")>]
    Inputs : TransactionInput array 
    [<JsonProperty("outputs")>]
    Outputs : TransactionOutput array }

/// <summary>
/// Transaction that is retrieved from a block.
/// </summary>
type RelatedTransaction = {
    [<JsonProperty("hash")>]
    Hash : string 
    [<JsonProperty("time")>]
    Time : string 
    [<JsonProperty("confirmations")>]
    Confirmations : int
    [<JsonProperty("is_coinbase")>]
    IsCoinbase : bool
    [<JsonProperty("estimated_value")>]
    EstimatedValue : int64
    [<JsonProperty("total_input_value")>]
    TotalInputValue : int64
    [<JsonProperty("total_output_value")>]
    TotalOutputValue : int64
    [<JsonProperty("total_fee")>]
    TotalFee : int
    [<JsonProperty("estimated_change")>]
    EstimatedChange : Nullable<int64> 
    [<JsonProperty("estimated_change_address")>]
    EstimatedChangeAddress : string 
    [<JsonProperty("inputs")>]
    Inputs : TransactionInput array 
    [<JsonProperty("outputs")>]
    Outputs : TransactionOutput array }

/// <summary>
/// Will retrieve a specific address containing basic info such as the current balance, 
/// the total amount received, and the number of transactions made. 
/// </summary>
type Address = {
    [<JsonProperty("address")>]
    Address : string 
    [<JsonProperty("hash160")>]
    Hash160 : string 
    [<JsonProperty("balance")>]
    Balance : int64
    [<JsonProperty("received")>]
    Received : int64
    [<JsonProperty("sent")>]
    Sent : int64
    [<JsonProperty("unconfirmed_received")>]
    UnconfirmedReceived : int 
    [<JsonProperty("unconfirmed_sent")>]
    UnconfirmedSent : int64
    [<JsonProperty("unconfirmed_transactions")>]
    UnconfirmedTransactions : int
    [<JsonProperty("total_transactions_in")>]
    TotalTransactionsIn : int
    [<JsonProperty("total_transactions_out")>]
    TotalTransactionsOut : int 
    [<JsonProperty("category")>]
    Category : string 
    [<JsonProperty("tag")>]
    Tag : string }

/// <summary>
/// Block containing containing information about the difficulty,
/// confirmations, transactions and more.
/// </summary>
type Block = {
    [<JsonProperty("hash")>]
    Hash : string 
    [<JsonProperty("height")>]
    Height : int 
    [<JsonProperty("block_time")>]
    BlockTime : string 
    [<JsonProperty("difficulty")>]
    Difficulty : int64
    [<JsonProperty("merkleroot")>]
    Merkleroot : string 
    [<JsonProperty("is_orphan")>]
    IsOrphan : bool
    [<JsonProperty("prev_block")>]
    PrevBlock : string 
    [<JsonProperty("next_block")>]
    NextBlock : string 
    [<JsonProperty("byte_size")>]
    ByteSize : int
    [<JsonProperty("confirmations")>]
    Confirmations : int
    [<JsonProperty("transactions")>]
    Transactions : int
    [<JsonProperty("value")>]
    Value : int64
    [<JsonProperty("miningpool_name")>]
    MiningpoolName : string 
    [<JsonProperty("miningpool_url")>]
    MiningpoolUrl : string 
    [<JsonProperty("miningpool_slug")>]
    MiningpoolSlug : string }
