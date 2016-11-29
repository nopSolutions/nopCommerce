using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Nop.Core.Domain.Security;

namespace Nop.Services.Security
{
    public class EncryptionService : IEncryptionService
    {
        private readonly SecuritySettings _securitySettings;
        public EncryptionService(SecuritySettings securitySettings)
        {
            this._securitySettings = securitySettings;
        }

        /// <summary>
        /// Create salt key
        /// </summary>
        /// <param name="size">Key size</param>
        /// <returns>Salt key</returns>
        public virtual string CreateSaltKey(int size) 
        {
            // If 0 was passed, salt setting does not exist
            //  default to nop < 3.90 value of 5
            if (size == 0) {
                size = 5;
            }

            // Generate a cryptographic random number
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[size];
            rng.GetBytes(buff);

            // Return a Base64 string representation of the random number
            return Convert.ToBase64String(buff);
        }

        /// <summary>
        /// Create a password hash
        /// </summary>
        /// <param name="password">{assword</param>
        /// <param name="saltkey">Salk key</param>
        /// <param name="passwordFormat">Password format (hash algorithm)</param>
        /// <returns>Password hash</returns>
        public virtual string CreatePasswordHash(string password, string saltkey, string passwordFormat = "SHA1")
        {
            return CreateHash(Encoding.UTF8.GetBytes(String.Concat(password, saltkey)), passwordFormat);
        }

        /// <summary>
        /// Create a data hash
        /// </summary>
        /// <param name="data">The data for calculating the hash</param>
        /// <param name="hashAlgorithm">Hash algorithm</param>
        /// <returns>Data hash</returns>
        public virtual string CreateHash(byte[] data, string hashAlgorithm = "SHA1")
        {
            if (String.IsNullOrEmpty(hashAlgorithm))
                hashAlgorithm = "SHA1";
           
            //return FormsAuthentication.HashPasswordForStoringInConfigFile(saltAndPassword, passwordFormat);
            var algorithm = HashAlgorithm.Create(hashAlgorithm);
            if (algorithm == null)
                throw new ArgumentException("Unrecognized hash name");

            var hashByteArray = algorithm.ComputeHash(data);
            return BitConverter.ToString(hashByteArray).Replace("-", "");
        }

        /// <summary>
        /// Encrypt text
        /// </summary>
        /// <param name="plainText">Text to encrypt</param>
        /// <param name="encryptionPrivateKey">Encryption private key</param>
        /// <returns>Encrypted text</returns>
        public virtual string EncryptText(string plainText, string encryptionPrivateKey = "") 
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            if (String.IsNullOrEmpty(encryptionPrivateKey))
                encryptionPrivateKey = _securitySettings.EncryptionKey;

            var sAlg = GetAlgorithmInstance(_securitySettings.EncryptionFormat, encryptionPrivateKey);
            
            byte[] encryptedBinary = EncryptTextToMemory(sAlg, plainText, sAlg.Key, sAlg.IV);
            return Convert.ToBase64String(encryptedBinary);
        }

        /// <summary>
        /// Decrypt text
        /// </summary>
        /// <param name="cipherText">Text to decrypt</param>
        /// <param name="encryptionPrivateKey">Encryption private key</param>
        /// <returns>Decrypted text</returns>
        public virtual string DecryptText(string cipherText, string encryptionPrivateKey = "") 
        {
            if (String.IsNullOrEmpty(cipherText))
                return cipherText;

            if (String.IsNullOrEmpty(encryptionPrivateKey))
                encryptionPrivateKey = _securitySettings.EncryptionKey;

            var sAlg = GetAlgorithmInstance(_securitySettings.EncryptionFormat, encryptionPrivateKey);

            byte[] buffer = Convert.FromBase64String(cipherText);
            return DecryptTextFromMemory(sAlg, buffer, sAlg.Key, sAlg.IV);
        }

        #region Utilities

        private byte[] EncryptTextToMemory(SymmetricAlgorithm sAlg, string data, byte[] key, byte[] iv) 
        {
            using (var ms = new MemoryStream()) {
                using (var cs = new CryptoStream(ms, sAlg.CreateEncryptor(key, iv), CryptoStreamMode.Write)) {
                    byte[] toEncrypt = Encoding.Unicode.GetBytes(data);
                    cs.Write(toEncrypt, 0, toEncrypt.Length);
                    cs.FlushFinalBlock();
                }

                return ms.ToArray();
            }
        }

        private string DecryptTextFromMemory(SymmetricAlgorithm sAlg, byte[] data, byte[] key, byte[] iv) 
        {
            using (var ms = new MemoryStream(data)) {
                using (var cs = new CryptoStream(ms, sAlg.CreateDecryptor(key, iv), CryptoStreamMode.Read))
                {
                    using (var sr = new StreamReader(cs, Encoding.Unicode))
                    {
                        return sr.ReadLine();
                    }
                }
            }
        }

        private static SymmetricAlgorithm GetAlgorithmInstance(EncryptionFormat eFormat, string encryptionPrivateKey) {
            SymmetricAlgorithm sAlg = null;
            switch (eFormat) {
                case EncryptionFormat.TripleDes:
                    sAlg = new TripleDESCryptoServiceProvider();
                    // Legacy (<3.90) mode.  This was only using a 128 bit key.
                    sAlg.Key = Encoding.ASCII.GetBytes(encryptionPrivateKey.Substring(0, 16));
                    sAlg.IV = Encoding.ASCII.GetBytes(encryptionPrivateKey.Substring(8, 8));
                    break;
                case EncryptionFormat.Aes128:
                    sAlg = new RijndaelManaged {
                        KeySize = 128,
                        BlockSize = 128,
                        Mode = CipherMode.CBC
                    };
                    sAlg.Key = Encoding.ASCII.GetBytes(encryptionPrivateKey.Substring(0, sAlg.KeySize / 8));
                    sAlg.IV = Encoding.ASCII.GetBytes(encryptionPrivateKey.Substring(8, sAlg.BlockSize / 8));
                    break;
                case EncryptionFormat.Aes256:
                    sAlg = new RijndaelManaged {
                        KeySize = 256,
                        BlockSize = 128,
                        Mode = CipherMode.CBC
                    };
                    sAlg.Key = Encoding.ASCII.GetBytes(encryptionPrivateKey.Substring(0, sAlg.KeySize / 8));
                    sAlg.IV = Encoding.ASCII.GetBytes(encryptionPrivateKey.Substring(8, sAlg.BlockSize / 8));
                    break;
            }

            return sAlg;
        }

        #endregion
    }
}
