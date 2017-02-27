//  A non-rigorous mental model.

type ClearContent = string
type EncryptedContent = string

////////////  SERIALIZATION ///////////////
module Serialization =
  type Serializer<'a> = 'a -> string
  type Deserializer<'a> = string -> 'a

module BlockCrypto = 
  type SymmetricKey = string
  type IV = string
  type Encryptor = IV -> SymmetricKey -> ClearContent -> EncryptedContent
  type Decryptor = SymmetricKey -> EncryptedContent -> ClearContent

module Hashing = 
  type Hash = string
  type Passcode = string
  type ContentHasher<'a> = 'a -> Hash

module KeyCrypto = 
  type PrivateKey = string
  type PublicKey = string