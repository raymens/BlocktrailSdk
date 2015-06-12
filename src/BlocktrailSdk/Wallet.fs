module internal BlocktrailSdk.Wallet

open HttpSignatures
open BlocktrailSdk.Models

/// Generate a new mnemonic from some random entropy (512 bit)
///
/// @param string    $forceEntropy           (optional) forced entropy instead of random entropy for testing purposes
/// @return string
/// @throws \Exception
////
let generateNewMnemonic (passphrase : string) (forceEntropy : string option) : Bitcoin.BIP39.BIP39 = 
    let entropy = 
        match forceEntropy with
        | None -> BIP39.generateEntropy 256
        | Some(s) -> s
    
    let bytes = readUnicodeBytes entropy
    new Bitcoin.BIP39.BIP39(bytes, passphrase)

/// Create a new key;
///  1) a BIP39 mnemonic
///  2) a seed from that mnemonic with the password
///  3) a private key from that seed
///
/// passphrase             the password to use in the BIP39 creation of the seed
/// forceEntropy           (optional) forced entropy instead of random entropy for testing purposes
let generateNewSeed passphrase (forceEntropy : string option) = 
    let mnemonic = generateNewMnemonic passphrase forceEntropy
    let extKey = new NBitcoin.ExtKey(mnemonic.SeedBytesHexString)
    (mnemonic.MnemonicSentence, mnemonic.SeedBytesHexString, extKey)

/// Create new primary key;
///  1) a BIP39 mnemonic
///  2) a seed from that mnemonic with the password
///  3) a private key from that seed
///
/// passphrase             the password to use in the BIP39 creation of the seed
/// @return array [mnemonic, seed, key]
//TODO: require a strong password?
let newPrimarySeed passPhrase = generateNewSeed passPhrase None

/// Perform a HMAC-SHA256 on a string using a HEX encoded key
let sign (str : string) (key : string) : byte array =
    (calculateHMACSHA256 (readAsciiBytes str) (hexDecode key))

/// Calculate the Authorization header, Signature header and signature string of a request.
/// Headers: (request-target, Date and Content-MD5 are used to calculate the signature.
/// (specification: http://tools.ietf.org/html/draft-cavage-http-signatures-03)
let signLib (api_key : string) (secret : string) (httpMethod : string) (md5 : string) date path =
    let signer = new HttpSigner(new AuthorizationParser(), new HttpSignatureStringExtractor());

    let getHttpMethod = function 
        | "delete" -> System.Net.Http.HttpMethod.Delete 
        | "post"->  System.Net.Http.HttpMethod.Post
        | _ ->  System.Net.Http.HttpMethod.Get

    let request = new Request();
    request.Path <- path
    request.Method <- getHttpMethod httpMethod
    request.SetHeader("Date", date);
    request.SetHeader("Content-md5", md5);

    let spec = new SignatureSpecification()
    spec.Algorithm <- "hmac-sha256"
    spec.Headers <- [| "(request-target)"; "content-md5"; "date"  |] |> Seq.ofArray
    spec.KeyId <- api_key

    let secret = base64 (System.Text.ASCIIEncoding.ASCII.GetBytes(secret))

    signer.Sign(request, spec, spec.KeyId, secret)

    let dict = new System.Collections.Generic.Dictionary<string, string>()
    dict.Add(api_key, secret)
    let keyStore = new KeyStore(dict)

    let signatureString = (new HttpSignatureStringExtractor()).ExtractSignatureString(request, spec)
    
    let signature = signer.Signature(request, spec, keyStore);  

    (request.Headers.["Authorization"], signature.Signature, signatureString)

