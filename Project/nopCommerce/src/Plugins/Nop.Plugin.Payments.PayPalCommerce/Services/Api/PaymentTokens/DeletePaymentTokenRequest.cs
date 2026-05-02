using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.PaymentTokens;

/// <summary>
/// Represents the request to delete a payment token
/// </summary>
public class DeletePaymentTokenRequest : IAuthorizedRequest
{
    #region Properties

    /// <summary>
    /// Gets or sets the ID of the payment token.
    /// </summary>
    [JsonIgnore]
    public string Id { get; set; }

    /// <summary>
    /// Gets the request path
    /// </summary>
    [JsonIgnore]
    public string Path => $"v3/vault/payment-tokens/{Uri.EscapeDataString(Id)}?";

    /// <summary>
    /// Gets the request method
    /// </summary>
    [JsonIgnore]
    public string Method => HttpMethods.Delete;

    #endregion
}