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
type Transaction() = 
    
    [<JsonProperty("hash")>]
    member val Hash = "" with get, set
    
    [<JsonProperty("first_seen_at")>]
    member val FirstSeenAt = "" with get, set
    
    [<JsonProperty("last_seen_at")>]
    member val LastSeenAt = "" with get, set
    
    [<JsonProperty("block_height")>]
    member val BlockHeight = 0 with get, set
    
    [<JsonProperty("block_time")>]
    member val BlockTime = "" with get, set
    
    [<JsonProperty("block_hash")>]
    member val BlockHash = "" with get, set
    
    [<JsonProperty("confirmations")>]
    member val Confirmations = 0 with get, set
    
    [<JsonProperty("is_coinbase")>]
    member val IsCoinbase = false with get, set
    
    [<JsonProperty("estimated_value")>]
    member val EstimatedValue = 0L with get, set
    
    [<JsonProperty("total_input_value")>]
    member val TotalInputValue = 0L with get, set
    
    [<JsonProperty("total_output_value")>]
    member val TotalOutputValue = 0L with get, set
    
    [<JsonProperty("total_fee")>]
    member val TotalFee = 0 with get, set
    
    [<JsonProperty("estimated_change")>]
    member val EstimatedChange = Nullable.op_Implicit 0L : Nullable<int64> with get, set
    
    [<JsonProperty("estimated_change_address")>]
    member val EstimatedChangeAddress = "" with get, set
    
    [<JsonProperty("high_priority")>]
    member val HighPriority = Nullable.op_Implicit false with get, set
    
    [<JsonProperty("enough_fee")>]
    member val EnoughFee = Nullable.op_Implicit false with get, set
    
    [<JsonProperty("contains_dust")>]
    member val ContainsDust = Nullable.op_Implicit false with get, set
    
    [<JsonProperty("inputs")>]
    member val Inputs = null : TransactionInput array with get, set
    
    [<JsonProperty("outputs")>]
    member val Outputs = null : TransactionOutput array with get, set

/// <summary>
/// Transaction that is retrieved from a block.
/// </summary>
type RelatedTransaction() = 
    
    [<JsonProperty("hash")>]
    member val Hash = "" with get, set
    
    [<JsonProperty("time")>]
    member val Time = "" with get, set
    
    [<JsonProperty("confirmations")>]
    member val Confirmations = 0 with get, set
    
    [<JsonProperty("is_coinbase")>]
    member val IsCoinbase = false with get, set
    
    [<JsonProperty("estimated_value")>]
    member val EstimatedValue = 0L with get, set
    
    [<JsonProperty("total_input_value")>]
    member val TotalInputValue = 0L with get, set
    
    [<JsonProperty("total_output_value")>]
    member val TotalOutputValue = 0L with get, set
    
    [<JsonProperty("total_fee")>]
    member val TotalFee = 0 with get, set
    
    [<JsonProperty("estimated_change")>]
    member val EstimatedChange = Nullable.op_Implicit 0L : Nullable<int64> with get, set
    
    [<JsonProperty("estimated_change_address")>]
    member val EstimatedChangeAddress = "" with get, set
    
    [<JsonProperty("inputs")>]
    member val Inputs = null : TransactionInput array with get, set
    
    [<JsonProperty("outputs")>]
    member val Outputs = null : TransactionOutput array with get, set

/// <summary>
/// Will retrieve a specific address containing basic info such as the current balance, 
/// the total amount received, and the number of transactions made. 
/// </summary>
type Address() = 
    
    [<JsonProperty("address")>]
    member val Address = "" with get, set
    
    [<JsonProperty("hash160")>]
    member val Hash160 = "" with get, set
    
    [<JsonProperty("balance")>]
    member val Balance = 0L with get, set
    
    [<JsonProperty("received")>]
    member val Received = 0L with get, set
    
    [<JsonProperty("sent")>]
    member val Sent = 0L with get, set
    
    [<JsonProperty("unconfirmed_received")>]
    member val UnconfirmedReceived = 0 with get, set
    
    [<JsonProperty("unconfirmed_sent")>]
    member val UnconfirmedSent = 0L with get, set
    
    [<JsonProperty("unconfirmed_transactions")>]
    member val UnconfirmedTransactions = 0 with get, set
    
    [<JsonProperty("total_transactions_in")>]
    member val TotalTransactionsIn = 0 with get, set
    
    [<JsonProperty("total_transactions_out")>]
    member val TotalTransactionsOut = 0 with get, set
    
    [<JsonProperty("category")>]
    member val Category = "" with get, set
    
    [<JsonProperty("tag")>]
    member val Tag = "" with get, set

/// <summary>
/// Block containing containing information about the difficulty,
/// confirmations, transactions and more.
/// </summary>
type Block() = 
    
    [<JsonProperty("hash")>]
    member val Hash = "" with get, set
    
    [<JsonProperty("height")>]
    member val Height = 0 with get, set
    
    [<JsonProperty("block_time")>]
    member val BlockTime = "" with get, set
    
    [<JsonProperty("difficulty")>]
    member val Difficulty = 0L with get, set
    
    [<JsonProperty("merkleroot")>]
    member val Merkleroot = "" with get, set
    
    [<JsonProperty("is_orphan")>]
    member val IsOrphan = false with get, set
    
    [<JsonProperty("prev_block")>]
    member val PrevBlock = "" with get, set
    
    [<JsonProperty("next_block")>]
    member val NextBlock = "" with get, set
    
    [<JsonProperty("byte_size")>]
    member val ByteSize = 0 with get, set
    
    [<JsonProperty("confirmations")>]
    member val Confirmations = 0 with get, set
    
    [<JsonProperty("transactions")>]
    member val Transactions = 0 with get, set
    
    [<JsonProperty("value")>]
    member val Value = 0L with get, set
    
    [<JsonProperty("miningpool_name")>]
    member val MiningpoolName = "" with get, set
    
    [<JsonProperty("miningpool_url")>]
    member val MiningpoolUrl = "" with get, set
    
    [<JsonProperty("miningpool_slug")>]
    member val MiningpoolSlug = "" with get, set
