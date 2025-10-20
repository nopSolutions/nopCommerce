using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the billing cycle details
/// </summary>
public class BillingCycle
{
    #region Properties

    /// <summary>
    /// Gets or sets the tenure type of the billing cycle identifies if the billing cycle is a trial (free or discounted) or regular billing cycle.
    /// </summary>
    [JsonProperty(PropertyName = "tenure_type")]
    public string TenureType { get; set; }

    /// <summary>
    /// Gets or sets the number of times this billing cycle gets executed.
    /// </summary>
    [JsonProperty(PropertyName = "total_cycles")]
    public int TotalCycles { get; set; }

    /// <summary>
    /// Gets or sets the order in which this cycle is to run among other billing cycles.
    /// </summary>
    [JsonProperty(PropertyName = "sequence")]
    public int Sequence { get; set; }

    /// <summary>
    /// Gets or sets the active pricing scheme for this billing cycle. A free trial billing cycle does not require a pricing scheme.
    /// </summary>
    [JsonProperty(PropertyName = "pricing_scheme")]
    public PricingScheme PricingScheme { get; set; }

    /// <summary>
    /// Gets or sets the billing cycle frequency.
    /// </summary>
    [JsonProperty(PropertyName = "frequency")]
    public BillingCycleFrequency Frequency { get; set; }

    /// <summary>
    /// Gets or sets the start date for the billing cycle, in YYYY-MM-DD. This field should be not be provided if the billing cycle starts at the time of checkout. When this field is not provided, the billing cycle amount will be included in any data validations confirming that the total provided by the merchant match the sum of individual items due at the time of checkout. Only one billing cycle (with sequence equal to 1) can have a no start date.
    /// </summary>
    [JsonProperty(PropertyName = "start_date")]
    public string StartDate { get; set; }

    #endregion
}