/// Send a wallet delete request
let sendDeleteWallet key secret identifier checksumAddress checksumSignature =
    let partialPath = sprintf "wallet/%s" identifier
    let path = sprintf "/v1/tBTC/wallet/%s?api_key=%s" identifier key    
    let data = { Checksum = checksumAddress; Signature = checksumSignature }
    let jsonData = jsonEncode data

    let md5 = calculateMD5 path
    let date = rfc1123 System.DateTime.UtcNow
    let authorizationHeader, signatureSigned, signatureString = signLib key secret "delete" md5 date path

    let httpRequest = new System.Net.Http.HttpRequestMessage()
    httpRequest.Headers.Add("Authorization", authorizationHeader)
    httpRequest.Headers.Add("Date", date)
    httpRequest.Headers.Add("User-Agent", userAgent)
    httpRequest.Method <- System.Net.Http.HttpMethod.Delete
    httpRequest.RequestUri <- new System.Uri("https://api.blocktrail.com" + path)
    httpRequest.Content <- new System.Net.Http.StringContent(jsonData)
    httpRequest.Content.Headers.Add("Content-MD5", md5)
    httpRequest.Content.Headers.ContentType <- new System.Net.Http.Headers.MediaTypeHeaderValue("application/json")
    let client = new System.Net.Http.HttpClient()
    let response = (client.SendAsync(httpRequest)).Result

    let responseString = response.Content.ReadAsStringAsync().Result

//    let md5 = calculateMD5 ("/" + path)
//    
//    let date = rfc1123 System.DateTime.UtcNow
//    let authorizationHeader, signatureSigned, signatureString = signLib key secret md5 date path
//    let signatureHeaderValue = authorizationHeader.Substring(10)
//
//    let headers = [ ("Authorization", authorizationHeader); ("content-md5", md5) ]
//    let response = requestDelete partialPath key jsonData headers
    responseString

/// Delete an existing wallet
let deleteWallet key secret (wallet : Wallet) =
    
    // TODO: Delete wallet
    let checksum = wallet.PrimaryPrivateKey.PrivateKey.PubKey.ToString(NBitcoin.Network.TestNet)
    //let blabla = new NBitcoin.BitcoinExtKey(wallet.PrimaryPrivateKey, NBitcoin.Network.TestNet)
    //let bytes = readAsciiBytes checksum
    //let tx = new NBitcoin.Transaction(bytes)
    //tx.Sign(wallet.PrimaryPrivateKey.Key.GetWif(NBitcoin.Network.TestNet), true)

    let btcSecret = new NBitcoin.BitcoinSecret(wallet.PrimaryPrivateKey.PrivateKey, NBitcoin.Network.TestNet);
    let signature = btcSecret.PrivateKey.SignMessage(checksum);

    let response = sendDeleteWallet key secret wallet.Identifier checksum signature

    0

/// Parse the response of Create Wallet 
let parseCreateWalletSuccessResponse json =
    let parsed = convertToObject<CreateWalletResponseRawSuccess>(json)
    let convertedKeyListToHDKeyList = parsed.BlocktrailPublicKeys |> List.map (fun k -> { Key = k.[0]; Path = k.[1] })
    { CreateWalletResponseSuccess.BlocktrailPublicKeys = convertedKeyListToHDKeyList
      CreateWalletResponseSuccess.KeyIndex = parsed.KeyIndex
      CreateWalletResponseSuccess.UpgradeKeyIndex = parsed.UpgradeKeyIndex }
    
/// Send request to Blocktrail to create a new wallet
let sendCreateWallet key secret identifier (primaryPublicKey : HDKey) (backupPublicKey :HDKey) primaryMnemonic checksum key_index = 
    let data = 
        { Identifier = identifier
          PrimaryPublicKey = primaryPublicKey
          BackupPublicKey = backupPublicKey
          PrimaryMnemonic = primaryMnemonic
          Checksum = checksum
          KeyIndex = key_index }
    
    let jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(data)
    let md5 = calculateMD5 jsonData

    let path = "/v1/tBTC/wallet?api_key=" + key
    
    let date = rfc1123 System.DateTime.UtcNow
    let authorizationHeader, signatureSigned, signatureString = signLib key secret "post" md5 date path

    let httpRequest = new System.Net.Http.HttpRequestMessage()
    httpRequest.Headers.Add("Authorization", authorizationHeader)
    httpRequest.Headers.Add("Date", date)
    httpRequest.Headers.Add("User-Agent", userAgent)
    httpRequest.Method <- System.Net.Http.HttpMethod.Post
    httpRequest.RequestUri <- new System.Uri("https://api.blocktrail.com" + path)
    httpRequest.Content <- new System.Net.Http.StringContent(jsonData)
    httpRequest.Content.Headers.Add("Content-MD5", md5)
    httpRequest.Content.Headers.ContentType <- new System.Net.Http.Headers.MediaTypeHeaderValue("application/json")
    let client = new System.Net.Http.HttpClient()
    let response = (client.SendAsync(httpRequest)).Result

    let responseString = response.Content.ReadAsStringAsync().Result

    match response.IsSuccessStatusCode with
    | true -> CreateWalletResponse.Succes(parseCreateWalletSuccessResponse responseString)
    | false -> CreateWalletResponse.Error(convertToObject<HttpErrorMessage> responseString)

