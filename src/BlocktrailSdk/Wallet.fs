module Wallet

open FSharp.Data

/// Generate a new mnemonic from some random entropy (512 bit)
///
/// @param string    $forceEntropy           (optional) forced entropy instead of random entropy for testing purposes
/// @return string
/// @throws \Exception
////
let generateNewMnemonic (passphrase : string) (forceEntropy : string option) : Bitcoin.BIP39.BIP39 = 
    let entropy = 
        match forceEntropy with
        | None -> BIP39.generateEntropy 512
        | Some(s) -> s
    let bytes = System.Text.UnicodeEncoding.Default.GetBytes(entropy)
    new Bitcoin.BIP39.BIP39(bytes, passphrase)

/// Create a new key;
///  1) a BIP39 mnemonic
///  2) a seed from that mnemonic with the password
///  3) a private key from that seed
///
/// @param string    $passphrase             the password to use in the BIP39 creation of the seed
/// @param string    $forceEntropy           (optional) forced entropy instead of random entropy for testing purposes
/// @return array
////
let generateNewSeed passphrase (forceEntropy : string option) = 
    let mnemonic = generateNewMnemonic passphrase forceEntropy

    let extKey = new NBitcoin.ExtKey(mnemonic.SeedBytesHexString)
    
    let btc = new NBitcoin.BitcoinExtKey(extKey, NBitcoin.Network.TestNet)
    (mnemonic.MnemonicSentence, mnemonic.SeedBytesHexString, btc)

let generatePrivateKey seed = 
    let seedBytes = NBitcoin.DataEncoders.Encoders.Hex.DecodeData(seed)
    let haskHey = System.Text.Encoding.UTF8.GetBytes("Bitcoin seed")
    let hashMAC = NBitcoin.Crypto.Hashes.HMACSHA512(haskHey, seedBytes)
    
    let key = 
        [| for i = 0 to 31 do
               yield hashMAC.[i] |]
    key

/// Create new primary key;
///  1) a BIP39 mnemonic
///  2) a seed from that mnemonic with the password
///  3) a private key from that seed
///
/// @param string    $passphrase             the password to use in the BIP39 creation of the seed
/// @return array [mnemonic, seed, key]
/// @TODO: require a strong password?
///
let newPrimarySeed passPhrase = generateNewSeed passPhrase None

type CreateWallet = 
    { identifier : string
      primary_public_key : string
      backup_public_key : string
      primary_mnemonic : string
      checksum : string
      account : int }

let sendCreateWallet identifier primaryPublicKey backupPublicKey primaryMnemonic checksum account = 
    let data = { identifier = identifier; primary_public_key = primaryPublicKey; backup_public_key = backupPublicKey; primary_mnemonic = primaryMnemonic; checksum = checksum; account = account }
    let jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(data)

    let response = try
                        Http.RequestString("https://api.blocktrail.com/v1/BTC/wallet", query = [ "api_key", "" ], httpMethod = "POST", body = TextRequest jsonData)
                   with 
                        | :? System.Net.WebException as e -> e.ToString()

    response
        
    

