using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.PaymentTokens;

/// <summary>
/// Represents the request to get customers' payment tokens
/// </summary>
public class GetPaymentTokensRequest : IAuthorizedRequest
{
    #region Properties

    /// <summary>
    /// Gets or sets the unique identifier representing a specific customer in merchant's system or records.
    /// </summary>
    [JsonIgnore]
    public string VaultCustomerId { get; set; }

    /// <summary>
    /// Gets the request path
    /// </summary>
    [JsonIgnore]
    public string Path => $"v3/vault/payment-tokens?customer_id={Uri.EscapeDataString(VaultCustomerId)}";

    /// <summary>
    /// Gets the request method
    /// </summary>
    [JsonIgnore]
    public string Method => HttpMethods.Get;

    #endregion
}