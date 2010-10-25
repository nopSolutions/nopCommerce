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
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.Common.Xml;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.Payment.Methods.PSIGate
{
    /// <summary>
    /// PSI Gate payment processor
    /// </summary>
    public class PSIGatePaymentProcessor : IPaymentMethod
    {
        #region Fields
        private bool useSandBox = true;
        private string storeID;
        private string passphrase;
        #endregion

        #region Ctor
        /// <summary>
        /// Creates a new instance of the PSIGatePaymentProcessor class
        /// </summary>
        public PSIGatePaymentProcessor()
        {

        }
        #endregion

        #region Methods

        /// <summary>
        /// Initializes the Authorize.NET payment processor
        /// </summary>
        private void InitSettings()
        {
            useSandBox = IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("PaymentMethod.PSIGate.UseSandbox");
            storeID = IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.PSIGate.StoreID");
            passphrase = IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.PSIGate.Passphrase");
        }

        /// <summary>
        /// Gets PSI Gate URL
        /// </summary>
        /// <returns></returns>
        private string GetPSIGateUrl()
        {
            return useSandBox ? "https://dev.psigate.com:7989/Messenger/XMLMessenger" :
                "https://secure.psigate.com:7934/Messenger/XMLMessenger";
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
            InitSettings();

            StringBuilder builder = new StringBuilder();
            builder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            builder.Append("<Order>");
            builder.Append("<StoreID>" +XmlHelper.XmlEncode(storeID) + "</StoreID>");
            builder.Append("<Passphrase>" + XmlHelper.XmlEncode(passphrase) + "</Passphrase>");
            builder.Append("<Bname>" + XmlHelper.XmlEncode(paymentInfo.BillingAddress.FirstName) + "</Bname>");
            builder.Append("<Bcompany>" + XmlHelper.XmlEncode(paymentInfo.BillingAddress.Company) + "</Bcompany>");
            builder.Append("<Baddress1>" + XmlHelper.XmlEncode(paymentInfo.BillingAddress.Address1) + "</Baddress1>");
            builder.Append("<Baddress2>" + XmlHelper.XmlEncode(paymentInfo.BillingAddress.Address2) + "</Baddress2>");
            builder.Append("<Bcity>" + XmlHelper.XmlEncode(paymentInfo.BillingAddress.City) + "</Bcity>");
            if (paymentInfo.BillingAddress.StateProvince != null)
                builder.Append("<Bprovince>" + XmlHelper.XmlEncode(paymentInfo.BillingAddress.StateProvince.Abbreviation) + "</Bprovince>");
            builder.Append("<Bpostalcode>" + XmlHelper.XmlEncode(paymentInfo.BillingAddress.ZipPostalCode) + "</Bpostalcode>");
            if (paymentInfo.BillingAddress.Country != null)
                builder.Append("<Bcountry>" + XmlHelper.XmlEncode(paymentInfo.BillingAddress.Country.TwoLetterIsoCode) + "</Bcountry>");
            builder.Append("<Phone>" + XmlHelper.XmlEncode(paymentInfo.BillingAddress.PhoneNumber) + "</Phone>");
            builder.Append("<Fax>" + XmlHelper.XmlEncode(paymentInfo.BillingAddress.FaxNumber) + "</Fax>");
            builder.Append("<Email>" + XmlHelper.XmlEncode(paymentInfo.BillingAddress.Email) + "</Email>");
            builder.Append("<Comments> </Comments>");
            builder.Append("<CustomerIP>" + XmlHelper.XmlEncode(HttpContext.Current.Request.UserHostAddress) + "</CustomerIP>");
            builder.Append("<Subtotal>" + XmlHelper.XmlEncode(paymentInfo.OrderTotal.ToString("N2", new CultureInfo("en-US", false).NumberFormat)) + "</Subtotal>");
            builder.Append("<PaymentType>CC</PaymentType>");
            builder.Append("<CardAction>0</CardAction>");
            builder.Append("<CardNumber>" + XmlHelper.XmlEncode(paymentInfo.CreditCardNumber) + "</CardNumber>");
            string cardExpMonth = string.Empty;
            if (paymentInfo.CreditCardExpireMonth < 10)
            {
                cardExpMonth = "0" + paymentInfo.CreditCardExpireMonth.ToString();
            }
            else
            {
                cardExpMonth = paymentInfo.CreditCardExpireMonth.ToString();
            }
            builder.Append("<CardExpMonth>" + XmlHelper.XmlEncode(cardExpMonth) + "</CardExpMonth>");
            builder.Append("<CardExpYear>" + XmlHelper.XmlEncode(paymentInfo.CreditCardExpireYear.ToString().Substring(2, 2)) + "</CardExpYear>");
            builder.Append("<CardIDNumber>" + XmlHelper.XmlEncode(paymentInfo.CreditCardCvv2) + "</CardIDNumber>");
            builder.Append("</Order>");
            string orderInfo = builder.ToString();

            byte[] bytes = Encoding.ASCII.GetBytes(orderInfo);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(GetPSIGateUrl());
            request.Method = "POST";
            request.ContentLength = bytes.Length;
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "application/xml; charset=UTF-8";
            request.KeepAlive = false;

            using (Stream str0 = request.GetRequestStream())
                str0.Write(bytes, 0, bytes.Length);

            string xmlResponse = string.Empty;
            try
            {
                using (HttpWebResponse resp1 = (HttpWebResponse)request.GetResponse())
                using (Stream str1 = resp1.GetResponseStream())
                using (StreamReader reader = new StreamReader(str1))
                    xmlResponse = reader.ReadToEnd();
            }
            catch (WebException exc)
            {
                if (exc.Response != null)
                    using (HttpWebResponse resp2 = (HttpWebResponse)exc.Response)
                    using (StreamReader rdr2 = new StreamReader(resp2.GetResponseStream()))
                        xmlResponse = rdr2.ReadToEnd();
            }
            StringReader input = new StringReader(xmlResponse);
            XmlTextReader rdr3 = new XmlTextReader(input);

        Label_1:
            while (rdr3.Read())
            {
                if (!(rdr3.IsStartElement() & (rdr3.Name == "Result")))
                {
                    continue;
                }
                while (rdr3.Read())
                {
                    if ((rdr3.NodeType == XmlNodeType.Element) && (rdr3.Name == "OrderID"))
                    {
                        processPaymentResult.AuthorizationTransactionId = rdr3.ReadElementString("OrderID");
                    }
                    else
                    {
                        if ((rdr3.NodeType == XmlNodeType.Element) && (rdr3.Name == "Approved"))
                        {
                            string approvedString = rdr3.ReadElementString("Approved");
                            if (approvedString == "APPROVED")
                                processPaymentResult.PaymentStatus = PaymentStatusEnum.Paid;

                            continue;
                        }
                        if ((rdr3.NodeType == XmlNodeType.Element) && (rdr3.Name == "ReturnCode"))
                        {
                            processPaymentResult.AuthorizationTransactionCode = rdr3.ReadElementString("ReturnCode");
                            continue;
                        }
                        if ((rdr3.NodeType == XmlNodeType.Element) && (rdr3.Name == "ErrMsg"))
                        {
                            processPaymentResult.Error = rdr3.ReadElementString("ErrMsg");
                            continue;
                        }
                        if ((rdr3.Name == "Result") & (rdr3.NodeType == XmlNodeType.EndElement))
                        {
                            goto Label_1;
                        }
                    }
                }
            }

            if (processPaymentResult.PaymentStatus != PaymentStatusEnum.Paid && string.IsNullOrEmpty(processPaymentResult.Error))
                processPaymentResult.Error = "Unknown PSI Gate error";
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
            return IoCFactory.Resolve<ISettingManager>().GetSettingValueDecimalNative("PaymentMethod.PSIGate.AdditionalFee");
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
