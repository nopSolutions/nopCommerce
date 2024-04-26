using Nop.Core.Domain.Orders;

namespace Nop.Services.Payments;

/// <summary>
/// Payment service interface
/// </summary>
public partial interface IPaymentService
{
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
    /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    Task<bool> CanRePostProcessPaymentAsync(Order order);

    /// <summary>
    /// Gets an additional handling fee of a payment method
    /// </summary>
    /// <param name="cart">Shopping cart</param>
    /// <param name="paymentMethodSystemName">Payment method system name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the additional handling fee
    /// </returns>
    Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart, string paymentMethodSystemName);

    /// <summary>
    /// Gets a value indicating whether capture is supported by payment method
    /// </summary>
    /// <param name="paymentMethodSystemName">Payment method system name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a value indicating whether capture is supported
    /// </returns>
    Task<bool> SupportCaptureAsync(string paymentMethodSystemName);

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
    /// Gets a value indicating whether partial refund is supported by payment method
    /// </summary>
    /// <param name="paymentMethodSystemName">Payment method system name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a value indicating whether partial refund is supported
    /// </returns>
    Task<bool> SupportPartiallyRefundAsync(string paymentMethodSystemName);

    /// <summary>
    /// Gets a value indicating whether refund is supported by payment method
    /// </summary>
    /// <param name="paymentMethodSystemName">Payment method system name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a value indicating whether refund is supported
    /// </returns>
    Task<bool> SupportRefundAsync(string paymentMethodSystemName);

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
    /// Gets a value indicating whether void is supported by payment method
    /// </summary>
    /// <param name="paymentMethodSystemName">Payment method system name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a value indicating whether void is supported
    /// </returns>
    Task<bool> SupportVoidAsync(string paymentMethodSystemName);

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
    /// Gets a recurring payment type of payment method
    /// </summary>
    /// <param name="paymentMethodSystemName">Payment method system name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a recurring payment type of payment method
    /// </returns>
    Task<RecurringPaymentType> GetRecurringPaymentTypeAsync(string paymentMethodSystemName);

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
    /// Gets masked credit card number
    /// </summary>
    /// <param name="creditCardNumber">Credit card number</param>
    /// <returns>Masked credit card number</returns>
    string GetMaskedCreditCardNumber(string creditCardNumber);

    /// <summary>
    /// Serialize CustomValues of ProcessPaymentRequest
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Serialized CustomValues</returns>
    string SerializeCustomValues(ProcessPaymentRequest request);

    /// <summary>
    /// Deserialize CustomValues of Order
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>Serialized CustomValues CustomValues</returns>
    Dictionary<string, object> DeserializeCustomValues(Order order);

    /// <summary>
    /// Generate an order GUID
    /// </summary>
    /// <param name="processPaymentRequest">Process payment request</param>
    Task GenerateOrderGuidAsync(ProcessPaymentRequest processPaymentRequest);
}