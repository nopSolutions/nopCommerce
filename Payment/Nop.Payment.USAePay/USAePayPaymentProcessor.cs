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
// Contributor(s): Chris Curtis_______. 
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using NopSolutions.NopCommerce.Common;
using USAePayAPI;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Payment.Methods.USAePay
{
    /// <summary>
    /// Represents an USA ePay (integrated) payment gateway
    /// </summary>
    public class USAePayPaymentProcessor : IPaymentMethod
    {
        #region Fields

        private string sourceKey;
        private string pin;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public USAePayPaymentProcessor()
        {
            InitSettings();
        }

        #endregion

        #region Utilities

        private void InitSettings()
        {
            sourceKey = IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.USAePayIntegrated.SourceKey");
            pin = IoC.Resolve<ISettingManager>().GetSettingValue("PaymenthMethod.USAePayIntegrated.Pin");
        }
        
        /// <summary>
        /// Converts the Expiration month and year to the USAePay required format: MMYY
        /// </summary>
        /// <returns>String in MMYY format</returns>
        private string GetCardExp(int ExpireMonth, int ExpireYear)
        {
            return String.Format("{0:D2}{1}", ExpireMonth, ExpireYear.ToString().Substring(2));
        }

        /// <summary>
        /// Gets transaction mode configured by store owner
        /// </summary>
        /// <returns></returns>
        private TransactMode GetCurrentTransactionMode()
        {
            TransactMode transactionModeEnum = TransactMode.Authorize;
            string transactionMode = IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.USAePayIntegrated.TransactionMode");
            if (!String.IsNullOrEmpty(transactionMode))
                transactionModeEnum = (TransactMode)Enum.Parse(typeof(TransactMode), transactionMode);
            return transactionModeEnum;
        }
        #endregion


        /// <summary>
        /// Process payment
        /// </summary>
        /// <param name="paymentInfo">Payment info required for an order processing</param>
        /// <param name="customer">Customer</param>
        /// <param name="OrderGuid">Unique order identifier</param>
        /// <param name="processPaymentResult">Process payment result</param>
        public void ProcessPayment(PaymentInfo paymentInfo, Customer customer, Guid OrderGuid, ref ProcessPaymentResult processPaymentResult)
        {
            TransactMode transactionMode = GetCurrentTransactionMode();

            var usaepay = new USAePayAPI.USAePay();
            usaepay.SourceKey = sourceKey;
            usaepay.Pin = pin;

            usaepay.OrderID = OrderGuid.ToString();
            usaepay.Amount = paymentInfo.OrderTotal;
            usaepay.CardNumber = paymentInfo.CreditCardNumber;
            usaepay.CardHolder = paymentInfo.CreditCardName;
            usaepay.CardExp = GetCardExp(paymentInfo.CreditCardExpireMonth, paymentInfo.CreditCardExpireYear);
            usaepay.BillingStreet = paymentInfo.BillingAddress.Address1;
            usaepay.BillingStreet2 = paymentInfo.BillingAddress.Address2;
            usaepay.BillingCity = paymentInfo.BillingAddress.City;
            if (paymentInfo.BillingAddress.StateProvince != null) usaepay.BillingState = paymentInfo.BillingAddress.StateProvince.Abbreviation;
            usaepay.BillingZip = paymentInfo.BillingAddress.ZipPostalCode;
            usaepay.BillingFirstName = paymentInfo.BillingAddress.FirstName;
            usaepay.BillingLastName = paymentInfo.BillingAddress.LastName;
            usaepay.BillingCompany = paymentInfo.BillingAddress.Company;

            if (paymentInfo.ShippingAddress != null)
            {
                usaepay.ShippingFirstName = paymentInfo.ShippingAddress.FirstName;
                usaepay.ShippingLastName = paymentInfo.ShippingAddress.LastName;
                usaepay.ShippingStreet = paymentInfo.ShippingAddress.Address1;
                usaepay.ShippingStreet2 = paymentInfo.ShippingAddress.Address2;
                usaepay.ShippingCity = paymentInfo.ShippingAddress.City;
                if (paymentInfo.ShippingAddress.StateProvince != null)  usaepay.ShippingState = paymentInfo.ShippingAddress.StateProvince.Abbreviation;
                usaepay.ShippingZip = paymentInfo.ShippingAddress.ZipPostalCode;
                usaepay.ShippingCompany = paymentInfo.ShippingAddress.Company;
            }

            try
            {
                if (transactionMode == TransactMode.Authorize)
                {
                    usaepay.AuthOnly();
                }
                else
                {
                    usaepay.Sale();
                }


                switch (usaepay.ResultCode)
                {
                    case "A":
                        processPaymentResult.PaymentStatus = transactionMode == TransactMode.Authorize ? PaymentStatusEnum.Authorized : PaymentStatusEnum.Paid;
                        processPaymentResult.AVSResult = usaepay.AvsResult;
                        processPaymentResult.AuthorizationTransactionCode = usaepay.AuthCode;
                        processPaymentResult.AuthorizationTransactionId = usaepay.RefNum;
                        break;
                    case "D":
                        processPaymentResult.Error = "Declined: " + usaepay.ErrorMesg;
                        processPaymentResult.FullError = "Declined : " + usaepay.ErrorMesg;
                        break;
                    default:
                        processPaymentResult.Error = "Error during checkout";
                        processPaymentResult.FullError = "Error during checkout: " + usaepay.ErrorMesg;
                        break;
                }
            }
            catch (Exception)
            {
                processPaymentResult.Error = "Error during checkout";
                processPaymentResult.FullError = "Error during checkout";
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

        public decimal GetAdditionalHandlingFee()
        {
            return IoC.Resolve<ISettingManager>().GetSettingValueDecimalNative("PaymentMethod.USAePayIntegrated.AdditionalFee");
        }

        public void Capture(Order order, ref ProcessPaymentResult processPaymentResult)
        {
            var usaepay = new USAePayAPI.USAePay();
            usaepay.SourceKey = sourceKey;
            usaepay.Pin = pin;

            try
            {
                usaepay.Capture(order.AuthorizationTransactionId);

                switch (usaepay.ResultCode)
                {
                    case "A":
                        processPaymentResult.PaymentStatus = PaymentStatusEnum.Paid;
                        processPaymentResult.AVSResult = usaepay.AvsResult;
                        processPaymentResult.AuthorizationTransactionCode = usaepay.AuthCode;
                        processPaymentResult.AuthorizationTransactionId = usaepay.RefNum;
                        break;
                    case "D":
                        processPaymentResult.Error = "Declined: " + usaepay.ErrorMesg;
                        processPaymentResult.FullError = "Declined : " + usaepay.ErrorMesg;
                        break;
                    default:
                        processPaymentResult.Error = "Error during checkout";
                        processPaymentResult.FullError = "Error during checkout: " + usaepay.ErrorMesg;
                        break;
                }
            }
            catch
            {
                processPaymentResult.Error = "Error during capture";
                processPaymentResult.FullError = "Error during capture";
            }
        }

        public void Refund(Order order, ref CancelPaymentResult cancelPaymentResult)
        {
            var usaepay = new USAePayAPI.USAePay();
            usaepay.SourceKey = sourceKey;
            usaepay.Pin = pin;

            try
            {
                usaepay.Refund(order.AuthorizationTransactionId);

                switch (usaepay.ResultCode)
                {
                    case "A":
                        cancelPaymentResult.PaymentStatus = PaymentStatusEnum.Refunded;
                        //cancelPaymentResult.AuthorizationTransactionId = usaepay.RefNum;
                        break;
                    case "D":
                        cancelPaymentResult.Error = "Refund Declined: " + usaepay.ErrorMesg;
                        cancelPaymentResult.FullError = "Refund Declined : " + usaepay.ErrorMesg;
                        break;
                    default:
                        cancelPaymentResult.Error = "Error during refund";
                        cancelPaymentResult.FullError = "Error during refund: " + usaepay.ErrorMesg;
                        break;
                }
            }
            catch
            {
                cancelPaymentResult.Error = "Error during refund";
                cancelPaymentResult.FullError = "Error during refund";
            }
        }

        public void Void(Order order, ref CancelPaymentResult cancelPaymentResult)
        {
            var usaepay = new USAePayAPI.USAePay();
            usaepay.SourceKey = sourceKey;
            usaepay.Pin = pin;

            try
            {
                usaepay.Void(order.AuthorizationTransactionId);

                switch (usaepay.ResultCode)
                {
                    case "A":
                        cancelPaymentResult.PaymentStatus = PaymentStatusEnum.Voided;
                        //cancelPaymentResult.AuthorizationTransactionId = usaepay.RefNum;
                        break;
                    case "D":
                        cancelPaymentResult.Error = "Void Declined: " + usaepay.ErrorMesg;
                        cancelPaymentResult.FullError = "Void Declined : " + usaepay.ErrorMesg;
                        break;
                    default:
                        cancelPaymentResult.Error = "Error during void";
                        cancelPaymentResult.FullError = "Error during void: " + usaepay.ErrorMesg;
                        break;
                }
            }
            catch
            {
                cancelPaymentResult.Error = "Error during void";
                cancelPaymentResult.FullError = "Error during void";
            }
        }

        public void ProcessRecurringPayment(PaymentInfo paymentInfo, Customer customer, Guid OrderGuid, ref ProcessPaymentResult processPaymentResult)
        {
            throw new NopException("Recurring payments not supported");
        }

        public void CancelRecurringPayment(Order order, ref CancelPaymentResult cancelPaymentResult)
        {
        }

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool CanCapture
        {
            get { return true; }
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
            get { return true; }
        }


        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool CanVoid
        {
            get { return true; }
        }


        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        /// <returns>A recurring payment type of payment method</returns>
        public RecurringPaymentTypeEnum SupportRecurringPayments
        {
            get { return RecurringPaymentTypeEnum.NotSupported; }
        }



        /// <summary>
        /// Gets a payment method type
        /// </summary>
        /// <returns>A payment method type</returns>
        public PaymentMethodTypeEnum PaymentMethodType
        {
            get { return PaymentMethodTypeEnum.Standard; }
        }
    }
}
