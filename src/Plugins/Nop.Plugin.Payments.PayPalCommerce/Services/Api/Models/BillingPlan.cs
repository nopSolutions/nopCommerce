using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the billing plan details
/// </summary>
public class BillingPlan
{
    #region Properties

    /// <summary>
    /// Gets or sets the name of the recurring plan.
    /// </summary>
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the array of billing cycles for trial billing and regular billing. A plan can have at most two trial cycles and only one regular cycle.
    /// </summary>
    [JsonProperty(PropertyName = "billing_cycles")]
    public List<BillingCycle> BillingCycles { get; set; }

    /// <summary>
    /// Gets or sets the price and currency for any one-time charges due at plan signup.
    /// </summary>
    [JsonProperty(PropertyName = "one_time_charges")]
    public OneTimeCharge OneTimeCharges { get; set; }

    #endregion
}