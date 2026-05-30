namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.Enums;

/// <summary>
/// Represents the type of the pricing model
/// </summary>
public enum PricingModelType
{
    /// <summary>
    /// A fixed pricing scheme where the customer is charged a fixed amount.
    /// </summary>
    FIXED,

    /// <summary>
    /// A variable pricing scheme where the customer is charged a variable amount.
    /// </summary>
    VARIABLE,

    /// <summary>
    /// A auto-reload pricing scheme where the customer is charged a fixed amount for reload.
    /// </summary>
    AUTO_RELOAD
}