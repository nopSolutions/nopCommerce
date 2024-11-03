using Newtonsoft.Json;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Webhooks;

/// <summary>
/// Represents the response to a request to verify a webhook signature
/// </summary>
public class CreateWebhookSignatureResponse : WebhookSignature, IApiResponse
{
    #region Properties

    /// <summary>
    /// Gets or sets the status of the signature verification.
    /// </summary>
    [JsonProperty(PropertyName = "verification_status")]
    public string VerificationStatus { get; set; }

    #endregion
}