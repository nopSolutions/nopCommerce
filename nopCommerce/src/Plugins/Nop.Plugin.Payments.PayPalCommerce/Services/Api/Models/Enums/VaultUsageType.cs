namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.Enums;

/// <summary>
/// Represents the type of the vault usage
/// </summary>
public enum VaultUsageType
{
    /// <summary>
    /// The customer acting on the merchant platform is a consumer.
    /// </summary>
    CONSUMER,

    /// <summary>
    /// The customer acting on the merchant platform is a business.
    /// </summary>
    BUSINESS,

    /// <summary>
    /// The merchant usage type.
    /// </summary>
    MERCHANT,

    /// <summary>
    /// The platform usage type.
    /// </summary>
    PLATFORM
}