let createWalletRecord identifier primaryPrivateKey primaryMnemonic backupPublicKey keyIndex network blocktrailPublicKeys =
    { Identifier = identifier;
      PrimaryPrivateKey = primaryPrivateKey; PrimaryMnemonic = primaryMnemonic; 
      BackupPublicKey = backupPublicKey; KeyIndex = keyIndex; Network = network; 
      BlocktrailPublicKeys = blocktrailPublicKeys }

///  create a new wallet
///   - will generate a new primary seed (with password) and backup seed (without password)
///   - send the primary seed (BIP39 'encrypted') and backup public key to the server
///   - receive the blocktrail co-signing public key from the server
///
/// param      identifier
/// param      password
/// param int  key_index         override for the account to use, this number specifies which blocktrail cosigning key is used
/// return array[Wallet, (string)backupMnemonic]
let createNewWallet (key: string) (secret : string) (identifier : string) (password : string) (key_index : int) = 
    // create new primaryseed
    let primaryMnemonic, primarySeed, primaryPrivateKey = newPrimarySeed password
    
    // create primary public key from the created private key
    let keyPath = new NBitcoin.KeyPath("0'")
    let primaryPublicKey = primaryPrivateKey.Derive(keyPath).Neuter()
    
    // create new backup seed
    let backupMnemonic, backupSeed, backupPrivateKey = newPrimarySeed ""
    
    // create backup public key from the created private key
    let backupPublicKey = backupPrivateKey.Neuter()

    // create a checksum of our private key which we'll later use to verify we used the right password
    let checksum = primaryPrivateKey.PrivateKey.PubKey.ToString(NBitcoin.Network.TestNet)

    // send the public keys to the server to store them
    //  and the mnemonic, which is safe because it's useless without the password
    let response = sendCreateWallet key secret identifier 
                         { Key = primaryPublicKey.ToString(NBitcoin.Network.TestNet); Path = "M/0'" }
                         { Key = backupPublicKey.ToString(NBitcoin.Network.TestNet); Path = "M" } 
                         primaryMnemonic checksum key_index

    // received the blocktrail public keys

    // TODO:
    // if the response suggest we should upgrade to a different blocktrail cosigning key then we should
    // do the upgrade to the new 'account' number for the BIP44 path
    (*
    $blocktrailPublicKeys = $result['blocktrail_public_keys'];

    // if the response suggests we should upgrade to a different blocktrail cosigning key then we should
    if (isset($result['upgrade_account'])) {
        $account = $result['upgrade_account'];

        // do the upgrade to the new 'account' number for the BIP44 Path
        $primaryPublicKey = BIP32::extended_private_to_public(BIP32::build_key($primaryPrivateKey, (string)BIP44::BIP44(($this->testnet ? 1 : 0), $account)->accountPath()));
        $result = $this->upgradeAccount($identifier, $account, $primaryPublicKey);

        // update the blocktrail public keys
        $blocktrailPublicKeys = $blocktrailPublicKeys + $result['blocktrail_public_keys'];
    }*)
    
    // return wallet
    match response with 
    | CreateWalletResponse.Succes(r) -> Some(createWalletRecord identifier primaryPrivateKey primaryMnemonic backupPublicKey 0 "Testnet" r.BlocktrailPublicKeys)
    | CreateWalletResponse.Error(r) -> None