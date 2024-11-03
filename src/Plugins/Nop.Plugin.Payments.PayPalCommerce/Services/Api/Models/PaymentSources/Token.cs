using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.PaymentSources;

/// <summary>
/// Represents the tokenized payment source to fund a payment
/// </summary>
public class Token
{
    #region Properties

    /// <summary>
    /// Gets or sets the PayPal-generated ID for the token.
    /// </summary>
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the tokenization method that generated the ID.
    /// </summary>
    [JsonProperty(PropertyName = "type")]
    public string Type { get; set; }

    #endregion
}