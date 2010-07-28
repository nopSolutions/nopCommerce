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
using System.Text;
using System.Web;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;


namespace NopSolutions.NopCommerce.Payment.Methods.PayPal
{
    /// <summary>
    /// Paypal Standard payment processor
    /// </summary>
    public class PayPalStandardPaymentProcessor : IPaymentMethod
    {
        #region Fields
        private bool useSandBox = true;
        private string businessEmail;
        private string PDTId;
        #endregion

        #region Ctor
        /// <summary>
        /// Creates a new instance of the PayPalStandardPaymentProcessor class
        /// </summary>
        public PayPalStandardPaymentProcessor()
        {
            useSandBox = SettingManager.GetSettingValueBoolean("PaymentMethod.PaypalStandard.UseSandbox");
            businessEmail = SettingManager.GetSettingValue("PaymentMethod.PaypalStandard.BusinessEmail");
            PDTId = SettingManager.GetSettingValue("PaymentMethod.PaypalStandard.PTIIdentityToken");

            if (string.IsNullOrEmpty(businessEmail))
                throw new NopException("Paypal Standard business Email is empty");

            if (string.IsNullOrEmpty(PDTId))
                throw new NopException("Paypal Standard PTD Id is empty");
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Gets Paypal URL
        /// </summary>
        /// <returns></returns>
        private string GetPaypalUrl()
        {
            return useSandBox ? "https://www.sandbox.paypal.com/us/cgi-bin/webscr" :
                "https://www.paypal.com/us/cgi-bin/webscr";
        }

        #endregion

        #region Methods
        /// <summary>
        /// Gets PDT details
        /// </summary>
        /// <param name="tx">TX</param>
        /// <param name="values">Values</param>
        /// <param name="response">Response</param>
        /// <returns>Result</returns>
        public bool GetPDTDetails(string tx, out Dictionary<string, string> values, out string response)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(GetPaypalUrl());
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            string formContent = string.Format("cmd=_notify-synch&at={0}&tx={1}", PDTId, tx);
            req.ContentLength = formContent.Length;

            using (StreamWriter sw = new StreamWriter(req.GetRequestStream(), Encoding.ASCII))
                sw.Write(formContent);

            response = null;
            using (StreamReader sr = new StreamReader(req.GetResponse().GetResponseStream()))
                response = HttpUtility.UrlDecode(sr.ReadToEnd());

            values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            bool firstLine = true, success = false;
            foreach (string l in response.Split('\n'))
            {
                string line = l.Trim();
                if (firstLine)
                {
                    success = line.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase);
                    firstLine = false;
                }
                else
                {
                    int equalPox = line.IndexOf('=');
                    if (equalPox >= 0)
                        values.Add(line.Substring(0, equalPox), line.Substring(equalPox + 1));
                }
            }

            return success;
        }
        
        /// <summary>
        /// Verifies IPN
        /// </summary>
        /// <param name="formString">Form string</param>
        /// <param name="values">Values</param>
        /// <returns>Result</returns>
        public bool VerifyIPN(string formString, out Dictionary<string, string> values)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(GetPaypalUrl());
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            string formContent = string.Format("{0}&cmd=_notify-validate", formString);
            req.ContentLength = formContent.Length;

            using (StreamWriter sw = new StreamWriter(req.GetRequestStream(), Encoding.ASCII))
            {
                sw.Write(formContent);
            }

            string response = null;
            using (StreamReader sr = new StreamReader(req.GetResponse().GetResponseStream()))
            {
                response = HttpUtility.UrlDecode(sr.ReadToEnd());
            }
            bool success = response.Trim().Equals("VERIFIED", StringComparison.OrdinalIgnoreCase);

            values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (string l in formString.Split('&'))
            {
                string line = l.Trim();
                int equalPox = line.IndexOf('=');
                if (equalPox >= 0)
                    values.Add(line.Substring(0, equalPox), line.Substring(equalPox + 1));
            }

            return success;
        }

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
            StringBuilder builder = new StringBuilder();
            string returnURL = CommonHelper.GetStoreLocation(false) + "PaypalPDTHandler.aspx";
            string cancel_returnURL = CommonHelper.GetStoreLocation(false) + "PaypalCancel.aspx";
            builder.Append(GetPaypalUrl());
            builder.AppendFormat("?cmd=_xclick&business={0}", HttpUtility.UrlEncode(businessEmail));
            builder.AppendFormat("&item_name=Order Number {0}", order.OrderId);
            builder.AppendFormat("&custom={0}", order.OrderGuid);
            builder.AppendFormat("&amount={0}", order.OrderTotal.ToString("N", new CultureInfo("en-us")));
            builder.Append(string.Format("&no_note=1&currency_code={0}", HttpUtility.UrlEncode(CurrencyManager.PrimaryStoreCurrency.CurrencyCode)));
            builder.AppendFormat("&invoice={0}", order.OrderId);
            builder.AppendFormat("&rm=2", new object[0]);
            if (order.ShippingStatus != ShippingStatusEnum.ShippingNotRequired)
                builder.AppendFormat("&no_shipping=2", new object[0]);
            else
                builder.AppendFormat("&no_shipping=1", new object[0]);
            builder.AppendFormat("&return={0}&cancel_return={1}", HttpUtility.UrlEncode(returnURL), HttpUtility.UrlEncode(cancel_returnURL));
            builder.AppendFormat("&first_name={0}", HttpUtility.UrlEncode(order.BillingFirstName));
            builder.AppendFormat("&last_name={0}", HttpUtility.UrlEncode(order.BillingLastName));
            builder.AppendFormat("&address1={0}", HttpUtility.UrlEncode(order.BillingAddress1));
            builder.AppendFormat("&address2={0}", HttpUtility.UrlEncode(order.BillingAddress2));
            builder.AppendFormat("&city={0}", HttpUtility.UrlEncode(order.BillingCity));
            StateProvince billingStateProvince = StateProvinceManager.GetStateProvinceById(order.BillingStateProvinceId);
            if (billingStateProvince != null)
                builder.AppendFormat("&state={0}", HttpUtility.UrlEncode(billingStateProvince.Abbreviation));
            else
                builder.AppendFormat("&state={0}", HttpUtility.UrlEncode(order.BillingStateProvince));
            Country billingCountry = CountryManager.GetCountryById(order.BillingCountryId);
            if (billingCountry != null)
                builder.AppendFormat("&country={0}", HttpUtility.UrlEncode(billingCountry.TwoLetterIsoCode));
            else
                builder.AppendFormat("&country={0}", HttpUtility.UrlEncode(order.BillingCountry));
            builder.AppendFormat("&Email={0}", HttpUtility.UrlEncode(order.BillingEmail));
            HttpContext.Current.Response.Redirect(builder.ToString());
            return string.Empty;
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee()
        {
            return SettingManager.GetSettingValueDecimalNative("PaymentMethod.PaypalStandard.AdditionalFee");
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

        #endregion

        #region Properies

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