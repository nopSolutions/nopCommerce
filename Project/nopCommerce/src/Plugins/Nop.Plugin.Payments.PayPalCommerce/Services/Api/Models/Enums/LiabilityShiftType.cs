namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.Enums;

/// <summary>
/// Represents the type of the liability shift
/// </summary>
public enum LiabilityShiftType
{
    /// <summary>
    /// Liability might shift to the card issuer.
    /// </summary>
    POSSIBLE,

    /// <summary>
    /// Liability has shifted to the card issuer.
    /// </summary>
    YES,

    /// <summary>
    /// Liability is with the merchant.
    /// </summary>
    NO,

    /// <summary>
    /// The authentication system isn't available.
    /// </summary>
    UNKNOWN
}