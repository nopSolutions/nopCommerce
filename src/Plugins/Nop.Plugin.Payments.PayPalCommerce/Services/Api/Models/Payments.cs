using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the payments history
/// </summary>
public class Payments
{
    #region Properties

    /// <summary>
    /// Gets or sets the array of authorized payments for a purchase unit. A purchase unit can have zero or more authorized payments.
    /// </summary>
    [JsonProperty(PropertyName = "authorizations")]
    public List<Authorization> Authorizations { get; set; }

    /// <summary>
    /// Gets or sets the array of captured payments for a purchase unit. A purchase unit can have zero or more captured payments.
    /// </summary>
    [JsonProperty(PropertyName = "captures")]
    public List<Capture> Captures { get; set; }

    /// <summary>
    /// Gets or sets the array of refunds for a purchase unit. A purchase unit can have zero or more refunds.
    /// </summary>
    [JsonProperty(PropertyName = "refunds")]
    public List<Refund> Refunds { get; set; }

    #endregion
}