namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.Enums;

/// <summary>
/// Represents the type of the integration
/// </summary>
public enum IntegrationType
{
    /// <summary>
    /// A first-party integration.
    /// </summary>
    FIRST_PARTY,

    /// <summary>
    /// A third-party integration.
    /// </summary>
    THIRD_PARTY,

    /// <summary>
    /// A first-party integration (URL onboarding).
    /// </summary>
    FO
}