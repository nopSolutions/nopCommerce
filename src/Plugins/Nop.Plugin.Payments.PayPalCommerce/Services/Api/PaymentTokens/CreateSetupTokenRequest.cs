using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.PaymentTokens;

/// <summary>
/// Represents the request to create a Setup Token from the given payment source and adds it to the Vault of the associated customer
/// </summary>
public class CreateSetupTokenRequest : IAuthorizedRequest
{
    #region Properties

    /// <summary>
    /// Gets or sets the customer in merchant's or partner's system of records.
    /// </summary>
    [JsonProperty(PropertyName = "customer")]
    public Payer Customer { get; set; }

    /// <summary>
    /// Gets or sets the vaulted payment method details.
    /// </summary>
    [JsonProperty(PropertyName = "payment_source")]
    public RecurringPaymentSource PaymentSource { get; set; }

    /// <summary>
    /// Gets the request path
    /// </summary>
    [JsonIgnore]
    public string Path => $"v3/vault/setup-tokens?";

    /// <summary>
    /// Gets the request method
    /// </summary>
    [JsonIgnore]
    public string Method => HttpMethods.Post;

    #endregion
}