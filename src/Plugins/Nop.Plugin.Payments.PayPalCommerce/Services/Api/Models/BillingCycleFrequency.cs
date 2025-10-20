using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the billing cycle frequency details
/// </summary>
public class BillingCycleFrequency
{
    #region Properties

    /// <summary>
    /// Gets or sets the interval unit.
    /// </summary>
    [JsonProperty(PropertyName = "interval_unit")]
    public string IntervalUnit { get; set; }

    /// <summary>
    /// Gets or sets the interval count.
    /// </summary>
    [JsonProperty(PropertyName = "interval_count")]
    public int IntervalCount { get; set; }

    #endregion
}