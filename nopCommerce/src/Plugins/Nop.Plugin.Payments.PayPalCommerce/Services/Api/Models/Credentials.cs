using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the REST API application credentials
/// </summary>
public class Credentials
{
    #region Properties

    /// <summary>
    /// Gets or sets the REST API client id.
    /// </summary>
    [JsonProperty(PropertyName = "client_id")]
    public string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the REST API client secret.
    /// </summary>
    [JsonProperty(PropertyName = "client_secret")]
    public string ClientSecret { get; set; }

    /// <summary>
    /// Gets or sets the PayPal-assigned ID for the payer.
    /// </summary>
    [JsonProperty(PropertyName = "payer_id")]
    public string PayerId { get; set; }

    #endregion
}