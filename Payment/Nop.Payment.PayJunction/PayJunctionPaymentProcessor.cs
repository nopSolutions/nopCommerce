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
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.IoC;


namespace NopSolutions.NopCommerce.Payment.Methods.PayJunction
{
    /// <summary>
    /// PayJunction payment processor
    /// </summary>
    public class PayJunctionPaymentProcessor : IPaymentMethod
    {
        #region Fields
        private bool useSandBox = false;
        private string pjlogon = string.Empty;
        private string pjpassword = string.Empty;
        #endregion

        #region Ctor
        /// <summary>
        /// Creates a new instance of the PayJunctionPaymentProcessor class
        /// </summary>
        public PayJunctionPaymentProcessor()
        {

        }
        #endregion

        #region Utilities

        /// <summary>
        /// Gets current transaction mode
        /// </summary>
        /// <returns>Current transaction mode</returns>
        public static TransactMode GetCurrentTransactionMode()
        {
            TransactMode transactionModeEnum = TransactMode.Authorize;
            string transactionMode = IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.PayJunction.TransactionMode");
            if (!String.IsNullOrEmpty(transactionMode))
                transactionModeEnum = (TransactMode)Enum.Parse(typeof(TransactMode), transactionMode);
            return transactionModeEnum;
        }

        /// <summary>
        /// Initializes the eWay payment processor
        /// </summary>
        private void InitSettings()
        {
            useSandBox = IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("PaymentMethod.PayJunction.UseSandbox");
            pjlogon = IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.PayJunction.pjlogon");
            pjpassword = IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.PayJunction.pjpassword");
        }


        /// <summary>
        /// Gets URL
        /// </summary>
        /// <returns></returns>
        private string GetUrl()
        {
            return useSandBox ? "demo.payjunction.com" :
                "payjunction.com";
        }

        // Check the certificate for errors and to make sure it meets your security policy.
        private static bool OnCertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            // Return true if there are no policy errors. The certificate can also be manually verified to make sure it meets your specific policies by interrogating the x509Certificate object.
            if (errors != SslPolicyErrors.None)
            {
                // if there was a problem return false
                return false;
            }
            else
            {
                // if there was not a problem return true
                return true;
            }
        }

        // This method will URL encode the values
        private string Encode(string str_Name, string str_Value)
        {
            string str_ReturnValue = string.Empty;

            try
            {
                str_ReturnValue = HttpUtility.UrlEncode(str_Value);
            }

            catch
            {
                throw new Exception("Encoding failure: Field - " + str_Name + " Value - " + str_Value);
            }
            return str_Name + "=" + str_ReturnValue;
        }// end of the Encode method
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
            InitSettings();
            TransactMode transactionMode = GetCurrentTransactionMode();

            string transactionModeStr = string.Empty;
            if (transactionMode == TransactMode.Authorize)
                transactionModeStr = "AUTHORIZATION";
            else if (transactionMode == TransactMode.AuthorizeAndCapture)
                transactionModeStr = "AUTHORIZATION_CAPTURE";
            else
                throw new NopException("Not supported transaction mode");


            // This is the standard information that is needed to connect to PayJunction
            string server = GetUrl();
            int port = 443;
            SslStream stream = null;

            // Encode Data Values
            string encodedPJLogin = Encode("dc_logon", pjlogon);
            string encodedPJPassword = Encode("dc_password", pjpassword);
            string encodedFirstname = Encode("dc_first_name", paymentInfo.BillingAddress.FirstName);
            string encodedLastname = Encode("dc_last_name", paymentInfo.BillingAddress.LastName);
            string encodedCCNumber = Encode("dc_number", paymentInfo.CreditCardNumber);
            string encodedExpMonth = Encode("dc_expiration_month", paymentInfo.CreditCardExpireMonth.ToString("D2"));
            string encodedExpYear = Encode("dc_expiration_year", paymentInfo.CreditCardExpireYear.ToString().Substring(2, 2));
            string encodedCVVCode = Encode("dc_verification_number", paymentInfo.CreditCardCvv2);
            string encodedAddress = Encode("dc_address", paymentInfo.BillingAddress.Address1);
            string encodedCity = Encode("dc_city", paymentInfo.BillingAddress.City);
            string encodedZipCode = Encode("dc_zipcode", paymentInfo.BillingAddress.ZipPostalCode);
            string encodedTransType = Encode("dc_transaction_type", transactionModeStr);
            string encodedAmount = Encode("dc_transaction_amount", paymentInfo.OrderTotal.ToString("0.00", CultureInfo.InvariantCulture));
            string encodedVersion = Encode("dc_version", "1.2");

