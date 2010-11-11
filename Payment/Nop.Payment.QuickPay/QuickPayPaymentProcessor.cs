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
using System.Security.Cryptography;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Security;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Payment.Methods.QuickPay
{
    /// <summary>
    /// QuickPay payment processor
    /// </summary>
    public class QuickPayPaymentProcessor : IPaymentMethod
    {
        #region Utilities

        private string FormatOrderNumber(string ordernumber)
        {
            //order number must be at least 4 digits long.
            if (ordernumber.Length < 4)
            {
                for (int i = 1; i < 4; i++)
                {
                    if (ordernumber.Length < 4)
                    {
                        ordernumber = "0" + ordernumber;
                    }
                }
            }

            return ordernumber;
        }
        //This only works when a payment have been authorized and not captured.
        private void DoCancel(Order order, ref CancelPaymentResult cancelPaymentResult, bool alreadyTriedRefund)
        {
            string merchant = IoC.Resolve<ISettingManager>().GetSettingValue(QuickPayConstants.SETTING_MERCHANTID);
            string protocol = "3";
            string capturePostUrl = "https://secure.quickpay.dk/api";
            string msgtype = "cancel";
            string transaction = order.AuthorizationTransactionId;
            string md5secret = IoC.Resolve<ISettingManager>().GetSettingValue(QuickPayConstants.SETTING_MD5SECRET);
            string stringToMd5 = string.Concat(protocol, msgtype, merchant, transaction, md5secret);


            string querystring = string.Empty;
            string md5check = GetMD5(stringToMd5);

            querystring += string.Format("protocol={0}&", protocol);
            querystring += string.Format("msgtype={0}&", msgtype);
            querystring += string.Format("merchant={0}&", merchant);
            querystring += string.Format("transaction={0}&", transaction);
            querystring += string.Format("md5check={0}", md5check);

            string retval = HttpRequestsFunctions.HttpPost(capturePostUrl, querystring);


            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(retval);
                XmlElement xmlElement = xmlDocument.DocumentElement;

                string rep_qpstatmsg = xmlElement.SelectSingleNode("qpstatmsg").InnerText;
                string rep_qpstat = xmlElement.SelectSingleNode("qpstat").InnerText;
                string rep_transaction = xmlElement.SelectSingleNode("transaction").InnerText;

                //refund successful
                if (rep_qpstat == "000")
                {
                    cancelPaymentResult.PaymentStatus = PaymentStatusEnum.Refunded;
                }
                else if (rep_qpstat == "004" && (!alreadyTriedRefund))
                {
                    DoRefund(order, ref cancelPaymentResult, true);
                }
                else
                {
                    cancelPaymentResult.Error = "Quickpay Cancel did not succeed, qpstat is:" + rep_qpstat;
                    cancelPaymentResult.FullError = "Quickpay Cancel did not succeed, qpstat is:" + rep_qpstat;
                }

            }
            catch (Exception exception)
            {
                throw new NopException("XML response for Quickpay Capture was not successfull. Reasons could be that the host did not respond. Below is stacktrace:" + exception.Message + exception.StackTrace + exception.Source, exception.InnerException);
            }

        }

        //This works only on refund to customer (refund what has already been captured...)
        private void DoRefund(Order order, ref CancelPaymentResult cancelPaymentResult, bool alreadyTriedCancel)
        {
            string merchant = IoC.Resolve<ISettingManager>().GetSettingValue(QuickPayConstants.SETTING_MERCHANTID);
            string protocol = "3";
            string capturePostUrl = "https://secure.quickpay.dk/api";
            string msgtype = "refund";
            string amount = (cancelPaymentResult.Amount * 100).ToString("0", CultureInfo.InvariantCulture); //NOTE: Primary store should be changed to DKK, if you do not have internatinal agreement with pbs and quickpay. Otherwise you need to do currency conversion here.
            string transaction = order.AuthorizationTransactionId;
            string md5secret = IoC.Resolve<ISettingManager>().GetSettingValue(QuickPayConstants.SETTING_MD5SECRET);
            string stringToMd5 = string.Concat(protocol, msgtype, merchant, amount, transaction, md5secret);


            string querystring = string.Empty;
            string md5check = GetMD5(stringToMd5);

            querystring += string.Format("protocol={0}&", protocol);
            querystring += string.Format("msgtype={0}&", msgtype);
            querystring += string.Format("merchant={0}&", merchant);
            querystring += string.Format("amount={0}&", amount);
            querystring += string.Format("transaction={0}&", transaction);
            querystring += string.Format("md5check={0}", md5check);

            string retval = HttpRequestsFunctions.HttpPost(capturePostUrl, querystring);


            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(retval);
                XmlElement xmlElement = xmlDocument.DocumentElement;

                string rep_qpstatmsg = xmlElement.SelectSingleNode("qpstatmsg").InnerText;
                string rep_qpstat = xmlElement.SelectSingleNode("qpstat").InnerText;
                string rep_transaction = xmlElement.SelectSingleNode("transaction").InnerText;

                //refund successful
                if (rep_qpstat == "000")
                {
                    cancelPaymentResult.PaymentStatus = PaymentStatusEnum.Refunded;
                }
                //not allowed in current state. This probably means that it has not been caputered yet.
                //we therefore try to just cancel, but not refund
                else if (rep_qpstat == "004" && (!alreadyTriedCancel))
                {
                    DoCancel(order, ref cancelPaymentResult, true);
                }
                else
                {
                    cancelPaymentResult.Error = "Quickpay Caputure refund did not succeed, qpstat is:" + rep_qpstat;
                    cancelPaymentResult.FullError = "Quickpay Caputure refund did not succeed, qpstat is:" + rep_qpstat;
                }

            }
            catch (Exception exception)
            {
                throw new NopException("XML response for Quickpay Capture was not successfull. Reasons could be that the host did not respond. Below is stacktrace:" + exception.Message + exception.StackTrace + exception.Source, exception.InnerException);
            }

        }
        #endregion
        
        #region Methods

        public string GetMD5(string InputStr)
        {
            byte[] textBytes = Encoding.Default.GetBytes(InputStr);
            try
            {
                MD5CryptoServiceProvider cryptHandler = new MD5CryptoServiceProvider();
                byte[] hash = cryptHandler.ComputeHash(textBytes);
                string ret = "";
                foreach (byte a in hash)
                {
                    if (a < 16)
                        ret += "0" + a.ToString("x");
                    else
                        ret += a.ToString("x");
                }
                return ret;
            }
            catch
            {
                throw;
            }
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
            CultureInfo cultureInfo = new CultureInfo(NopContext.Current.WorkingLanguage.LanguageCulture);

            string language = cultureInfo.TwoLetterISOLanguageName;
            string amount = (order.OrderTotal * 100).ToString("0", CultureInfo.InvariantCulture); //NOTE: Primary store should be changed to DKK, if you do not have internatinal agreement with pbs and quickpay. Otherwise you need to do currency conversion here.

            string currencyCode = IoC.Resolve<ICurrencyService>().PrimaryStoreCurrency.CurrencyCode; //NOTE: Primary store should be changed to DKK, if you do not have internatinal agreement with pbs and quickpay. Otherwise you need to do currency conversion here.
            string protocol = "3";
            string autocapture = "0";
            string cardtypelock = IoC.Resolve<ISettingManager>().GetSettingValue(QuickPayConstants.SETTING_CREDITCARD_CODE_PROPERTY, "dankort");
            bool useSandBox = IoC.Resolve<ISettingManager>().GetSettingValueBoolean(QuickPayConstants.SETTING_USE_SANDBOX);
            string testmode = (useSandBox) ? "1" : "0";
            string continueurl = CommonHelper.GetStoreLocation(false) + "CheckoutCompleted.aspx";
            string cancelurl = CommonHelper.GetStoreLocation(false) + "QuickpayCancel.aspx";
            string callbackurl = CommonHelper.GetStoreLocation(false) + "QuickpayReturn.aspx";
            string merchant = IoC.Resolve<ISettingManager>().GetSettingValue(QuickPayConstants.SETTING_MERCHANTID);
            string ipaddress = NopContext.Current.UserHostAddress;
            string msgtype = "authorize";
            string md5secret = IoC.Resolve<ISettingManager>().GetSettingValue(QuickPayConstants.SETTING_MD5SECRET);
            string ordernumber = FormatOrderNumber(order.OrderId.ToString());

            string stringToMd5 = string.Concat(protocol, msgtype, merchant,
                language, ordernumber, amount, currencyCode, continueurl, 
                cancelurl, callbackurl, autocapture, cardtypelock, 
                ipaddress, testmode, md5secret);
            string md5check = GetMD5(stringToMd5);

            RemotePost remotePostHelper = new RemotePost();
            remotePostHelper.FormName = "QuickPay";
            remotePostHelper.Url = "https://secure.quickpay.dk/form/";
            remotePostHelper.Add("protocol", protocol);
            remotePostHelper.Add("msgtype", msgtype);
            remotePostHelper.Add("merchant", merchant);
            remotePostHelper.Add("language", language);
            remotePostHelper.Add("ordernumber", ordernumber);
            remotePostHelper.Add("amount", amount);
            remotePostHelper.Add("currency", currencyCode);
            remotePostHelper.Add("continueurl", continueurl);
            remotePostHelper.Add("cancelurl", cancelurl);
            remotePostHelper.Add("callbackurl", callbackurl);
            remotePostHelper.Add("autocapture", autocapture);
            remotePostHelper.Add("cardtypelock", cardtypelock);
            remotePostHelper.Add("testmode", testmode);
            remotePostHelper.Add("ipaddress", ipaddress);
            remotePostHelper.Add("md5check", md5check);


            remotePostHelper.Post();
            return string.Empty;
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee()
        {
            return IoC.Resolve<ISettingManager>().GetSettingValueDecimalNative("PaymentMethod.QuickPay.AdditionalFee");
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="processPaymentResult">Process payment result</param>
        public void Capture(Order order, ref ProcessPaymentResult processPaymentResult)
        {
            bool useSandBox = IoC.Resolve<ISettingManager>().GetSettingValueBoolean(QuickPayConstants.SETTING_USE_SANDBOX);
            string merchant = IoC.Resolve<ISettingManager>().GetSettingValue(QuickPayConstants.SETTING_MERCHANTID);
            string protocol = "3";
            string testmode = (useSandBox) ? "1" : "0";
            const string autocapture = "0"; //in initial phase while testing system, no autocapture!.
            string capturePostUrl = "https://secure.quickpay.dk/api";
            string msgtype = "capture";
            string ordernumber = FormatOrderNumber(order.OrderId.ToString());
            string currencyCode = IoC.Resolve<ICurrencyService>().PrimaryStoreCurrency.CurrencyCode; //NOTE: Primary store should be changed to DKK, if you do not have internatinal agreement with pbs and quickpay. Otherwise you need to do currency conversion here.
            string amount = (order.OrderTotal * 100).ToString("0", CultureInfo.InvariantCulture); //NOTE: Primary store should be changed to DKK, if you do not have internatinal agreement with pbs and quickpay. Otherwise you need to do currency conversion here.
            string transaction = order.AuthorizationTransactionId;
            string md5secret = IoC.Resolve<ISettingManager>().GetSettingValue(QuickPayConstants.SETTING_MD5SECRET);
            string stringToMd5 = string.Concat(protocol, msgtype, merchant, amount, autocapture, transaction, md5secret);
            
            string querystring = string.Empty;
            string md5check = GetMD5(stringToMd5);
            querystring += string.Format("protocol={0}&", protocol);
            querystring += string.Format("msgtype={0}&", msgtype);
            querystring += string.Format("merchant={0}&", merchant);
            querystring += string.Format("ordernumber={0}&", ordernumber);
            querystring += string.Format("amount={0}&", amount);
            querystring += string.Format("currency={0}&", currencyCode);
            querystring += string.Format("autocapture={0}&", autocapture);
            querystring += string.Format("transaction={0}&", transaction);
            querystring += string.Format("md5check={0}", md5check);
            querystring += string.Format("testmode={0}", testmode);
            
            string retval = HttpRequestsFunctions.HttpPost(capturePostUrl, querystring);

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(retval);
                XmlElement xmlElement = xmlDocument.DocumentElement;

                string rep_qpstatmsg = xmlElement.SelectSingleNode("qpstatmsg").InnerText;
                string rep_qpstat = xmlElement.SelectSingleNode("qpstat").InnerText;
                string rep_transaction = xmlElement.SelectSingleNode("transaction").InnerText;

                //caputre successful
                if (rep_qpstat == "000")
                {
                    if (IoC.Resolve<IOrderService>().CanMarkOrderAsPaid(order))
                    {
                        IoC.Resolve<IOrderService>().MarkOrderAsPaid(order.OrderId);
                        processPaymentResult.PaymentStatus = PaymentStatusEnum.Paid;
                    }
                    //IoC.Resolve<IOrderService>().SetCaptureResults(order.OrderId, rep_transaction, rep_qpstatmsg);
                }
                else
                {
                    processPaymentResult.Error = "Quickpay Caputure did not succeed, qpstat is:" + rep_qpstat;
                    processPaymentResult.FullError = "Quickpay Caputure did not succeed, qpstat is:" + rep_qpstat;
                }


                processPaymentResult.CaptureTransactionId = rep_transaction;
                processPaymentResult.CaptureTransactionResult = rep_qpstatmsg;
            }
            catch (Exception exception)
            {
                throw new NopException("XML response for Quickpay Capture was not successfull. Reasons could be that the host did not respond. Below is stacktrace:" + exception.Message + exception.StackTrace + exception.Source, exception.InnerException);
            }
        }

        /// <summary>
        /// Refunds payment
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="cancelPaymentResult">Cancel payment result</param>        
        public void Refund(Order order, ref CancelPaymentResult cancelPaymentResult)
        {
            DoRefund(order, ref cancelPaymentResult, false);
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
                return true;
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
                return true;
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
