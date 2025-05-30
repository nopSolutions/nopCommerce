namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.Enums;

/// <summary>
/// Represents the type of the product status
/// </summary>
public enum ProductStatusType
{
    /// <summary>
    /// The product is approved and could be enabled.
    /// </summary>
    APPROVED,

    /// <summary>
    /// The product is in a pending state waiting for decision from policy systems.
    /// </summary>
    PENDING,

    /// <summary>
    /// The product is declined.
    /// </summary>
    DECLINED,

    /// <summary>
    /// The product is enabled for the account and can be used.
    /// </summary>
    SUBSCRIBED,

    /// <summary>
    /// The request is in review by policy system.
    /// </summary>
    IN_REVIEW,

    /// <summary>
    /// Need details or documents required to enable this product.
    /// </summary>
    NEED_MORE_DATA,

    /// <summary>
    /// The product can no longer be used.
    /// </summary>
    DENIED
}