module internal BlocktrailSdk.BIP39

open PWDTK_DOTNET451
open System

/// generate random entropy using \MCRYPT_DEV_URANDOM
///
/// @param int  $size                    desired strength, must be multiple of 32, recommended 128-256
/// @throws \Exception
/// @return string                       hex Entropy
let generateEntropy (size : int) : string = 
    if size % 32 <> 0 then failwith "Entropy must be in a multiple of 32"

    let generator = System.Security.Cryptography.RNGCryptoServiceProvider.Create()
    let data : byte array = Array.zeroCreate size
    generator.GetBytes(data)
    convertByteArray2String data

/// Create Mnemonic from Entropy
///
/// @param string        $entropyHex     hex Entropy
/// @param BIP39WordList $wordList       defaults to BIP39 english word list
/// @return string                       hex Mnemonic
////
let entropyToMnemonic (entropyHex : string) (wordList : string list option) = 
    (*
    // calculate entropy, /2 because PHP can't do bytes
    $ENT = (strlen($entropyHex) / 2) * 8;
    // calculate how long the checksum should be
    $CS = $ENT / 32;

    // get the checksum
    $checksum = self::entropyChecksum($entropyHex);

    // create the string of bits to use
    $bits = str_pad(gmp_strval(gmp_init($entropyHex, 16), 2) . $checksum, $ENT + $CS, "0", STR_PAD_LEFT); // PHP trims off 0s

    // use provided wordList or default
    $wordList = $wordList ?: self::defaultWordList();

    // build word list
    $result = array();
    foreach (str_split($bits, 11) as $bit) {
        $idx = gmp_strval(gmp_init($bit, 2), 10);

        $result[] = $wordList->getWord($idx);
    }

    // implode and enjoy
    $result = implode(" ", $result);

    return $result; 
    *)
    let bytes = System.Text.UTF8Encoding.Default.GetBytes(entropyHex)
    let x = new Bitcoin.BIP39.BIP39(entropyBytes = bytes)
    x.MnemonicSentence
    

let mnemonicToSeedHex mnemonic passphrase : Bitcoin.BIP39.BIP39 =
    let bip = new Bitcoin.BIP39.BIP39(512, passphrase)
    bip
    
    (*
       $passphrase = self::normalizePassphrase($passphrase); 
            $salt = "mnemonic" . $passphrase; 
            return hash_pbkdf2("sha512", $mnemonic, $salt, 2048, 64 * 2, false); 
        } 
    *)