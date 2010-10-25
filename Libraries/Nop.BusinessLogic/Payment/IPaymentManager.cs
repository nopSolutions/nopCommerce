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
using System.Web.Compilation;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.BusinessLogic.Payment
{
    /// <summary>
    /// Payment manager interface
    /// </summary>
    public partial interface IPaymentManager
    {
        /// <summary>
        /// Process payment
        /// </summary>
        /// <param name="paymentInfo">Payment info required for an order processing</param>
        /// <param name="customer">Customer</param>
        /// <param name="orderGuid">Unique order identifier</param>
        /// <param name="processPaymentResult">Process payment result</param>
        void ProcessPayment(PaymentInfo paymentInfo, Customer customer,
            Guid orderGuid, ref ProcessPaymentResult processPaymentResult);

        /// <summary>
        /// Post process payment (payment gateways that require redirecting)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>The error status, or String.Empty if no errors</returns>
        string PostProcessPayment(Order order);

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <param name="paymentMethodId">Payment method identifier</param>
        /// <returns>Additional handling fee</returns>
        decimal GetAdditionalHandlingFee(int paymentMethodId);

        /// <summary>
        /// Gets a value indicating whether capture is supported by payment method
        /// </summary>
        /// <param name="paymentMethodId">Payment method identifier</param>
        /// <returns>A value indicating whether capture is supported</returns>
        bool CanCapture(int paymentMethodId);
        
        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="processPaymentResult">Process payment result</param>
        void Capture(Order order, ref ProcessPaymentResult processPaymentResult);

        /// <summary>
        /// Gets a value indicating whether partial refund is supported by payment method
        /// </summary>
        /// <param name="paymentMethodId">Payment method identifier</param>
        /// <returns>A value indicating whether partial refund is supported</returns>
        bool CanPartiallyRefund(int paymentMethodId);

        /// <summary>
        /// Gets a value indicating whether refund is supported by payment method
        /// </summary>
        /// <param name="paymentMethodId">Payment method identifier</param>
        /// <returns>A value indicating whether refund is supported</returns>
        bool CanRefund(int paymentMethodId);

        /// <summary>
        /// Refunds payment
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="cancelPaymentResult">Cancel payment result</param>
        void Refund(Order order, ref CancelPaymentResult cancelPaymentResult);

        /// <summary>
        /// Gets a value indicating whether void is supported by payment method
        /// </summary>
        /// <param name="paymentMethodId">Payment method identifier</param>
        /// <returns>A value indicating whether void is supported</returns>
        bool CanVoid(int paymentMethodId);

        /// <summary>
        /// Voids payment
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="cancelPaymentResult">Cancel payment result</param>
        void Void(Order order, ref CancelPaymentResult cancelPaymentResult);

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        /// <param name="paymentMethodId">Payment method identifier</param>
        /// <returns>A recurring payment type of payment method</returns>
        RecurringPaymentTypeEnum SupportRecurringPayments(int paymentMethodId);

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        /// <param name="paymentMethodId">Payment method identifier</param>
        /// <returns>A payment method type</returns>
        PaymentMethodTypeEnum GetPaymentMethodType(int paymentMethodId);

        /// <summary>
        /// Process recurring payments
        /// </summary>
        /// <param name="paymentInfo">Payment info required for an order processing</param>
        /// <param name="customer">Customer</param>
        /// <param name="orderGuid">Unique order identifier</param>
        /// <param name="processPaymentResult">Process payment result</param>
        void ProcessRecurringPayment(PaymentInfo paymentInfo, Customer customer,
            Guid orderGuid, ref ProcessPaymentResult processPaymentResult);

        /// <summary>
        /// Cancels recurring payment
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="cancelPaymentResult">Cancel payment result</param>
        void CancelRecurringPayment(Order order, ref CancelPaymentResult cancelPaymentResult);

        /// <summary>
        /// Gets masked credit card number
        /// </summary>
        /// <param name="creditCardNumber">Credit card number</param>
        /// <returns>Masked credit card number</returns>
        string GetMaskedCreditCardNumber(string creditCardNumber);
    }
}
