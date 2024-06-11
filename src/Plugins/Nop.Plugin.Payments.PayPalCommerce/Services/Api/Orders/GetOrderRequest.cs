using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Orders;

/// <summary>
/// Represents the request to get an order
/// </summary>
public class GetOrderRequest : IAuthorizedRequest
{
    #region Properties

    /// <summary>
    /// Gets or sets the ID of the order for which to show details.
    /// </summary>
    [JsonIgnore]
    public string OrderId { get; set; }

    /// <summary>
    /// Gets or sets the comma-separated list of fields that should be returned for the order. Valid filter field is `payment_source`.
    /// </summary>
    [JsonProperty(PropertyName = "fields")]
    public string Fields { get; set; }

    /// <summary>
    /// Gets the request path
    /// </summary>
    [JsonIgnore]
    public string Path => $"v2/checkout/orders/{Uri.EscapeDataString(OrderId)}?";

    /// <summary>
    /// Gets the request method
    /// </summary>
    [JsonIgnore]
    public string Method => HttpMethods.Get;

    #endregion
}