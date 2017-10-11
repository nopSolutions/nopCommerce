using System;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace Nop.Core.Data
{
    /// <summary>
    /// Provides utility methods to allow easy encryption and description of small pieces of data with keys based on the user that the program is being run under.
    /// </summary>
    public class ProtectedDataService
    {
        protected const DataProtectionScope PROTECTION_SCOPE = DataProtectionScope.CurrentUser;  //This value determines if ProtectedData will happen at the user account level or machine level
        private const string ENCRYPTION_PREFIX = "ENCRYPT::"; //This will be prepended to the beginning of every encrypted string so we can easily determine if it's encrypted or not.

        /// <summary>
        /// Returns the clear text version of ciphertext generated from this class.  When it receives a string without the correct prefix it will function as a null object and return the input parameter.
        /// </summary>
        /// <param name="cipherText">Text to be deciphered.  Should begin with ENCRYPTION_PREFIX value and be base64 encoded.</param>
        /// <returns></returns>
        public string GetClearText(string cipherText)
        {
            if (!IsCipherText(cipherText))
                return cipherText;
            var decodedCipherText = System.Convert.FromBase64String(cipherText.Substring(ENCRYPTION_PREFIX.Length));
            return Encoding.UTF8.GetString(ProtectedData.Unprotect(decodedCipherText, GetEntropy(), PROTECTION_SCOPE));
        }

        /// <summary>
        /// Returns ciphertext that is base64 encoded and prefixed with the ENCRYPTION_PREFIX value.
        /// </summary>
        /// <param name="clearText">Plain text string to be encrypted.</param>
        /// <returns></returns>
        public string GetCipherText(string clearText)
        {
            if (IsCipherText(clearText))
                return clearText;
            byte[] clearTextBytes = Encoding.UTF8.GetBytes(clearText);
            var unencodedCipherText = ProtectedData.Protect(clearTextBytes, GetEntropy(), PROTECTION_SCOPE);
            var encodedCipherText = System.Convert.ToBase64String(unencodedCipherText);
            return PrefixSettingWithEncryptionFlag(encodedCipherText);
        }

        /// <summary>
        /// Helper method that checks to see if a string starts with ENCRYPTION_PREFIX value
        /// </summary>
        /// <param name="value">text to check.</param>
        /// <returns></returns>
        public bool IsCipherText(string value)
        {
            return value.StartsWith(ENCRYPTION_PREFIX);
        }

        /// <summary>
        /// Returns entropy that is required for ProtectedData to work.  The entropy is non-changing based on the user the code is running under.
        /// </summary>
        /// <returns></returns>
        protected byte[] GetEntropy()
        {   //TODO: This may not be the best way to create repeatable, secure entropy.
            byte[] entropy = Encoding.ASCII.GetBytes(WindowsIdentity.GetCurrent().Name);
            Array.Resize(ref entropy, 16);
            return entropy;
        }

        /// <summary>
        /// Helper method that returns a string prefixed with the value of ENCRYPTION_PREFIX
        /// </summary>
        /// <param name="cipherText">ciper text that needs to have prefix attached to denote that it is cipher text.</param>
        /// <returns></returns>
        protected string PrefixSettingWithEncryptionFlag(string cipherText)
        {
            return $"{ENCRYPTION_PREFIX}{cipherText}";
        }
    }
}