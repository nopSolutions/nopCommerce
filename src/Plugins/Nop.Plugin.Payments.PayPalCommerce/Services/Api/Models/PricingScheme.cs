using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the pricing scheme details
/// </summary>
public class PricingScheme
{
    #region Properties

    /// <summary>
    /// Gets or sets the pricing model for the billing cycle.
    /// </summary>
    [JsonProperty(PropertyName = "pricing_model")]
    public string PricingModel { get; set; }

    /// <summary>
    /// Gets or sets the price the customer will be charged based on the pricing model.
    /// </summary>
    [JsonProperty(PropertyName = "price")]
    public Money Price { get; set; }

    /// <summary>
    /// Gets or sets the threshold amount on which the reload charge would be triggered. This will be associated with the account-balance where if the account-balance goes below this amount then customer would incur reload charge.
    /// </summary>
    [JsonProperty(PropertyName = "reload_threshold_amount")]
    public Money ReloadThresholdAmount { get; set; }

    #endregion
}