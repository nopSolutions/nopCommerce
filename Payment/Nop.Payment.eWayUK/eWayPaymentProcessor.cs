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
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.Payment.Methods.eWayUK
{
    /// <summary>
    /// eWay (UK) payment processor
    /// </summary>
    public class eWayPaymentProcessor : IPaymentMethod
    {
        #region Fields
        private string customerID;
        private string username;
        private string paymentPage;
        #endregion

        #region Ctor
        /// <summary>
        /// Creates a new instance of the eWayPaymentProcessor class
        /// </summary>
        public eWayPaymentProcessor()
        {

        }
        #endregion

        #region Classes
        public class TransactionRequestResult
        {
            public bool Result;
            public string URI;
            public string Error;
        }
        #endregion

        #region Utilities

        /// <summary>
        /// Format the string needed to post to the Generate page
        /// </summary>
        /// <param name="fieldName">eWAY Parameter Name</param>
        /// <param name="value">Value of Parameter</param>
        /// <returns>Formated value for the URL</returns>
        private string Format(string fieldName, string value)
        {
            if (!string.IsNullOrEmpty(value))
                return "&" + fieldName + "=" + value;
            else
                return "";
        }

        /// <summary>
        /// Parse the result of the transaction request and save the appropriate fields in an object to be used later
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private TransactionRequestResult ParseResults(string xml)
        {
            string _currentNode;
            StringReader _sr = new StringReader(xml);
            XmlTextReader _xtr = new XmlTextReader(_sr);
            _xtr.XmlResolver = null;
            _xtr.WhitespaceHandling = WhitespaceHandling.None;

            // get the root node
            _xtr.Read();

            TransactionRequestResult res = new TransactionRequestResult();

            if ((_xtr.NodeType == XmlNodeType.Element) && (_xtr.Name == "TransactionRequest"))
            {
                while (_xtr.Read())
                {
                    if ((_xtr.NodeType == XmlNodeType.Element) && (!_xtr.IsEmptyElement))
                    {
                        _currentNode = _xtr.Name;
                        _xtr.Read();
                        if (_xtr.NodeType == XmlNodeType.Text)
                        {
                            switch (_currentNode)
                            {
                                case "Result":
                                    res.Result = bool.Parse(_xtr.Value);
                                    break;

                                case "URI":
                                    res.URI = _xtr.Value;
                                    break;

                                case "Error":
                                    res.Error = _xtr.Value;
                                    break;
                            }
                        }
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Initializes the eWay payment processor
        /// </summary>
        private void InitSettings()
        {
            customerID = IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.eWayUK.CustomerID");
            username = IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.eWayUK.Username");
            paymentPage = IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.eWayUK.PaymentPage");
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
            InitSettings();
            processPaymentResult.PaymentStatus = PaymentStatusEnum.Pending;
        }

        /// <summary>
        /// Post process payment (payment gateways that require redirecting)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>The error status, or String.Empty if no errors</returns>
        public string PostProcessPayment(Order order)
        {
            InitSettings();

            string resultXML = string.Empty;

            string strPost = "CustomerID=" + customerID;
            strPost += Format("UserName", username);
            //send amounts to the generator in DOLLAR FORM. ie 10.05
            strPost += Format("Amount", order.OrderTotal.ToString("0.00", CultureInfo.InvariantCulture));
            strPost += Format("Currency", IoCFactory.Resolve<ICurrencyManager>().PrimaryStoreCurrency.CurrencyCode);


            // supported languages: 
            // "EN" - English
            // "FR" - French
            // "DE" - German
            // "ES" - Spanish
            // "NL" - Dutch
            strPost += Format("Language", "EN");
            strPost += Format("CustomerFirstName", order.BillingFirstName);
            strPost += Format("CustomerLastName", order.BillingLastName);
            strPost += Format("CustomerAddress", order.BillingAddress1);
            strPost += Format("CustomerCity", order.BillingCity);
            strPost += Format("CustomerState", order.BillingStateProvince);
            strPost += Format("CustomerPostCode", order.BillingZipPostalCode);
            strPost += Format("CustomerCountry", order.BillingCountry);
            strPost += Format("CustomerEmail", order.BillingEmail);
            strPost += Format("CustomerPhone", order.BillingPhoneNumber);
            strPost += Format("InvoiceDescription", order.OrderId.ToString());
            strPost += Format("CancelURL", CommonHelper.GetStoreLocation(false) + "eWayMerchantReturn.aspx");
            strPost += Format("ReturnUrl", CommonHelper.GetStoreLocation(false) + "eWayMerchantReturn.aspx");

            strPost += Format("MerchantReference", order.OrderId.ToString());
            strPost += Format("MerchantInvoice", order.OrderId.ToString());
            strPost += Format("MerchantOption1", order.OrderId.ToString());

            string url = paymentPage + "Request?" + strPost;

            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url);
            objRequest.Method = WebRequestMethods.Http.Get;

            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();

            //get the response from the transaction generate page
            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
            {
                resultXML = sr.ReadToEnd();
                // Close and clean up the StreamReader
                sr.Close();
            }

            //parse the result message
            TransactionRequestResult resultObj = ParseResults(resultXML);

            if (resultObj.Result)
            {
                //redirect the user to the payment page
                HttpContext.Current.Response.Redirect(resultObj.URI);
            }
            else
            {
                throw new NopException(resultObj.Error);
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee()
        {
            return IoCFactory.Resolve<ISettingManager>().GetSettingValueDecimalNative("PaymentMethod.eWayUK.AdditionalFee");
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
