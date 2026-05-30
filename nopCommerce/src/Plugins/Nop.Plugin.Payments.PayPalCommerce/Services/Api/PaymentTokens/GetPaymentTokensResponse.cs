using Newtonsoft.Json;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.PaymentTokens;

/// <summary>
/// Represents the response to a request to get customers' payment tokens
/// </summary>
public class GetPaymentTokensResponse : IApiResponse
{
    #region Properties

    /// <summary>
    /// Gets or sets the array of payment tokens.
    /// </summary>
    [JsonProperty(PropertyName = "payment_tokens")]
    public List<PaymentToken> PaymentTokens { get; set; }

    /// <summary>
    /// Gets or sets the array of request-related [HATEOAS links](/docs/api/reference/api-responses/#hateoas-links). To complete payer approval, use the `approve` link with the `GET` method.
    /// </summary>
    [JsonProperty(PropertyName = "links")]
    public List<Link> Links { get; set; }

    /// <summary>
    /// Gets or sets the customer in merchant's system of records.
    /// </summary>
    [JsonProperty(PropertyName = "customer")]
    public Payer Customer { get; set; }

    /// <summary>
    /// Gets or sets the total number of items.
    /// </summary>
    [JsonProperty(PropertyName = "total_items")]
    public int? TotalItems { get; set; }

    /// <summary>
    /// Gets or sets the total number of pages.
    /// </summary>
    [JsonProperty(PropertyName = "total_pages")]
    public int? TotalPages { get; set; }

    #endregion

}