            // Concatenate Encoded Transaction String
            string transactioninfo = "POST /quick_link?" + encodedPJLogin + "&" + encodedPJPassword + "&" + encodedFirstname + "&" + encodedLastname + "&" + encodedCCNumber + "&" + encodedExpMonth + "&" + encodedExpYear + "&" + encodedCCNumber + "&" + encodedAddress + "&" + encodedCity + "&" + encodedZipCode + "&" + encodedTransType + "&" + encodedAmount + "&" + encodedVersion + " \r\n\r\n";

            try
            {
                // Instantiate a TcpClient with the server and port 
                TcpClient client = new TcpClient(server, port);
                // Convert the data to send into a byte array
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(transactioninfo);
                // Specify the callback function that will act as the validation delegate. 
                RemoteCertificateValidationCallback callback = new
                    // This lets you inspect the certificate to see if it meets your validation requirements.
                RemoteCertificateValidationCallback(OnCertificateValidation);
                // Instantiate an SslStream with the NetworkStream returned from the TcpClient.
                stream = new SslStream(client.GetStream(), false, callback);
                // As a client, you can authenticate the server and validate the results using the SslStream. 
                // This is the host name of the server you are connecting to, which may or may not be the name used to connect to the server when TcpClient is instantiated.
                stream.AuthenticateAsClient(server);
                // Send the message to the server. 
                stream.Write(data, 0, data.Length);
                // Buffer to hold data returned from the server.
                data = new Byte[2048];
                // Read the response from the server up to the size of the buffer.
                int bytes = stream.Read(data, 0, data.Length);
                Console.WriteLine(bytes);
                // Convert the received bytes into a string
                string responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                // Create an Array "keyValue" that contains the response
                string[] keyValue = responseData.Split(Convert.ToChar(28));
                Dictionary<string, string> keyValueDic = new Dictionary<string, string>();
                foreach (string key in keyValue)
                {
                    string str1 = key.Split(new char[] { '=' })[0];
                    string str2 = key.Split(new char[] { '=' })[1];
                    keyValueDic.Add(str1, str2);
                }

                string dc_response_code = string.Empty;
                if (keyValueDic.ContainsKey("dc_response_code"))
                {
                    dc_response_code = keyValueDic["dc_response_code"];
                } 
                string dc_response_message = string.Empty;
                if (keyValueDic.ContainsKey("dc_response_message"))
                {
                    dc_response_message = keyValueDic["dc_response_message"];
                }

                if (dc_response_code == "00" || dc_response_code == "85")
                {
                    string dc_transaction_id = string.Empty;
                    if (keyValueDic.ContainsKey("dc_transaction_id"))
                    {
                        dc_transaction_id = keyValueDic["dc_transaction_id"];
                    }
                    if (transactionMode == TransactMode.Authorize)
                    {
                        processPaymentResult.PaymentStatus = PaymentStatusEnum.Authorized;
                        processPaymentResult.AuthorizationTransactionId = dc_transaction_id;
                    }
                    else
                    {
                        processPaymentResult.PaymentStatus = PaymentStatusEnum.Paid;
                        processPaymentResult.CaptureTransactionId = dc_transaction_id;
                    }
                }
                else
                {
                    processPaymentResult.Error = string.Format("Error: {0}. {1}", dc_response_message, dc_response_code);
                    processPaymentResult.FullError = string.Format("Error: {0}. {1}", dc_response_message, dc_response_code);
                }
            }
            catch (Exception exc)
            {
                processPaymentResult.Error = string.Format("Error: {0}", exc.Message);
                processPaymentResult.FullError = string.Format("Error: {0}", exc.ToString());
            }
            finally
            {
                // Make sure that the SslStream is closed.
                if (stream != null)
                    stream.Close();
            }
        }

        /// <summary>
        /// Post process payment (payment gateways that require redirecting)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>The error status, or String.Empty if no errors</returns>
        public string PostProcessPayment(Order order)
        {
            return string.Empty;
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee()
        {
            return IoCFactory.Resolve<ISettingManager>().GetSettingValueDecimalNative("PaymentMethod.PayJunction.AdditionalFee");
        }

        /// <summary>
        /// Captures payment (from admin panel)
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

        #endregion

        #region Properies

        /// <summary>
        /// Gets a value indicating whether capture is allowed from admin panel
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