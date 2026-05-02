using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Orders;

/// <summary>
/// Represents the request to create an order
/// </summary>
public class CreateOrderRequest : Order, IAuthorizedRequest
{
    #region Properties

    /// <summary>
    /// Gets or sets the payer experience during the approval process for the payment with PayPal.<blockquote><strong>Note:</strong> Partners and Marketplaces might configure <code>brand_name</code> and <code>shipping_preference</code> during partner account setup, which overrides the request values.</blockquote>
    /// </summary>
    [JsonProperty(PropertyName = "application_context")]
    public ApplicationContext ApplicationContext { get; set; }

    /// <summary>
    /// Gets the request path
    /// </summary>
    [JsonIgnore]
    public string Path => $"v2/checkout/orders?";

    /// <summary>
    /// Gets the request method
    /// </summary>
    [JsonIgnore]
    public string Method => HttpMethods.Post;

    #endregion
}