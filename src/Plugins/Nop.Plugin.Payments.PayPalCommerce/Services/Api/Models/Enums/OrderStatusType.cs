namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.Enums;

/// <summary>
/// Represents the type of the order status
/// </summary>
public enum OrderStatusType
{
    /// <summary>
    /// The order was created with the specified context.
    /// </summary>
    CREATED,

    /// <summary>
    /// The order was saved and persisted. The order status continues to be in progress until a capture is made with final_capture = true for all purchase units within the order.
    /// </summary>
    SAVED,

    /// <summary>
    /// The customer approved the payment through the PayPal wallet or another form of guest or unbranded payment. For example, a card, bank account, or so on.
    /// </summary>
    APPROVED,

    /// <summary>
    /// All purchase units in the order are voided.
    /// </summary>
    VOIDED,

    /// <summary>
    /// The payment was authorized or the authorized payment was captured for the order.
    /// </summary>
    COMPLETED,

    /// <summary>
    /// The order requires an action from the payer (e.g. 3DS authentication). Redirect the payer to the "rel":"payer-action" HATEOAS link returned as part of the response prior to authorizing or capturing the order. Some payment sources may not return a payer-action HATEOAS link (eg. MB WAY). For these payment sources the payer-action is managed by the scheme itself (eg. through SMS, email, in-app notification, etc).
    /// </summary>
    PAYER_ACTION_REQUIRED
}