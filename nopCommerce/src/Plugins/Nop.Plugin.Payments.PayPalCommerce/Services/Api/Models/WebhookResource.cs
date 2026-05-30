using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the webhook resource
/// </summary>
public class WebhookResource : IWebhookResource
{
    #region Properties

    /// <summary>
    /// Gets or sets the API caller-provided external ID.
    /// </summary>
    [JsonProperty(PropertyName = "custom_id")]
    public string CustomId { get; set; }

    #endregion
}