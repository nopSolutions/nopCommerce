using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Orders;
using Nop.Services.Plugins;

namespace Nop.Services.Payments;

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
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the process payment result
    /// </returns>
    Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest);

    /// <summary>
    /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
    /// </summary>
    /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest);

    /// <summary>
    /// Returns a value indicating whether payment method should be hidden during checkout
    /// </summary>
    /// <param name="cart">Shopping cart</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true - hide; false - display.
    /// </returns>
    Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart);

    /// <summary>
    /// Gets additional handling fee
    /// </summary>
    /// <param name="cart">Shopping cart</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the additional handling fee
    /// </returns>
    Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart);

    /// <summary>
    /// Captures payment
    /// </summary>
    /// <param name="capturePaymentRequest">Capture payment request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the capture payment result
    /// </returns>
    Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest);

    /// <summary>
    /// Refunds a payment
    /// </summary>
    /// <param name="refundPaymentRequest">Request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest);

    /// <summary>
    /// Voids a payment
    /// </summary>
    /// <param name="voidPaymentRequest">Request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest);

    /// <summary>
    /// Process recurring payment
    /// </summary>
    /// <param name="processPaymentRequest">Payment info required for an order processing</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the process payment result
    /// </returns>
    Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest);

    /// <summary>
    /// Cancels a recurring payment
    /// </summary>
    /// <param name="cancelPaymentRequest">Request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest);

    /// <summary>
    /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    Task<bool> CanRePostProcessPaymentAsync(Order order);

    /// <summary>
    /// Validate payment form
    /// </summary>
    /// <param name="form">The parsed form values</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of validating errors
    /// </returns>
    Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form);

    /// <summary>
    /// Get payment information
    /// </summary>
    /// <param name="form">The parsed form values</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the payment info holder
    /// </returns>
    Task<ProcessPaymentRequest> GetPaymentInfoAsync(IFormCollection form);

    /// <summary>
    /// Gets a type of a view component for displaying plugin in public store ("payment info" checkout step)
    /// </summary>
    /// <returns>View component type</returns>
    Type GetPublicViewComponent();

    /// <summary>
    /// Gets a payment method description that will be displayed on checkout pages in the public store
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task<string> GetPaymentMethodDescriptionAsync();

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

    #endregion
}