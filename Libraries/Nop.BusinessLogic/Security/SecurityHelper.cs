//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic.Configuration;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.Common;

namespace NopSolutions.NopCommerce.BusinessLogic.Security
{
    /// <summary>
    /// Represents a security helper
    /// </summary>
    public partial class SecurityHelper
    {
        #region Utilities

        private static byte[] EncryptTextToMemory(string data, byte[] key, byte[] iv)
        {
            var mStream = new MemoryStream();
            var cStream = new CryptoStream(mStream, new TripleDESCryptoServiceProvider().CreateEncryptor(key, iv), CryptoStreamMode.Write);
            byte[] toEncrypt = new UnicodeEncoding().GetBytes(data);
            cStream.Write(toEncrypt, 0, toEncrypt.Length);
            cStream.FlushFinalBlock();
            byte[] ret = mStream.ToArray();
            cStream.Close();
            mStream.Close();
            return ret;
        }

        private static string DecryptTextFromMemory(byte[] data, byte[] key, byte[] iv)
        {
            var msDecrypt = new MemoryStream(data);
            var csDecrypt = new CryptoStream(msDecrypt, new TripleDESCryptoServiceProvider().CreateDecryptor(key, iv), CryptoStreamMode.Read);
            var sReader = new StreamReader(csDecrypt, new UnicodeEncoding());
            return sReader.ReadLine();
        }

        #endregion

        #region Methods
        /// <summary>
        /// Decrypts text
        /// </summary>
        /// <param name="cipherText">Cipher text</param>
        /// <returns>Decrypted string</returns>
        public static string Decrypt(string cipherText)
        {
            string encryptionPrivateKey = SettingManager.GetSettingValue("Security.EncryptionPrivateKey");
            return Decrypt(cipherText, encryptionPrivateKey);
        }

        /// <summary>
        /// Decrypts text
        /// </summary>
        /// <param name="cipherText">Cipher text</param>
        /// <param name="encryptionPrivateKey">Encryption private key</param>
        /// <returns>Decrypted string</returns>
        protected static string Decrypt(string cipherText, string encryptionPrivateKey)
        {
            if (String.IsNullOrEmpty(cipherText))
                return cipherText;

            var tDESalg = new TripleDESCryptoServiceProvider();
            tDESalg.Key = new ASCIIEncoding().GetBytes(encryptionPrivateKey.Substring(0, 16));
            tDESalg.IV = new ASCIIEncoding().GetBytes(encryptionPrivateKey.Substring(8, 8));

            byte[] buffer = Convert.FromBase64String(cipherText);
            string result = DecryptTextFromMemory(buffer, tDESalg.Key, tDESalg.IV);
            return result;
        }

        /// <summary>
        /// Encrypts text
        /// </summary>
        /// <param name="plainText">Plaint text</param>
        /// <returns>Encrypted string</returns>
        public static string Encrypt(string plainText)
        {
            string encryptionPrivateKey = SettingManager.GetSettingValue("Security.EncryptionPrivateKey");
            return Encrypt(plainText, encryptionPrivateKey);
        }

        /// <summary>
        /// Encrypts text
        /// </summary>
        /// <param name="plainText">Plaint text</param>
        /// <param name="encryptionPrivateKey">Encryption private key</param>
        /// <returns>Encrypted string</returns>
        protected static string Encrypt(string plainText, string encryptionPrivateKey)
        {
            if (String.IsNullOrEmpty(plainText))
                return plainText;

            var tDESalg = new TripleDESCryptoServiceProvider();
            tDESalg.Key = new ASCIIEncoding().GetBytes(encryptionPrivateKey.Substring(0, 16));
            tDESalg.IV = new ASCIIEncoding().GetBytes(encryptionPrivateKey.Substring(8, 8));

            byte[] encryptedBinary = EncryptTextToMemory(plainText, tDESalg.Key, tDESalg.IV);
            string result = Convert.ToBase64String(encryptedBinary);
            return result;
        }

