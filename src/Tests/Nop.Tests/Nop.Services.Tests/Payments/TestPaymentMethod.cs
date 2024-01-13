using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Services.Payments;
using Nop.Services.Plugins;

namespace Nop.Tests.Nop.Services.Tests.Payments;

public class TestPaymentMethod : BasePlugin, IPaymentMethod
{
    #region Methods

    /// <summary>
    /// Refunds a payment
    /// </summary>
    /// <param name="refundPaymentRequest">Request</param>
    /// <returns>Result</returns>
    public Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
    {
        var result = new RefundPaymentResult();
        result.AddError("Refund method not supported");

        return Task.FromResult(result);
    }

    /// <summary>
    /// Voids a payment
    /// </summary>
    /// <param name="voidPaymentRequest">Request</param>
    /// <returns>Result</returns>
    public Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
    {
        var result = new VoidPaymentResult();
        result.AddError("Void method not supported");

        return Task.FromResult(result);
    }

    /// <summary>
    /// Process recurring payment
    /// </summary>
    /// <param name="processPaymentRequest">Payment info required for an order processing</param>
    /// <returns>Process payment result</returns>
    public Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest)
    {
        var result = new ProcessPaymentResult();
        result.AddError("Recurring method not supported");

        return Task.FromResult(result);
    }

    /// <summary>
    /// Cancels a recurring payment
    /// </summary>
    /// <param name="cancelPaymentRequest">Request</param>
    /// <returns>Result</returns>
    public Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest)
    {
        var result = new CancelRecurringPaymentResult();
        result.AddError("Cancelling recurring orders not supported");

        return Task.FromResult(result);
    }

    /// <summary>
    /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>Result</returns>
    public Task<bool> CanRePostProcessPaymentAsync(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        //it's not a redirection payment method. So we always return false
        return Task.FromResult(false);
    }

    /// <summary>
    /// Validate payment form
    /// </summary>
    /// <param name="form">The parsed form values</param>
    /// <returns>List of validating errors</returns>
    public Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form)
    {
        return Task.FromResult<IList<string>>(new List<string>());
    }

    /// <summary>
    /// Get payment information
    /// </summary>
    /// <param name="form">The parsed form values</param>
    /// <returns>Payment info holder</returns>
    public Task<ProcessPaymentRequest> GetPaymentInfoAsync(IFormCollection form)
    {
        return Task.FromResult(new ProcessPaymentRequest());
    }

    /// <summary>
    /// Process a payment
    /// </summary>
    /// <param name="processPaymentRequest">Payment info required for an order processing</param>
    /// <returns>Process payment result</returns>
    public Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
    {
        var result = new ProcessPaymentResult
        {
            NewPaymentStatus = PaymentStatus.Paid
        };

        return Task.FromResult(result);
    }

    /// <summary>
    /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
    /// </summary>
    /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
    public Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Returns a value indicating whether payment method should be hidden during checkout
    /// </summary>
    /// <param name="cart">Shopping cart</param>
    /// <returns>true - hide; false - display.</returns>
    public Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
    {
        return Task.FromResult(false);
    }

    /// <summary>
    /// Gets additional handling fee
    /// </summary>
    /// <param name="cart">Shopping cart</param>
    /// <returns>Additional handling fee</returns>
    public Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
    {
        return Task.FromResult(AdditionalHandlingFee);
    }

    /// <summary>
    /// Captures payment
    /// </summary>
    /// <param name="capturePaymentRequest">Capture payment request</param>
    /// <returns>Capture payment result</returns>
    public Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
    {
        var result = new CapturePaymentResult();
        result.AddError("Capture method not supported");

        return Task.FromResult(result);
    }

    /// <summary>
    /// Gets a type of a view component for displaying plugin in public store ("payment info" checkout step)
    /// </summary>
    /// <returns>View component type</returns>
    public Type GetPublicViewComponent()
    {
        return null;
    }

    public Task<string> GetPaymentMethodDescriptionAsync()
    {
        return Task.FromResult(PaymentMethodDescription);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether capture is supported
    /// </summary>
    public bool SupportCapture => TestSupportCapture;

    /// <summary>
    /// Gets a value indicating whether partial refund is supported
    /// </summary>
    public bool SupportPartiallyRefund => TestSupportPartiallyRefund;

    /// <summary>
    /// Gets a value indicating whether refund is supported
    /// </summary>
    public bool SupportRefund => TestSupportRefund;

    /// <summary>
    /// Gets a value indicating whether void is supported
    /// </summary>
    public bool SupportVoid => TestSupportVoid;

    /// <summary>
    /// Gets a recurring payment type of payment method
    /// </summary>
    /// <returns>A recurring payment type of payment method</returns>
    public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;

    /// <summary>
    /// Gets a payment method type
    /// </summary>
    /// <returns>A payment method type</returns>
    public PaymentMethodType PaymentMethodType => PaymentMethodType.Standard;

    /// <summary>
    /// Gets a value indicating whether we should display a payment information page for this plugin
    /// </summary>
    public bool SkipPaymentInfo => false;

    /// <summary>
    /// Gets a payment method description that will be displayed on checkout pages in the public store
    /// </summary>
    /// <remarks>
    /// return description of this payment method to be display on "payment method" checkout step. good practice is to make it localizable
    /// for example, for a redirection payment method, description may be like this: "You will be redirected to PayPal site to complete the payment"
    /// </remarks>
    public string PaymentMethodDescription => string.Empty;

    #endregion

    #region Test data

    public static bool TestSupportCapture { get; set; } = false;

    public static bool TestSupportRefund { get; set; } = false;

    public static bool TestSupportPartiallyRefund { get; set; } = false;

    public static bool TestSupportVoid { get; set; } = false;

    public static decimal AdditionalHandlingFee { get; set; } = decimal.Zero;

    #endregion


}