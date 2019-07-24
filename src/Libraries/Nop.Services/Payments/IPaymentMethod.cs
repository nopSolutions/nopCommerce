using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Orders;
using Nop.Services.Plugins;

namespace Nop.Services.Payments
{
    /// <summary>
    /// Provides an interface for creating payment gateways and methods
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
        /// Returns a value indicating whether payment method should be hidden during checkout
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>true - hide; false - display.</returns>
        bool HidePaymentMethod(IList<ShoppingCartItem> cart);

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>Additional handling fee</returns>
        decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart);

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

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        bool CanRePostProcessPayment(Order order);

        /// <summary>
        /// Validate payment form
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>List of validating errors</returns>
        IList<string> ValidatePaymentForm(IFormCollection form);

        /// <summary>
        /// Get payment information
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>Payment info holder</returns>
        ProcessPaymentRequest GetPaymentInfo(IFormCollection form);

        /// <summary>
        /// Gets a name of a view component for displaying plugin in public store ("payment info" checkout step)
        /// </summary>
        /// <returns>View component name</returns>
        string GetPublicViewComponentName();

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        bool SupportCapture { get; }

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        bool SupportPartiallyRefund { get; }

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        bool SupportRefund { get; }

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        bool SupportVoid { get; }

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        RecurringPaymentType RecurringPaymentType { get; }

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        PaymentMethodType PaymentMethodType { get; }

        /// <summary>
        /// Gets a value indicating whether we should display a payment information page for this plugin
        /// </summary>
        bool SkipPaymentInfo { get; }

        /// <summary>
        /// Gets a payment method description that will be displayed on checkout pages in the public store
        /// </summary>
        string PaymentMethodDescription { get; }

        #endregion
    }
}
