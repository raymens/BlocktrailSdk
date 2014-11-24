# BlocktrailSdk
[![Build status](https://ci.appveyor.com/api/projects/status/jv8qagurkj09ng50/branch/master?svg=true)](https://ci.appveyor.com/project/raymens/blocktrailsdk/branch/master)

[![Gitter](https://badges.gitter.im/Join Chat.svg)](https://gitter.im/raymens/BlocktrailSdk?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

This personal and (maybe) soon to be community .NET project is for using the Blocktrail API to discover all there is to know about the Bitcoin blockchain.
This includes fetching information about:

* blocks
* transactions
* addresses

The library is written in F# but is usable in any .NET language. The library is still in the Alpha stage but will be updated to include more features and to improve the stability.

Read the [Getting started tutorial](http://raymens.github.io/BlocktrailSdk/index.html#Getting-started) to learn more.

Documentation: http://raymens.github.io/BlocktrailSdk

## TODO

Functionality | Status     |
--------------|------------|
Conversion of units (Satoshi, btc etc.) | - |

### Data API

API | Status    |
----|-----------|
Address | :) |
Address Transactions | :) |
Address Uncomfirmed Transactions | :) |
Address Unspent Outputs | :) |
Get Block | :) |
Block Transactions | :) |
All Blocks | :) |
Latest Block | :) |
Get Transaction | :) |

### Webhooks API

API | Status       |
----|--------------|
List All Webhooks | - |
Get Existing Webhook | - | 
Update an existing Webhook URL | - |
Delete a Webhook and all it's Events | - |
Subscribe Webhook to Events | - |
List Webhook Events | - |
Unsubscribe the Webhook from an Event | - |

### Payments API (closed beta)
This API is still in closed beta. I'm waiting for the API to be open and released before I can implement it.
Following functionality is what there is to be expected.

API | Status       |
----|--------------|
Creating a Wallet | - |
Initialise Existing Wallet | - |
Get new address | - |
Get wallet balance | - |
Wallet Webhook | - |
Pay | - |
