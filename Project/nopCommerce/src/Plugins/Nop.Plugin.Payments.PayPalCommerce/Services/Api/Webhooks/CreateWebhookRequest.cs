using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Webhooks;

/// <summary>
/// Represents the request to subscribe a webhook listener to events
/// </summary>
public class CreateWebhookRequest : Webhook, IAuthorizedRequest
{
    #region Properties

    /// <summary>
    /// Gets the request path
    /// </summary>
    [JsonIgnore]
    public string Path => $"v1/notifications/webhooks?";

    /// <summary>
    /// Gets the request method
    /// </summary>
    [JsonIgnore]
    public string Method => HttpMethods.Post;

    #endregion
}