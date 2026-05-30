using Newtonsoft.Json;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Webhooks;

/// <summary>
/// Represents the response to a request to get all webhooks for an app
/// </summary>
public class GetWebhooksResponse : IApiResponse
{
    #region Properties

    /// <summary>
    /// Gets or sets the array of webhooks.
    /// </summary>
    [JsonProperty(PropertyName = "webhooks")]
    public List<Webhook> Webhooks { get; set; }

    #endregion
}