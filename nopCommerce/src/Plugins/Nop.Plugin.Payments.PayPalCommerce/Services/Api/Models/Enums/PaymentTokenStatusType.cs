namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.Enums;

/// <summary>
/// Represents the type of the payment token status
/// </summary>
public enum PaymentTokenStatusType
{
    /// <summary>
    /// The payment token was created.
    /// </summary>
    CREATED,

    /// <summary>
    /// The payment token requires an action from the payer. Redirect the payer to the "rel":"approve" HATEOAS link returned as part of the response.
    /// </summary>
    PAYER_ACTION_REQUIRED
}