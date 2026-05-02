using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Orders;

/// <summary>
/// Represents the request to authorize payment for an order
/// </summary>
public class CreateAuthorizationRequest : Order, IAuthorizedRequest
{
    #region Properties

    /// <summary>
    /// Gets or sets the ID of the order for which to authorize.
    /// </summary>
    [JsonIgnore]
    public string OrderId { get; set; }

    /// <summary>
    /// Gets the request path
    /// </summary>
    [JsonIgnore]
    public string Path => $"v2/checkout/orders/{Uri.EscapeDataString(OrderId)}/authorize?";

    /// <summary>
    /// Gets the request method
    /// </summary>
    [JsonIgnore]
    public string Method => HttpMethods.Post;

    #endregion
}