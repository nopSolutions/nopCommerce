using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.PaymentTokens;

/// <summary>
/// Represents the request to get a payment token of vaulted payment source
/// </summary>
public class GetPaymentTokenRequest : PaymentToken, IAuthorizedRequest
{
    #region Properties

    /// <summary>
    /// Gets or sets the ID of the payment token.
    /// </summary>
    [JsonIgnore]
    public string PaymentTokenId { get; set; }

    /// <summary>
    /// Gets the request path
    /// </summary>
    [JsonIgnore]
    public string Path => $"v3/vault/payment-tokens/{Uri.EscapeDataString(PaymentTokenId)}?";

    /// <summary>
    /// Gets the request method
    /// </summary>
    [JsonIgnore]
    public string Method => HttpMethods.Get;

    #endregion
}