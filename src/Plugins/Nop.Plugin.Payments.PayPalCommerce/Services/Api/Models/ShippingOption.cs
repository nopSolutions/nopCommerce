using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the shipping option
/// </summary>
public class ShippingOption
{
    #region Properties

    /// <summary>
    /// Gets or sets the unique ID that identifies a payer-selected shipping option.
    /// </summary>
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the description that the payer sees, which helps them choose an appropriate shipping option. For example, `Free Shipping, USPS Priority Shipping`, `Expédition prioritaire USPS`, or `USPS yōuxiān fā huò`. Localize this description to the payer's locale.
    /// </summary>
    [JsonProperty(PropertyName = "label")]
    public string Label { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the shipping option that the payee or merchant expects to be pre-selected for the payer when they first view the `shipping.options` in the PayPal Checkout experience.
    /// </summary>
    [JsonProperty(PropertyName = "selected")]
    public bool? Selected { get; set; }

    /// <summary>
    /// Gets or sets the classification for the method of purchase fulfillment.
    /// </summary>
    [JsonProperty(PropertyName = "type")]
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the shipping cost for the selected option.
    /// </summary>
    [JsonProperty(PropertyName = "amount")]
    public Money Amount { get; set; }

    #endregion
}