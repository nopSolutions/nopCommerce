using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Webhooks;

/// <summary>
/// Represents the request to verify a webhook signature
/// </summary>
public class CreateWebhookSignatureRequest : WebhookSignature, IAuthorizedRequest
{
    #region Properties

    /// <summary>
    /// Gets the request path
    /// </summary>
    [JsonIgnore]
    public string Path => $"v1/notifications/verify-webhook-signature?";

    /// <summary>
    /// Gets the request method
    /// </summary>
    [JsonIgnore]
    public string Method => HttpMethods.Post;

    #endregion
}