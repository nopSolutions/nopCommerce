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
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Security;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.Payment.Methods.Manual
{
    /// <summary>
    /// Manual payment processor
    /// </summary>
    public class ManualPaymentProcessor : IPaymentMethod
    {
        #region Methods
        /// <summary>
        /// Gets current transaction mode
        /// </summary>
        /// <returns>Current transaction mode</returns>
        public static TransactMode GetCurrentTransactionMode()
        {
            TransactMode transactionModeEnum = TransactMode.Authorize;
            string transactionMode = IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.Manual.TransactionMode");
            if (!String.IsNullOrEmpty(transactionMode))
                transactionModeEnum = (TransactMode)Enum.Parse(typeof(TransactMode), transactionMode);
            return transactionModeEnum;
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
            processPaymentResult.AllowStoringCreditCardNumber = true;
            TransactMode transactionMode = GetCurrentTransactionMode();
            switch (transactionMode)
            {
                case TransactMode.Pending:
                    processPaymentResult.PaymentStatus = PaymentStatusEnum.Pending;
                    break;
                case TransactMode.Authorize:
                    processPaymentResult.PaymentStatus = PaymentStatusEnum.Authorized;
                    break;
                case TransactMode.AuthorizeAndCapture:
                    processPaymentResult.PaymentStatus = PaymentStatusEnum.Paid;
                    break;
                default:
                    throw new NopException("Not supported transact type");
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
            return IoCFactory.Resolve<ISettingManager>().GetSettingValueDecimalNative("PaymentMethod.Manual.AdditionalFee");
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
            processPaymentResult.AllowStoringCreditCardNumber = true;
            TransactMode transactionMode = GetCurrentTransactionMode();
            switch (transactionMode)
            {
                case TransactMode.Pending:
                    processPaymentResult.PaymentStatus = PaymentStatusEnum.Pending;
                    break;
                case TransactMode.Authorize:
                    processPaymentResult.PaymentStatus = PaymentStatusEnum.Authorized;
                    break;
                case TransactMode.AuthorizeAndCapture:
                    processPaymentResult.PaymentStatus = PaymentStatusEnum.Paid;
                    break;
                default:
                    throw new NopException("Not supported transact type");
            }

            //restore credit cart info
            if (paymentInfo.IsRecurringPayment)
            {
                Order initialOrder = IoCFactory.Resolve<IOrderManager>().GetOrderById(paymentInfo.InitialOrderId);
                if (initialOrder != null)
                {
                    paymentInfo.CreditCardType = processPaymentResult.AllowStoringCreditCardNumber ? SecurityHelper.Decrypt(initialOrder.CardType) : string.Empty;
                    paymentInfo.CreditCardName = processPaymentResult.AllowStoringCreditCardNumber ? SecurityHelper.Decrypt(initialOrder.CardName) : string.Empty;
                    paymentInfo.CreditCardNumber = processPaymentResult.AllowStoringCreditCardNumber ? SecurityHelper.Decrypt(initialOrder.CardNumber) : string.Empty;
                    paymentInfo.CreditCardCvv2 = processPaymentResult.AllowStoringCreditCardNumber ? SecurityHelper.Decrypt(initialOrder.CardCvv2) : string.Empty;
                    try
                    {
                        paymentInfo.CreditCardExpireMonth = Convert.ToInt32(processPaymentResult.AllowStoringCreditCardNumber ? SecurityHelper.Decrypt(initialOrder.CardExpirationMonth) : "0");
                        paymentInfo.CreditCardExpireYear = Convert.ToInt32(processPaymentResult.AllowStoringCreditCardNumber ? SecurityHelper.Decrypt(initialOrder.CardExpirationYear) : "0");
                    }
                    catch
                    {
                    }
                }
            }
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
                return RecurringPaymentTypeEnum.Manual;
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
