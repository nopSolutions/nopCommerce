using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Nop.Core.Data
{
    /// <summary>
    /// Data settings (connection string information)
    /// </summary>
    public partial class DataSettings
    {
        protected ProtectedDataService ProtectedDataService = new ProtectedDataService();

        /// <summary>
        /// Ctor
        /// </summary>
        public DataSettings()
        {
            RawDataSettings = new Dictionary<string, string>();
        }

        /// <summary>
        /// Data provider
        /// </summary>
        public string DataProvider { get; set; }

        /// <summary>
        /// Connection string
        /// </summary>
        public string DataConnectionString { get; set; }

        /// <summary>
        /// Should DataConnectionString be encrypted
        /// </summary>
        public bool EncryptStringSettings { get; set; }

        /// <summary>
        /// Raw settings file
        /// </summary>
        public IDictionary<string, string> RawDataSettings { get; private set; }

        /// <summary>
        /// A value indicating whether entered information is valid
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return !String.IsNullOrEmpty(this.DataProvider) && !String.IsNullOrEmpty(this.DataConnectionString);
        }

        /// <summary>
        /// Returns true if any STRING properties are in a decrypted state as determined by ProtectionDataService
        /// </summary>
        /// <returns></returns>
        public bool HasAnyDecryptedDataSettings()
        {
            bool retVal = false;
            PropertyInfo[] properties = this.GetType().GetProperties();

            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(string) && property.GetValue(this) != null && !ProtectedDataService.IsCipherText(property.GetValue(this).ToString()))
                {
                    retVal = true;
                }
            }
            return retVal;
        }

        /// <summary>
        /// Returns true if any STRING properties are in an encrypted state as determined by ProtectionDataService
        /// </summary>
        /// <returns></returns>
        public bool HasAnyEncryptedDataSettings()
        {
            bool retVal = false;
            PropertyInfo[] properties = this.GetType().GetProperties();

            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(string) && property.GetValue(this) != null && ProtectedDataService.IsCipherText(property.GetValue(this).ToString()))
                {
                    retVal = true;
                }
            }
            return retVal;
        }

        /// <summary>
        /// Decrypts all STRING PROPERTIES.  If already decrypted then they are unchanged.
        /// </summary>
        /// <returns></returns>
        public void Decrypt()
        {
            PropertyInfo[] properties = this.GetType().GetProperties();

            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(string) && property.GetValue(this) != null && ProtectedDataService.IsCipherText(property.GetValue(this).ToString()))
                {
                    string clearText = ProtectedDataService.GetClearText(property.GetValue(this).ToString());
                    property.SetValue(this, clearText);
                }
            }
        }

        /// <summary>
        /// Encrypts all STRING PROPERTIES.  If already encrypted then they are unchanged.
        /// </summary>
        /// <returns></returns>
        public void Encrypt()
        {
            PropertyInfo[] properties = this.GetType().GetProperties();

            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(string) && property.GetValue(this) != null && !ProtectedDataService.IsCipherText(property.GetValue(this).ToString()))
                {
                    string cipherText = ProtectedDataService.GetCipherText(this.ProtectedDataService.GetCipherText(property.GetValue(this).ToString()));
                    property.SetValue(this, cipherText);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataSettings GetClone()
        {
            return JsonConvert.DeserializeObject<DataSettings>(JsonConvert.SerializeObject(this, Formatting.None));
        }
    }
}