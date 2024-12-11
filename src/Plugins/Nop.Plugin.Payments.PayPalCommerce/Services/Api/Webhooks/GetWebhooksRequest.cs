using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Webhooks;

/// <summary>
/// Represents the request to get all webhooks for an app
/// </summary>
public class GetWebhooksRequest : IAuthorizedRequest
{
    #region Properties

    /// <summary>
    /// Gets or sets the anchor type ("APPLICATION" (default) or "ACCOUNT")
    /// </summary>
    [JsonIgnore]
    public string AnchorType { get; set; }

    /// <summary>
    /// Gets the request path
    /// </summary>
    [JsonIgnore]
    public string Path => $"v1/notifications/webhooks?{(!string.IsNullOrEmpty(AnchorType) ? $"anchor_type={Uri.EscapeDataString(AnchorType)}" : null)}";

    /// <summary>
    /// Gets the request method
    /// </summary>
    [JsonIgnore]
    public string Method => HttpMethods.Get;

    #endregion
}