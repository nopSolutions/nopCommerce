namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.Enums;

/// <summary>
/// Represents the type of the vault status
/// </summary>
public enum VaultStatusType
{
    /// <summary>
    /// The payment source has been saved in your customer's vault.
    /// </summary>
    VAULTED,

    /// <summary>
    /// Customer has approved the action of saving the specified payment_source into their vault.
    /// </summary>
    APPROVED,

    /// <summary>
    /// DEPRECATED
    /// The payment source has been saved in your customer's vault.
    /// </summary>
    CREATED
}