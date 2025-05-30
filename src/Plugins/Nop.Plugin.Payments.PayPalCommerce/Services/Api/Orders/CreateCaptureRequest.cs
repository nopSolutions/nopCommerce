using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Orders;

/// <summary>
/// Represents the request to capture payment for an order
/// </summary>
public class CreateCaptureRequest : Order, IAuthorizedRequest
{
    #region Properties

    /// <summary>
    /// Gets or sets the ID of the order for which to capture a payment.
    /// </summary>
    [JsonIgnore]
    public string OrderId { get; set; }

    /// <summary>
    /// Gets the request path
    /// </summary>
    [JsonIgnore]
    public string Path => $"v2/checkout/orders/{Uri.EscapeDataString(OrderId)}/capture?";

    /// <summary>
    /// Gets the request method
    /// </summary>
    [JsonIgnore]
    public string Method => HttpMethods.Post;

    #endregion
}