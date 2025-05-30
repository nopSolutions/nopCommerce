using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Orders;

/// <summary>
/// Represents the request to update the order
/// </summary>
public class UpdateOrderRequest<TValue> : List<Patch<TValue>>, IAuthorizedRequest where TValue : class
{
    #region Ctor

    public UpdateOrderRequest(IEnumerable<Patch<TValue>> collection) : base(collection)
    {
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the ID of the order to update.
    /// </summary>
    [JsonIgnore]
    public string OrderId { get; set; }

    /// <summary>
    /// Gets the request path
    /// </summary>
    [JsonIgnore]
    public string Path => $"v2/checkout/orders/{Uri.EscapeDataString(OrderId)}?";

    /// <summary>
    /// Gets the request method
    /// </summary>
    [JsonIgnore]
    public string Method => HttpMethods.Patch;

    #endregion
}