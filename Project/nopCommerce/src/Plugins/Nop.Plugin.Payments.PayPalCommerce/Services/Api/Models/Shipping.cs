using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the shipping details
/// </summary>
public class Shipping
{
    #region Properties

    /// <summary>
    /// Gets or sets the classification for the method of purchase fulfillment (e.g shipping, in-store pickup, etc). Either `type` or `options` may be present, but not both.
    /// </summary>
    [JsonProperty(PropertyName = "type")]
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the array of shipping options that the payee or merchant offers to the payer to ship or pick up their items.
    /// </summary>
    [JsonProperty(PropertyName = "options")]
    public List<ShippingOption> Options { get; set; }

    /// <summary>
    /// Gets or sets the name of the person to whom to ship the items.
    /// </summary>
    [JsonProperty(PropertyName = "name")]
    public Name Name { get; set; }

    /// <summary>
    /// Gets or sets the address of the person to whom to ship the items.
    /// </summary>
    [JsonProperty(PropertyName = "address")]
    public Address Address { get; set; }

    /// <summary>
    /// Gets or sets the array of trackers for a transaction.
    /// </summary>
    [JsonProperty(PropertyName = "trackers")]
    public List<ShippingTracker> Trackers { get; set; }

    #endregion
}