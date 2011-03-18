using System;
using System.Collections.Generic;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Plugins;

namespace Nop.Services.Payments
{
    /// <summary>
    /// Provides an interface for creating payment gateways & methods
    /// </summary>
    public partial interface IPaymentMethod : IPlugin
    {
        #region Methods

        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest);
        
        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest);

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <returns>Additional handling fee</returns>
        decimal GetAdditionalHandlingFee();
        
        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>Capture payment result</returns>
        CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest);

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest);

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>Result</returns>
        VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest);

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest);

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>Result</returns>
        CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest);

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        bool CanCapture { get; }

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        bool CanPartiallyRefund { get; }

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        bool CanRefund { get; }

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        bool CanVoid { get; }

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        /// <returns>A recurring payment type of payment method</returns>
        RecurringPaymentType SupportRecurringPayments { get; }
        
        /// <summary>
        /// Gets a payment method type
        /// </summary>
        /// <returns>A payment method type</returns>
        PaymentMethodType PaymentMethodType { get; }
        
        #endregion
    }
}
