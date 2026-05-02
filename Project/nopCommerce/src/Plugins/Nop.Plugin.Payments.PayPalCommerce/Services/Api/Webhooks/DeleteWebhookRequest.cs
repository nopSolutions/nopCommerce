using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Webhooks;

/// <summary>
/// Represents the request to delete a webhook
/// </summary>
public class DeleteWebhookRequest : IAuthorizedRequest
{
    #region Properties

    /// <summary>
    /// Gets or sets the ID of the webhook
    /// </summary>
    [JsonIgnore]
    public string WebhookId { get; set; }

    /// <summary>
    /// Gets the request path
    /// </summary>
    [JsonIgnore]
    public string Path => $"v1/notifications/webhooks/{Uri.EscapeDataString(WebhookId)}?";

    /// <summary>
    /// Gets the request method
    /// </summary>
    [JsonIgnore]
    public string Method => HttpMethods.Delete;

    #endregion
}