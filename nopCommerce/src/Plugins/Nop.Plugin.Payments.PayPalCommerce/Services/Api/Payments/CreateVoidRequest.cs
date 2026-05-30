using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Payments;

/// <summary>
/// Represents the request to void or cancel an authorized payment
/// </summary>
public class CreateVoidRequest : IAuthorizedRequest
{
    #region Properties

    /// <summary>
    /// Gets or sets the ID of the webhook
    /// </summary>
    [JsonIgnore]
    public string AuthorizationId { get; set; }

    /// <summary>
    /// Gets the request path
    /// </summary>
    [JsonIgnore]
    public string Path => $"v2/payments/authorizations/{Uri.EscapeDataString(AuthorizationId)}/void?";

    /// <summary>
    /// Gets the request method
    /// </summary>
    [JsonIgnore]
    public string Method => HttpMethods.Post;

    #endregion
}