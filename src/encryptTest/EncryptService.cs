using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace encryptTest
{
    public static class EncryptService
    {

        public static byte[] GetBytes(string input, Encoding encoding)
        {
            return encoding.GetBytes(input);
        }
        public static string Encrypt(string plainText)
        {
            // Check arguments.
            Aes myAes = Aes.Create();


            byte[] passwordBytes = GetBytes("1234567890aA+", new UTF8Encoding());
            byte[] aesKey = SHA256Managed.Create().ComputeHash(passwordBytes);
            byte[] aesIV = MD5.Create().ComputeHash(passwordBytes);
            myAes.Key = aesKey;
            myAes.IV = aesIV;
            myAes.Mode = CipherMode.ECB;
            myAes.Padding = PaddingMode.PKCS7;

            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = myAes.Key;
                aesAlg.IV = myAes.IV;


                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return Convert.ToBase64String(encrypted);
        }

        public static string Decrypt(string plaintxt)
        {

            var cipherText = Convert.FromBase64String(plaintxt);
            Aes myAes = Aes.Create();


            byte[] passwordBytes = GetBytes("1234567890aA+", new UTF8Encoding());
            byte[] aesKey = SHA256Managed.Create().ComputeHash(passwordBytes);
            byte[] aesIV = MD5.Create().ComputeHash(passwordBytes);
            myAes.Key = aesKey;
            myAes.IV = aesIV;
            myAes.Mode = CipherMode.ECB;
            myAes.Padding = PaddingMode.PKCS7;

            byte[] encrypted;

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = myAes.Key;
                aesAlg.IV = myAes.IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}