///  create a new wallet
///   - will generate a new primary seed (with password) and backup seed (without password)
///   - send the primary seed (BIP39 'encrypted') and backup public key to the server
///   - receive the blocktrail co-signing public key from the server
///
/// @param      $identifier
/// @param      $password
/// @param int  $account         override for the account to use, this number specifies which blocktrail cosigning key is used
/// @return array[Wallet, (string)backupMnemonic]
let createNewWallet (identifier : string) (password : string) (account : int) = 
    let testnet = true
    // create new primaryseed
    let primaryMnemonic, primarySeed, primaryPrivateKey = newPrimarySeed password

    // create primary public key from the created private key
    let primaryPublicKey = primaryPrivateKey.ExtKey.Key.PubKey

    // create new backup seed
    let backupMnemonic, backupSeed, backupPrivateKey = newPrimarySeed ""

    // create backup public key from the created private key
    let backupPublicKey = backupPrivateKey.ExtKey.Key.PubKey

    // create a checksum of our private key which we'll later use to verify we used the right password
    (* let key = generatePrivateKey primarySeed.SeedBytesHexString
    let privateKey = new Bitcoin.KeyCore.PrivateKey(Bitcoin.BitcoinUtilities.Globals.TestAddressVersion, key)
    let publicKey = new Bitcoin.KeyCore.PublicKey(privateKey, Bitcoin.BitcoinUtilities.Globals.TestAddressVersion)
    let addr = new Bitcoin.KeyCore.BitcoinAddress(publicKey)
    let checksum = addr.BitcoinAddressEncodedString *)
    let checksum = primaryPublicKey.GetAddress(NBitcoin.Network.TestNet).ToString()

    printfn "Primary Mnenomic: %s" primaryMnemonic
    printfn "Primary Seed: %s" primarySeed
    printfn "Primary PrivateKey: %s" (primaryPrivateKey.ToString())
    printfn "Primary PublicKey: %s" (primaryPublicKey.ToString())

    printfn "Backup Mnenomic: %s" backupMnemonic
    printfn "Backup Seed: %s" backupSeed
    printfn "Backup PrivateKey: %s" (backupPrivateKey.ToString())
    printfn "Backup PublicKey: %s" (backupPublicKey.ToString())

    printfn "Checksum: %s" (checksum.ToString())


    // send the public keys to the server to store them
    //  and the mnemonic, which is safe because it's useless without the password
    let response = sendCreateWallet identifier (primaryPublicKey.ToString()) (backupPublicKey.ToString()) primaryMnemonic checksum account

    // received the blocktrail public keys
    // if the response suggest we should upgrade to a different blocktrail cosigning key then we should
    // do the upgrade to the new 'account' number for the BIP44 path
    // return wallet en mnemonic
    0
(*

    public function createNewWallet($identifier, $password, $account = 0) {
        // create new primary seed
        list($primaryMnemonic, $primarySeed, $primaryPrivateKey) = $this->newPrimarySeed($password);
        // create primary public key from the created private key
        $primaryPublicKey = BIP32::extended_private_to_public(BIP32::build_key($primaryPrivateKey, (string)BIP44::BIP44(($this->testnet ? 1 : 0), $account)->accountPath()));

        // create new backup seed
        list($backupMnemonic, $backupSeed, $backupPrivateKey) = $this->newBackupSeed();
        // create backup public key from the created private key
        $backupPublicKey = BIP32::extract_public_key($backupPrivateKey);

        // create a checksum of our private key which we'll later use to verify we used the right password
        $checksum = $this->createChecksum($primaryPrivateKey);

        // send the public keys to the server to store them
        //  and the mnemonic, which is safe because it's useless without the password
        $result = $this->_createNewWallet($identifier, $primaryPublicKey, $backupPublicKey, $primaryMnemonic, $checksum, $account);
        // received the blocktrail public keys
        $blocktrailPublicKeys = $result['blocktrail_public_keys'];

        // if the response suggests we should upgrade to a different blocktrail cosigning key then we should
        if (isset($result['upgrade_account'])) {
            $account = $result['upgrade_account'];

            // do the upgrade to the new 'account' number for the BIP44 Path
            $primaryPublicKey = BIP32::extended_private_to_public(BIP32::build_key($primaryPrivateKey, (string)BIP44::BIP44(($this->testnet ? 1 : 0), $account)->accountPath()));
            $result = $this->upgradeAccount($identifier, $account, $primaryPublicKey);

            // update the blocktrail public keys
            $blocktrailPublicKeys = $blocktrailPublicKeys + $result['blocktrail_public_keys'];
        }

        // return wallet and backup mnemonic
        return array(new Wallet($this, $identifier, $primaryPrivateKey, $backupPublicKey, $blocktrailPublicKeys, $account, $this->testnet), $backupMnemonic);
    }

    /**
     * create wallet using the API
     *
     * @param string    $identifier             the wallet identifier to create
     * @param array     $primaryPublicKey       BIP32 extended public key - array(key, path)
     * @param string    $backupPublicKey        plain public key
     * @param string    $primaryMnemonic        mnemonic to store
     * @param string    $checksum               checksum to store
     * @param int       $account                account that we expect to use
     * @return mixed
     */
    public function _createNewWallet($identifier, $primaryPublicKey, $backupPublicKey, $primaryMnemonic, $checksum, $account) {
        $data = [
            'identifier' => $identifier,
            'primary_public_key' => $primaryPublicKey,
            'backup_public_key' => $backupPublicKey,
            'primary_mnemonic' => $primaryMnemonic,
            'checksum' => $checksum,
            'account' => $account
        ];

        $response = $this->client->post("wallet", null, $data, 'http-signatures');
        return json_decode($response->body(), true);
    }
    *)
