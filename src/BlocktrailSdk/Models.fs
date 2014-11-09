namespace BlocktrailSdk.Models

open System

type TransactionInput = 
    { index : int
      output_hash : string
      output_index : int
      value : int64
      address : string
      ``type`` : string
      multisig : Object
      script_signature : string }

type TransactionOutput = 
    { index : int
      value : int64
      address : string
      ``type`` : string
      multisig : Object
      script : string
      script_hex : string
      spent_hash : string
      spent_index : int }

type Transaction = 
    { hash : string
      first_seen_at : string
      last_seen_at : string
      block_height : int
      block_time : string
      block_hash : string
      confirmations : int
      is_coinbase : bool
      estimated_value : int64
      total_input_value : int64
      total_output_value : int64
      total_fee : int
      estimated_change : Nullable<int64>
      estimated_change_address : string
      high_priority : bool
      enough_fee : bool
      contains_dust : bool
      inputs : TransactionInput list
      outputs : TransactionOutput list }

type Address = 
    { address : string
      hash160 : string
      balance : int
      received : int
      sent : int
      unconfirmed_received : int
      unconfirmed_sent : int
      unconfirmed_transactions : int
      total_transactions_in : int
      total_transactions_out : int
      category : string
      tag : string }

type Block = 
    { hash : string
      height : int
      block_time : string
      difficulty : int64
      merkleroot : string
      is_orphan : bool
      prev_block : string
      next_block : string
      byte_size : int
      confirmations : int
      transactions : int
      value : int64
      miningpool_name : string
      miningpool_url : string
      miningpool_slug : string }

type Output = 
    { hash : string
      height : int
      block_time : string
      difficulty : int64
      merkleroot : string
      is_orphan : bool
      byte_size : int
      confirmations : int
      transactions : int
      value : int64
      miningpool_name : string
      miningpool_url : string
      miningpool_slug : string }

type Paging<'T> = 
    { current_page : int
      per_page : int
      total : int64
      data : array<'T> }
