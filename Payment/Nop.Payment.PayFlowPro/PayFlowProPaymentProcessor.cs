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
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using PayPal.Payments.DataObjects;
using PayPal.Payments.Common.Utility;
using PayPal.Payments.Transactions;
using System.Threading;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.BusinessLogic.IoC;


namespace NopSolutions.NopCommerce.Payment.Methods.PayFlowPro
{
    /// <summary>
    /// PayFlow payment processor
    /// </summary>
    public class PayFlowProPaymentProcessor : IPaymentMethod
    {
        #region Fields
        private bool useSandBox = true;
        private string  user;
        private string vendor;
        private string partner;
        private string password;
        #endregion

        #region Ctor
        /// <summary>
        /// Creates a new instance of the PayFlowPaymentProcessor class
        /// </summary>
        public PayFlowProPaymentProcessor()
        {

        }
        #endregion

        #region Utilities
        /// <summary>
        /// Gets Paypal URL
        /// </summary>
        /// <returns></returns>
        private string GetPaypalUrl()
        {
            return useSandBox ? "pilot-payflowpro.verisign.com/transaction" :
                "payflowpro.verisign.com/transaction";
        }

        #endregion

        #region Methods
        /// <summary>
        /// Gets transaction mode configured by store owner
        /// </summary>
        /// <returns></returns>
        private TransactMode GetCurrentTransactionMode()
        {
            TransactMode transactionModeEnum = TransactMode.Authorize;
            string transactionMode = IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.PayFlowPro.TransactionMode");
            if (!String.IsNullOrEmpty(transactionMode))
                transactionModeEnum = (TransactMode)Enum.Parse(typeof(TransactMode), transactionMode);
            return transactionModeEnum;
        }

        /// <summary>
        /// Initializes the PayPalDirectPaymentProcessor
        /// </summary>
        private void InitSettings()
        {
            useSandBox = IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("PaymentMethod.PayFlowPro.UseSandbox");
            user = IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.PayFlowPro.User");
            vendor = IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.PayFlowPro.Vendor");
            partner = IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.PayFlowPro.Partner");
            password = IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.PayFlowPro.Password");
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
            TransactMode transactionMode = GetCurrentTransactionMode();

            //little hack here
            CultureInfo userCulture = Thread.CurrentThread.CurrentCulture;
            NopContext.Current.SetCulture(new CultureInfo("en-US"));
            try
            {
                Invoice invoice = new Invoice();

                BillTo to = new BillTo();
                to.FirstName = paymentInfo.BillingAddress.FirstName;
                to.LastName = paymentInfo.BillingAddress.LastName;
                to.Street = paymentInfo.BillingAddress.Address1;
                to.City = paymentInfo.BillingAddress.City;
                to.Zip = paymentInfo.BillingAddress.ZipPostalCode;
                if (paymentInfo.BillingAddress.StateProvince != null)
                    to.State = paymentInfo.BillingAddress.StateProvince.Abbreviation;
                invoice.BillTo = to;

                if (paymentInfo.ShippingAddress != null)
                {
                    ShipTo to2 = new ShipTo();
                    to2.ShipToFirstName = paymentInfo.ShippingAddress.FirstName;
                    to2.ShipToLastName = paymentInfo.ShippingAddress.LastName;
                    to2.ShipToStreet = paymentInfo.ShippingAddress.Address1;
                    to2.ShipToCity = paymentInfo.ShippingAddress.City;
                    to2.ShipToZip = paymentInfo.ShippingAddress.ZipPostalCode;
                    if (paymentInfo.ShippingAddress.StateProvince != null)
                        to2.ShipToState = paymentInfo.ShippingAddress.StateProvince.Abbreviation;
                    invoice.ShipTo = to2;
                }                
                
                invoice.InvNum = orderGuid.ToString();
                decimal orderTotal = Math.Round(paymentInfo.OrderTotal, 2);
                invoice.Amt = new PayPal.Payments.DataObjects.Currency(orderTotal, IoCFactory.Resolve<ICurrencyManager>().PrimaryStoreCurrency.CurrencyCode);

                string creditCardExp = string.Empty;
                if (paymentInfo.CreditCardExpireMonth < 10)
                {
                    creditCardExp = "0" + paymentInfo.CreditCardExpireMonth.ToString();
                }
                else
                {
                    creditCardExp = paymentInfo.CreditCardExpireMonth.ToString();
                }
                creditCardExp = creditCardExp + paymentInfo.CreditCardExpireYear.ToString().Substring(2, 2);
                CreditCard credCard = new CreditCard(paymentInfo.CreditCardNumber, creditCardExp);
                credCard.Cvv2 = paymentInfo.CreditCardCvv2;
                CardTender tender = new CardTender(credCard);
                // <vendor> = your merchant (login id)  
                // <user> = <vendor> unless you created a separate <user> for Payflow Pro
                // partner = paypal
                UserInfo userInfo = new UserInfo(user, vendor, partner, password);
                string url = GetPaypalUrl();
                PayflowConnectionData payflowConnectionData = new PayflowConnectionData(url, 443, null, 0, null, null);

                Response response = null;
                if (transactionMode == TransactMode.Authorize)
                {
                    response = new AuthorizationTransaction(userInfo, payflowConnectionData, invoice, tender, PayflowUtility.RequestId).SubmitTransaction();
                }
                else
                {
                    response = new SaleTransaction(userInfo, payflowConnectionData, invoice, tender, PayflowUtility.RequestId).SubmitTransaction();
                }

                if (response.TransactionResponse != null)
                {
                    if (response.TransactionResponse.Result == 0)
                    {
                        processPaymentResult.AuthorizationTransactionId = response.TransactionResponse.Pnref;
                        processPaymentResult.AuthorizationTransactionResult = response.TransactionResponse.RespMsg;

                        if (transactionMode == TransactMode.Authorize)
                        {
                            processPaymentResult.PaymentStatus = PaymentStatusEnum.Authorized;
                        }
                        else
                        {
                            processPaymentResult.PaymentStatus = PaymentStatusEnum.Paid;
                        }
                    }
                    else
                    {
                        processPaymentResult.Error = string.Format("{0} - {1}", response.TransactionResponse.Result, response.TransactionResponse.RespMsg);
                        processPaymentResult.FullError = string.Format("Response Code : {0}. Response Description : {1}", response.TransactionResponse.Result, response.TransactionResponse.RespMsg);
                    }
                }
                else
                {
                    processPaymentResult.Error = "Error during checkout";
                    processPaymentResult.FullError = "Error during checkout";
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                NopContext.Current.SetCulture(userCulture);
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
            return IoCFactory.Resolve<ISettingManager>().GetSettingValueDecimalNative("PaymentMethod.PayFlowPro.AdditionalFee");
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
