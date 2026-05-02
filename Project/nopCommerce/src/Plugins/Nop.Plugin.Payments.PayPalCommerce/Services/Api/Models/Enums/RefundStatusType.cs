namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.Enums;

/// <summary>
/// Represents the type of the refund status
/// </summary>
public enum RefundStatusType
{
    /// <summary>
    /// The refund was cancelled.
    /// </summary>
    CANCELLED,

    /// <summary>
    /// The refund could not be processed.
    /// </summary>
    FAILED,

    /// <summary>
    /// The refund is pending. For more information, see `status_details.reason`.
    /// </summary>
    PENDING,

    /// <summary>
    /// The funds for this transaction were debited to the customer's account.
    /// </summary>
    COMPLETED
}