        /// <summary>
        /// Change encryption private key
        /// </summary>
        /// <param name="newEncryptionPrivateKey">New encryption private key</param>
        public static void ChangeEncryptionPrivateKey(string newEncryptionPrivateKey)
        {
            if (String.IsNullOrEmpty(newEncryptionPrivateKey) || newEncryptionPrivateKey.Length != 16)
                throw new NopException("Encryption private key must be 16 characters long");

            string oldEncryptionPrivateKey = SettingManager.GetSettingValue("Security.EncryptionPrivateKey");

            if (oldEncryptionPrivateKey == newEncryptionPrivateKey)
                return;

            var orders = OrderManager.LoadAllOrders();
            //uncomment this line to support transactions
            //using (var scope = new System.Transactions.TransactionScope())
            {
                foreach (var order in orders)
                {
                    string decryptedCardType = Decrypt(order.CardType, oldEncryptionPrivateKey);
                    string decryptedCardName = Decrypt(order.CardName, oldEncryptionPrivateKey);
                    string decryptedCardNumber = Decrypt(order.CardNumber, oldEncryptionPrivateKey);
                    string decryptedMaskedCreditCardNumber = Decrypt(order.MaskedCreditCardNumber, oldEncryptionPrivateKey);
                    string decryptedCardCvv2 = Decrypt(order.CardCvv2, oldEncryptionPrivateKey);
                    string decryptedCardExpirationMonth = Decrypt(order.CardExpirationMonth, oldEncryptionPrivateKey);
                    string decryptedCardExpirationYear = Decrypt(order.CardExpirationYear, oldEncryptionPrivateKey);

                    string encryptedCardType = Encrypt(decryptedCardType, newEncryptionPrivateKey);
                    string encryptedCardName = Encrypt(decryptedCardName, newEncryptionPrivateKey);
                    string encryptedCardNumber = Encrypt(decryptedCardNumber, newEncryptionPrivateKey);
                    string encryptedMaskedCreditCardNumber = Encrypt(decryptedMaskedCreditCardNumber, newEncryptionPrivateKey);
                    string encryptedCardCvv2 = Encrypt(decryptedCardCvv2, newEncryptionPrivateKey);
                    string encryptedCardExpirationMonth = Encrypt(decryptedCardExpirationMonth, newEncryptionPrivateKey);
                    string encryptedCardExpirationYear = Encrypt(decryptedCardExpirationYear, newEncryptionPrivateKey);

                    OrderManager.UpdateOrder(order.OrderId, order.OrderGuid, order.CustomerId,
                       order.CustomerLanguageId, order.CustomerTaxDisplayType, order.CustomerIP,
                       order.OrderSubtotalInclTax, order.OrderSubtotalExclTax, order.OrderShippingInclTax,
                       order.OrderShippingExclTax, order.PaymentMethodAdditionalFeeInclTax, order.PaymentMethodAdditionalFeeExclTax,
                       order.TaxRates, order.OrderTax, order.OrderTotal, order.OrderDiscount,
                       order.OrderSubtotalInclTaxInCustomerCurrency, order.OrderSubtotalExclTaxInCustomerCurrency,
                       order.OrderShippingInclTaxInCustomerCurrency, order.OrderShippingExclTaxInCustomerCurrency,
                       order.PaymentMethodAdditionalFeeInclTaxInCustomerCurrency, order.PaymentMethodAdditionalFeeExclTaxInCustomerCurrency,
                       order.TaxRatesInCustomerCurrency, order.OrderTaxInCustomerCurrency, order.OrderTotalInCustomerCurrency,
                       order.OrderDiscountInCustomerCurrency,
                       order.CheckoutAttributeDescription, order.CheckoutAttributesXml,
                       order.CustomerCurrencyCode, order.OrderWeight,
                       order.AffiliateId, order.OrderStatus, order.AllowStoringCreditCardNumber,
                       encryptedCardType, encryptedCardName, encryptedCardNumber,
                       encryptedMaskedCreditCardNumber, encryptedCardCvv2, encryptedCardExpirationMonth, encryptedCardExpirationYear,
                       order.PaymentMethodId, order.PaymentMethodName, order.AuthorizationTransactionId,
                       order.AuthorizationTransactionCode, order.AuthorizationTransactionResult,
                       order.CaptureTransactionId, order.CaptureTransactionResult,
                       order.SubscriptionTransactionId, order.PurchaseOrderNumber,
                       order.PaymentStatus, order.PaidDate, order.BillingFirstName, order.BillingLastName, order.BillingPhoneNumber,
                       order.BillingEmail, order.BillingFaxNumber, order.BillingCompany, order.BillingAddress1,
                       order.BillingAddress2, order.BillingCity,
                       order.BillingStateProvince, order.BillingStateProvinceId, order.BillingZipPostalCode,
                       order.BillingCountry, order.BillingCountryId, order.ShippingStatus,
                       order.ShippingFirstName, order.ShippingLastName, order.ShippingPhoneNumber,
                       order.ShippingEmail, order.ShippingFaxNumber, order.ShippingCompany,
                       order.ShippingAddress1, order.ShippingAddress2, order.ShippingCity,
                       order.ShippingStateProvince, order.ShippingStateProvinceId, order.ShippingZipPostalCode,
                       order.ShippingCountry, order.ShippingCountryId,
                       order.ShippingMethod, order.ShippingRateComputationMethodId,
                       order.ShippedDate, order.DeliveryDate, order.TrackingNumber, 
                       order.VatNumber, order.Deleted, order.CreatedOn);
                }

                SettingManager.SetParam("Security.EncryptionPrivateKey", newEncryptionPrivateKey);

                //uncomment this line to support transactions
                //scope.Complete();
            }

        }

        #endregion
    }
}
