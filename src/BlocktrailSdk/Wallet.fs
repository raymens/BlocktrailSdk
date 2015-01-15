module Wallet

open FSharp.Data
open HttpSignatures



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
    (mnemonic.MnemonicSentence, mnemonic.SeedBytesHexString, extKey)

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
      primary_public_key : string array
      backup_public_key : string array
      primary_mnemonic : string
      checksum : string
      key_index : int }

let convertByteArray2String (bytes : byte array) = System.BitConverter.ToString(bytes).Replace("-", "").ToLower(); //bytes |> Array.fold (fun s x -> s + x.ToString("x2")) ""

let readBytes (str : string) : byte array = 
    System.Text.Encoding.ASCII.GetBytes(str)

let hexDecode (str : string) : byte array =
    str
    |> Seq.windowed 2
    |> Seq.mapi (fun i j -> (i,j))
    |> Seq.filter (fun (i,j) -> i % 2=0)
    |> Seq.map (fun (_,j) -> System.Byte.Parse(new System.String(j),System.Globalization.NumberStyles.AllowHexSpecifier))
    |> Array.ofSeq 

let generateMD5 (str : string) : string = 
    use md5 = System.Security.Cryptography.MD5.Create()
    let bts = readBytes str
    let retVal = md5.ComputeHash(bts)
    convertByteArray2String retVal

let generateHMACSHA256 (data : byte array) (key : byte array) = 
    use hmac = new System.Security.Cryptography.HMACSHA256(key)
    hmac.ComputeHash(data)

let base64 (bytes : byte array) : string = 
    System.Convert.ToBase64String(bytes)


let sign (str : string) (secret : string) : byte array =
    (generateHMACSHA256 (readBytes str) (hexDecode secret))

let signLib (api_key : string) (secret : string) (md5 : string) date path =
    let signer = new HttpSigner(new AuthorizationParser(), new HttpSignatureStringExtractor());

    let request = new Request();
    request.Path <- path
    request.Method <- System.Net.Http.HttpMethod.Post
    //request.SetHeader("host", "api.blocktrail.com");
    request.SetHeader("Date", date);
    request.SetHeader("Content-md5", md5);

    let spec = new SignatureSpecification()
    spec.Algorithm <- "hmac-sha256"
    spec.Headers <- [| "(request-target)"; "date";  "content-md5" |] |> Seq.ofArray
    spec.KeyId <- api_key

    let secret = System.Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(secret))

    signer.Sign(request, spec, spec.KeyId, secret)

    let dict = new System.Collections.Generic.Dictionary<string, string>()
    dict.Add(api_key, secret)
    let keyStore = new KeyStore(dict)

    let signatureString = (new HttpSignatureStringExtractor()).ExtractSignatureString(request, spec)
    

    let signature = signer.Signature(request, spec, keyStore);  

    (request.Headers.["Authorization"], signature.Signature, signatureString)

let sendCreateWallet identifier (primaryPublicKey : string array) (backupPublicKey : string array) primaryMnemonic checksum key_index = 
    let data = 
        { identifier = identifier
          primary_public_key = primaryPublicKey
          backup_public_key = backupPublicKey
          primary_mnemonic = primaryMnemonic
          checksum = checksum
          key_index = key_index }
    
    let jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(data)
    let key = "API_KEY"
    let secret = "API_SECRET"
    let md5 = generateMD5 jsonData

    let path = "/v1/tBTC/wallet?api_key=" + key
    
    let date = System.DateTime.UtcNow.ToString("R")
    let authorizationHeader, signatureSigned, signatureString = signLib key secret md5 date path

    let signatureHeaderValue = authorizationHeader.Substring(10)

    let httpRequest = new System.Net.Http.HttpRequestMessage()
    httpRequest.Headers.Add("Authorization", authorizationHeader)
    httpRequest.Headers.Add("Date", date)
    httpRequest.Headers.Add("Signature", signatureHeaderValue)
    httpRequest.Method <- System.Net.Http.HttpMethod.Post
    httpRequest.RequestUri <- new System.Uri("https://api.blocktrail.com" + path)
    httpRequest.Content <- new System.Net.Http.StringContent(jsonData)
    httpRequest.Content.Headers.Add("Content-MD5", md5)
    httpRequest.Content.Headers.ContentType <- new System.Net.Http.Headers.MediaTypeHeaderValue("application/json")
    let client = new System.Net.Http.HttpClient()
    let response2 = (client.SendAsync(httpRequest)).Result

    let statusCode, response = (response2.StatusCode.ToString(), response2.Content.ReadAsStringAsync().Result)

    (statusCode, response)

///  create a new wallet
///   - will generate a new primary seed (with password) and backup seed (without password)
///   - send the primary seed (BIP39 'encrypted') and backup public key to the server
///   - receive the blocktrail co-signing public key from the server
///
/// @param      $identifier
/// @param      $password
/// @param int  $account         override for the account to use, this number specifies which blocktrail cosigning key is used
/// @return array[Wallet, (string)backupMnemonic]
let createNewWallet (identifier : string) (password : string) (key_index : int) = 
    // create new primaryseed
    let primaryMnemonic, primarySeed, primaryPrivateKey = newPrimarySeed password
    // create primary public key from the created private key
    //let primaryPublicKey = primaryPrivateKey.Neuter()
    let keyPath = new NBitcoin.KeyPath("0'")
    let primaryPublicKey = primaryPrivateKey.Derive(keyPath).Neuter()
    // create new backup seed
    let backupMnemonic, backupSeed, backupPrivateKey = newPrimarySeed ""
    // create backup public key from the created private key
    //let backupPublicKey = backupPrivateKey.Neuter()
    let backupPublicKey = backupPrivateKey.Neuter()
    // create a checksum of our private key which we'll later use to verify we used the right password
    let checksum = primaryPrivateKey.Key.PubKey.ToString(NBitcoin.Network.TestNet)
//    printfn "Primary Mnenomic: %s" primaryMnemonic
//    printfn "Primary Seed: %s" primarySeed
//    printfn "Primary PrivateKey: %s" (primaryPrivateKey.ToString())
//    printfn "Primary PublicKey: %s" (primaryPublicKey.ToString())
//    printfn "Backup Mnenomic: %s" backupMnemonic
//    printfn "Backup Seed: %s" backupSeed
//    printfn "Backup PrivateKey: %s" (backupPrivateKey.ToString())
//    printfn "Backup PublicKey: %s" (backupPublicKey.ToString())
//    printfn "Checksum: %s" (checksum.ToString())
    // send the public keys to the server to store them
    //  and the mnemonic, which is safe because it's useless without the password
    let statusCode, response = 
        sendCreateWallet identifier [| (primaryPublicKey.ToString(NBitcoin.Network.TestNet)); "M/0'" |]
            [| (backupPublicKey.ToString(NBitcoin.Network.TestNet)); "M" |] primaryMnemonic checksum key_index
    // received the blocktrail public keys
    // if the response suggest we should upgrade to a different blocktrail cosigning key then we should
    // do the upgrade to the new 'account' number for the BIP44 path
    // return wallet en mnemonic
    printfn "%s %s" statusCode response
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
