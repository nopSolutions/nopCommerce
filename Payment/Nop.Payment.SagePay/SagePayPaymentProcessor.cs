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
// Contributor(s): Stuart Lodge_______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Payment.Methods.SagePay
{
    /// <summary>
    /// SagePay payment processor
    /// </summary>
    public class SagePayPaymentProcessor : IPaymentMethod
    { 
        #region Fields
        private bool useSandBox = true;
        private string partnerID;
        private string vendorName;
        private string vendorDescription;
        private string emailThanksMessage;
        private string encryptionPassword;
        private string protocolNumber;
        private string transactionType = "PAYMENT"; // '** This can be DEFERRED or AUTHENTICATE if your Sage Pay account supports those payment types **
        public bool sendEmails = false;
        public bool applyCVS = false;
        public bool apply3DS = false;
        #endregion

        #region Ctor
        /// <summary>
        /// Creates a new instance of the WorldpayPaymentProcessor class
        /// </summary>
        public SagePayPaymentProcessor()
        {
            useSandBox = SettingManager.GetSettingValueBoolean("PaymentMethod.SagePay.UseSandbox", false);
            partnerID = SettingManager.GetSettingValue("PaymentMethod.SagePay.PartnerID");
            vendorName = SettingManager.GetSettingValue("PaymentMethod.SagePay.VendorName");
            vendorDescription = SettingManager.GetSettingValue("PaymentMethod.SagePay.VendorDescription");
            sendEmails = SettingManager.GetSettingValueBoolean("PaymentMethod.SagePay.SendEmails", false);
            emailThanksMessage = SettingManager.GetSettingValue("PaymentMethod.SagePay.EmailThanksMessage");
            applyCVS = SettingManager.GetSettingValueBoolean("PaymentMethod.SagePay.ApplyCVS", true);
            apply3DS = SettingManager.GetSettingValueBoolean("PaymentMethod.SagePay.Apply3DS", true);
            encryptionPassword = SettingManager.GetSettingValue("PaymentMethod.SagePay.EncryptionPassword");
            protocolNumber = SettingManager.GetSettingValue("PaymentMethod.SagePay.ProtocolNumber");

            if (string.IsNullOrEmpty(partnerID))
                partnerID = string.Empty; // this is OK - just means we don't have a partner id
            if (string.IsNullOrEmpty(protocolNumber))
                protocolNumber = "2.23";
        }
        #endregion

        #region Utilities

        /// <summary>
        /// Gets SagePay URL
        /// </summary>
        /// <returns></returns>
        private string GetSagePayUrl()
        {
            if (useSandBox)
                return "https://test.sagepay.com/simulator/vspformgateway.asp";

            return "https://live.sagepay.com/gateway/service/vspform-register.vsp";
        }

        private string GenerateCryptField(Order order)
        {
            if (string.IsNullOrEmpty(encryptionPassword))
                throw new NopException("SagePay Encryption Password is not set");

            StringBuilder cryptBuilder = new StringBuilder();

            cryptBuilder.AppendFormat("VendorTxCode={0}", order.OrderId.ToString("N"));
            cryptBuilder.AppendFormat("&ReferrerID={0}", partnerID);
            cryptBuilder.AppendFormat("&Amount={0:0.00}", order.OrderTotal); // FormatNumber(order.OrderTotal, 2, -1, 0, 0)); // ** Formatted to 2 decimal places with leading digit **
            cryptBuilder.AppendFormat("&Currency={0}", CurrencyManager.PrimaryStoreCurrency.CurrencyCode);
            cryptBuilder.AppendFormat("&Description={0}", vendorDescription); // ** Up to 100 chars of free format description **

            // ** The SuccessURL is the page to which Form returns the customer if the transaction is successful **
            // ** You can change this for each transaction, perhaps passing a session ID or state flag if you wish **
            string successReturnURL = CommonHelper.GetStoreLocation(false) + "SagePaySuccess.aspx";
            cryptBuilder.AppendFormat("&SuccessURL={0}", successReturnURL);

            // ** The FailureURL is the page to which Form returns the customer if the transaction is unsuccessful **
            // ** You can change this for each transaction, perhaps passing a session ID or state flag if you wish **
            string failureReturnURL = CommonHelper.GetStoreLocation(false) + "SagePayFailure.aspx";
            cryptBuilder.AppendFormat("&FailureURL={0}", failureReturnURL);

            // ** Pass the Customer's name for use within confirmation emails and the Sage Pay Admin area.
            cryptBuilder.AppendFormat("&CustomerName={0} {1}", order.BillingFirstName, order.BillingLastName);
            cryptBuilder.AppendFormat("&CustomerEMail={0}", order.BillingEmail);
            cryptBuilder.AppendFormat("&VendorEMail={0}", MessageManager.AdminEmailAddress);

            // SendEMail ** Optional setting. 0 = Do not send either customer or vendor e-mails, 1 = Send customer and vendor e-mails if address(es) are provided(DEFAULT). 
            cryptBuilder.AppendFormat("&SendEMail={0}", sendEmails ? "1" : "0");
            // '** You can specify any custom message to send to your customers in their confirmation e-mail here **
            // '** The field can contain HTML if you wish, and be different for each order.  The field is optional **
            cryptBuilder.AppendFormat("&eMailMessage={0}", HttpUtility.UrlEncode(emailThanksMessage));

            // ** Populate Customer Details for crypt string
            // ** Billing Details
            cryptBuilder.AppendFormat("&BillingSurname={0}", order.BillingLastName);
            cryptBuilder.AppendFormat("&BillingFirstnames={0}", order.BillingFirstName);
            cryptBuilder.AppendFormat("&BillingAddress1={0}", order.BillingAddress1);
            if (!string.IsNullOrEmpty(order.BillingAddress1))
                cryptBuilder.AppendFormat("&BillingAddress2={0}", order.BillingAddress2);
            cryptBuilder.AppendFormat("&BillingCity={0}", order.BillingCity);
            cryptBuilder.AppendFormat("&BillingPostCode={0}", order.BillingZipPostalCode);
            var billingCountryCode = CountryManager.GetCountryById(order.BillingCountryId).TwoLetterIsoCode;
            cryptBuilder.AppendFormat("&BillingCountry={0}", billingCountryCode);
            if (!string.IsNullOrEmpty(order.BillingStateProvince))
            {
                if (billingCountryCode == "US")
                {
                    var stateProvince = StateProvinceManager.GetStateProvinceById(order.BillingStateProvinceId);
                    if (stateProvince != null)
                    {
                        cryptBuilder.AppendFormat("&BillingState={0}", stateProvince.Abbreviation);
                    }
                }
            }
            if (!string.IsNullOrEmpty(order.BillingPhoneNumber))
                cryptBuilder.AppendFormat("&BillingPhone={0}", order.BillingPhoneNumber);

            if (order.ShippingStatus != ShippingStatusEnum.ShippingNotRequired)
            {
                cryptBuilder.AppendFormat("&DeliverySurname={0}", order.ShippingLastName);
                cryptBuilder.AppendFormat("&DeliveryFirstnames={0}", order.ShippingFirstName);
                cryptBuilder.AppendFormat("&DeliveryAddress1={0}", order.ShippingAddress1);
                if (!string.IsNullOrEmpty(order.ShippingAddress2))
                    cryptBuilder.AppendFormat("&DeliveryAddress2={0}", order.ShippingAddress2);
                cryptBuilder.AppendFormat("&DeliveryCity={0}", order.ShippingCity);
                cryptBuilder.AppendFormat("&DeliveryPostCode={0}", order.ShippingZipPostalCode);
                var shippingCountryCode = CountryManager.GetCountryById(order.ShippingCountryId).TwoLetterIsoCode;
                cryptBuilder.AppendFormat("&DeliveryCountry={0}", shippingCountryCode);
                if (!string.IsNullOrEmpty(order.ShippingStateProvince))
                {
                    if (shippingCountryCode == "US")
                    {
                        var stateProvince = StateProvinceManager.GetStateProvinceById(order.ShippingStateProvinceId);
                        if (stateProvince != null)
                        {
                            cryptBuilder.AppendFormat("&DeliveryState={0}", stateProvince.Abbreviation);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(order.ShippingPhoneNumber))
                    cryptBuilder.AppendFormat("&DeliveryPhone={0}", order.ShippingPhoneNumber);
            }

            cryptBuilder.AppendFormat("&Basket={0}", CreateBasketText(order));

            // ** For charities registered for Gift Aid, set to 1 to display the Gift Aid check box on the payment pages **
            // gift aid fixed to false
            cryptBuilder.Append("&AllowGiftAid=0");

            // ** Allow fine control over AVS/CV2 checks and rules by changing this value. 0 is Default **
            // ** It can be changed dynamically, per transaction, if you wish.  See the Server Protocol document **
            cryptBuilder.AppendFormat("&ApplyAVSCV2={0}", applyCVS ? "1" : "0");

            // ** Allow fine control over 3D-Secure checks and rules by changing this value. 0 is Default **
            // ** It can be changed dynamically, per transaction, if you wish.  See the Server Protocol document **
            cryptBuilder.AppendFormat("&Apply3DSecure={0}", apply3DS ? "1" : "0");

            string fullPlainTextCrypt = cryptBuilder.ToString();
            byte[] xOrCryptArray = SimpleXor(fullPlainTextCrypt, encryptionPassword);
            string base64XOrCrypt = Convert.ToBase64String(xOrCryptArray);

            return base64XOrCrypt;
        }

        private static string CreateBasketText(Order order)
        {
            StringBuilder toReturn = new StringBuilder();

            // final bit - here we go...
            int numberOfItemsToList = order.OrderProductVariants.Count;
            if (order.OrderShippingInclTax > 0.0M)
                numberOfItemsToList++;

            toReturn.AppendFormat("{0}", numberOfItemsToList);

            foreach (var item in order.OrderProductVariants)
            {
                //'** Extract the Quantity and Product from the list of "x of y," entries in the cart **
                //intQuantity = cleanInput(Left(strThisEntry, 1), "Number")
                //intProductID = cleanInput(Mid(strThisEntry, 6, InStr(strThisEntry, ",") - 6), "Number")

                //'** Add another item to our Form basket **
                //intBasketItems = intBasketItems + 1
                string name = item.ProductVariant.LocalizedFullProductName;
                if (string.IsNullOrEmpty(name))
                    name = "anonymous product";

                toReturn.AppendFormat(":{0}:{1}", name, item.Quantity);
                toReturn.AppendFormat(":{0:0.00}", item.UnitPriceExclTax);
                toReturn.AppendFormat(":{0:0.00}", item.UnitPriceInclTax - item.UnitPriceExclTax);
                toReturn.AppendFormat(":{0:0.00}", item.UnitPriceInclTax);
                toReturn.AppendFormat(":{0:0.00}", item.PriceInclTax);
            }

            if (order.OrderShippingInclTax > 0.0M)
            {
                toReturn.AppendFormat(":Delivery:1:{0:0.00}:{1:0.00}:{2:0.00}:{3:0.00}",
                    order.OrderShippingExclTax,
                    order.OrderShippingInclTax - order.OrderShippingExclTax,
                    order.OrderShippingInclTax,
                    order.OrderShippingInclTax
                    );
            }
            return toReturn.ToString();
        }

        //' ** The SimpleXor encryption algorithm. **
        //' ** NOTE: This is a placeholder really.  Future releases of Sage Pay Form will use AES **
        //' ** This simple function and the Base64 will deter script kiddies and prevent the "View Source" type tampering **
        private static byte[] SimpleXor(string input, string passKey)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(passKey))
                throw new NopException("Internal error - invalid input for SimpleXor");

            int iInputIndex = 0;
            int iKeyIndex = 0;
            byte[] toReturn = new byte[input.Length];

            // ** Step through the plain text source XORing the character at each point with the next character in the key **
            // ** Loop through the key characters as necessary **
            while (iInputIndex < input.Length)
            {
                byte bInput = (byte)input[iInputIndex];
                byte bKey = (byte)passKey[iKeyIndex];
                toReturn[iInputIndex] = (byte)(bInput ^ bKey);
                iInputIndex++;
                iKeyIndex++;
                if (iKeyIndex >= passKey.Length)
                    iKeyIndex = 0;
            }

            return toReturn;
        }

        //' ** The SimpleXor encryption algorithm. **
        //' ** NOTE: This is a placeholder really.  Future releases of Sage Pay Form will use AES **
        //' ** This simple function and the Base64 will deter script kiddies and prevent the "View Source" type tampering **
        private static string SimpleXor(byte[] input, string passKey)
        {
            if (input == null || input.Length == 0 || string.IsNullOrEmpty(passKey))
                throw new NopException("Internal error - invalid input for SimpleXor");

            int iInputIndex = 0;
            int iKeyIndex = 0;
            byte[] toReturn = new byte[input.Length];

            StringBuilder sb = new StringBuilder(input.Length);

            // ** Step through the plain text source XORing the character at each point with the next character in the key **
            // ** Loop through the key characters as necessary **
            while (iInputIndex < input.Length)
            {
                byte bInput = (byte)input[iInputIndex];
                byte bKey = (byte)passKey[iKeyIndex];
                toReturn[iInputIndex] = (byte)(bInput ^ bKey);
                if (toReturn[iInputIndex] == 0)
                    throw new NopException("Null character seen while decoding SimpleXOr");
                sb.Append((char)toReturn[iInputIndex]);
                iInputIndex++;
                iKeyIndex++;
                if (iKeyIndex >= passKey.Length)
                    iKeyIndex = 0;
            }

            return sb.ToString();
        }


        #endregion

        #region Methods

        /// <summary>
        /// Process payment
        /// </summary>
        /// <param name="paymentInfo">Payment info required for an order processing</param>
        /// <param name="customer">Customer</param>
        /// <param name="orderGuid">Unique order identifier</param>
        /// <param name="processPaymentResult">Process payment result</param>
        public void ProcessPayment(PaymentInfo paymentInfo, Customer customer, Guid orderGuid, ref ProcessPaymentResult processPaymentResult)
        {
            processPaymentResult.PaymentStatus = PaymentStatusEnum.Pending;
        }

        /// <summary>
        /// Post process payment (payment gateways that require redirecting)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>The error status, or String.Empty if no errors</returns>
        public string PostProcessPayment(Order order)
        {
            if (string.IsNullOrEmpty(vendorName))
                throw new NopException("SagePay VendorDescription is not set");
            if (string.IsNullOrEmpty(vendorDescription))
                throw new NopException("SagePay VendorDescription is not set");

            RemotePost remotePostHelper = new RemotePost();
            remotePostHelper.FormName = "SagePayForm";
            remotePostHelper.Url = GetSagePayUrl();

            remotePostHelper.Add("VPSProtocol", protocolNumber);
            remotePostHelper.Add("TxType", transactionType);
            remotePostHelper.Add("Vendor", vendorName);
            remotePostHelper.Add("Crypt", GenerateCryptField(order));
            remotePostHelper.Post();
            return string.Empty;
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee()
        {
            return decimal.Zero;
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="processPaymentResult">Process payment result</param>
        public void Capture(Order order, ref ProcessPaymentResult processPaymentResult)
        {
            throw new NopException("Capture method not supported");
        }
        
        /// <summary>
        /// Refunds payment
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="cancelPaymentResult">Cancel payment result</param>        
        public void Refund(Order order, ref CancelPaymentResult cancelPaymentResult)
        {
            throw new NopException("Refund method not supported");
        }

        /// <summary>
        /// Voids payment
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="cancelPaymentResult">Cancel payment result</param>        
        public void Void(Order order, ref CancelPaymentResult cancelPaymentResult)
        {
            throw new NopException("Void method not supported");
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="paymentInfo">Payment info required for an order processing</param>
        /// <param name="customer">Customer</param>
        /// <param name="orderGuid">Unique order identifier</param>
        /// <param name="processPaymentResult">Process payment result</param>
        public void ProcessRecurringPayment(PaymentInfo paymentInfo, Customer customer, Guid orderGuid, ref ProcessPaymentResult processPaymentResult)
        {
            throw new NopException("Recurring payments not supported");
        }

        /// <summary>
        /// Cancels recurring payment
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="cancelPaymentResult">Cancel payment result</param>        
        public void CancelRecurringPayment(Order order, ref CancelPaymentResult cancelPaymentResult)
        {
        }

        public string Decrypt(string inputCrypt)
        {
            try
            {
                if (string.IsNullOrEmpty(encryptionPassword))
                    throw new NopException("SagePay Encryption Password is not set");

                // the base 64 encoded string is put into urls a bit oddly - see https://support.sagepay.com/forum/Topic321-23-1.aspx
                // so we need to correct plusses back to spaces.
                string correctedInputCrypt = inputCrypt.Replace(" ", "+");
                byte[] base64Bytes = Convert.FromBase64String(correctedInputCrypt);
                string postXOrString = SimpleXor(base64Bytes, encryptionPassword);
                return postXOrString;
            }
            catch (Exception exc)
            {
                LogManager.InsertLog(LogTypeEnum.OrderError, "Error seen while decoding SagePay crypt string " + exc.Message, exc);
                return string.Empty;
            }
        }

        private static char[] AmpSplit = new char[] { '&' };
        private static char[] EqualSplit = new char[] { '=' };

        public Dictionary<string, string> Parse(string decrypted)
        {
            var toReturn = new Dictionary<string, string>();

            var splitArray = decrypted.Split(AmpSplit);
            foreach (var keyValuePair in splitArray)
            {
                var innerSplitArray = keyValuePair.Split(EqualSplit, 2);

                // for now we ignore badly formatted kvp pairs
                if (innerSplitArray.Length != 2)
                    continue;

                toReturn[innerSplitArray[0].Trim()] = innerSplitArray[1];
            }

            return toReturn;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool CanCapture
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool CanPartiallyRefund
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool CanRefund
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool CanVoid
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        /// <returns>A recurring payment type of payment method</returns>
        public RecurringPaymentTypeEnum SupportRecurringPayments
        {
            get
            {
                return RecurringPaymentTypeEnum.NotSupported;
            }
        }

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        /// <returns>A payment method type</returns>
        public PaymentMethodTypeEnum PaymentMethodType
        {
            get
            {
                return PaymentMethodTypeEnum.Standard;
            }
        }
        #endregion
    }
}
