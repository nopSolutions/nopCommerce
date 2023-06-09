using System.Security.Cryptography;
using System.Text;
using Nop.Core;
using Nop.Core.Domain.Security;

namespace Nop.Services.Security
{
    /// <summary>
    /// Encryption service
    /// </summary>
    public partial class EncryptionService : IEncryptionService
    {
        #region Fields

        protected readonly SecuritySettings _securitySettings;

        #endregion

        #region Ctor

        public EncryptionService(SecuritySettings securitySettings)
        {
            _securitySettings = securitySettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Encrypt text
        /// </summary>
        /// <param name="data">Text to encrypt</param>
        /// <param name="provider">Encryption algorithm</param>
        /// <returns>Encrypted data</returns>
        protected static byte[] EncryptTextToMemory(string data, SymmetricAlgorithm provider)
        {
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, provider.CreateEncryptor(), CryptoStreamMode.Write))
            {
                var toEncrypt = Encoding.Unicode.GetBytes(data);
                cs.Write(toEncrypt, 0, toEncrypt.Length);
                cs.FlushFinalBlock();
            }

            return ms.ToArray();
        }

        /// <summary>
        /// Decrypt text
        /// </summary>
        /// <param name="data">Encrypted data</param>
        /// <param name="provider">Encryption algorithm</param>
        /// <returns>Decrypted text</returns>
        protected static string DecryptTextFromMemory(byte[] data, SymmetricAlgorithm provider)
        {
            using var ms = new MemoryStream(data);
            using var cs = new CryptoStream(ms, provider.CreateDecryptor(), CryptoStreamMode.Read);
            using var sr = new StreamReader(cs, Encoding.Unicode);
            
            return sr.ReadToEnd();
        }

        /// <summary>
        /// Gets encryption algorithm
        /// </summary>
        /// <param name="encryptionKey">Encryption key</param>
        /// <returns>Encryption algorithm</returns>
        protected virtual SymmetricAlgorithm GetEncryptionAlgorithm(string encryptionKey)
        {
            if (string.IsNullOrEmpty(encryptionKey))
                throw new ArgumentNullException(nameof(encryptionKey));

            SymmetricAlgorithm provider = _securitySettings.UseAesEncryptionAlgorithm ? Aes.Create() : TripleDES.Create();

            var vectorBlockSize = provider.BlockSize / 8;

            provider.Key = Encoding.ASCII.GetBytes(encryptionKey[0..16]);
            provider.IV = Encoding.ASCII.GetBytes(encryptionKey[^vectorBlockSize..]);

            return provider;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create salt key
        /// </summary>
        /// <param name="size">Key size</param>
        /// <returns>Salt key</returns>
        public virtual string CreateSaltKey(int size)
        {
            //generate a cryptographic random number
            using var provider = RandomNumberGenerator.Create();
            var buff = new byte[size];
            provider.GetBytes(buff);

            // Return a Base64 string representation of the random number
            return Convert.ToBase64String(buff);
        }

        /// <summary>
        /// Create a password hash
        /// </summary>
        /// <param name="password">Password</param>
        /// <param name="saltkey">Salk key</param>
        /// <param name="passwordFormat">Password format (hash algorithm)</param>
        /// <returns>Password hash</returns>
        public virtual string CreatePasswordHash(string password, string saltkey, string passwordFormat)
        {
            return HashHelper.CreateHash(Encoding.UTF8.GetBytes(string.Concat(password, saltkey)), passwordFormat);
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

            if (string.IsNullOrEmpty(encryptionPrivateKey))
                encryptionPrivateKey = _securitySettings.EncryptionKey;

            using var provider = GetEncryptionAlgorithm(encryptionPrivateKey);
            var encryptedBinary = EncryptTextToMemory(plainText, provider);

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
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            if (string.IsNullOrEmpty(encryptionPrivateKey))
                encryptionPrivateKey = _securitySettings.EncryptionKey;

            using var provider = GetEncryptionAlgorithm(encryptionPrivateKey);

            var buffer = Convert.FromBase64String(cipherText);
            return DecryptTextFromMemory(buffer, provider);
        }

        #endregion
    }
}