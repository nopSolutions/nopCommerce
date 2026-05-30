using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Payments;

/// <summary>
/// Represents the request to refund a captured payment
/// </summary>
public class CreateRefundRequest : Refund, IAuthorizedRequest
{
    #region Properties

    /// <summary>
    /// Gets or sets the ID of the webhook
    /// </summary>
    [JsonIgnore]
    public string CaptureId { get; set; }

    /// <summary>
    /// Gets or sets the additional payment instructions to be consider during refund payment processing. This object is only applicable to merchants that have been enabled for PayPal Commerce Platform for Marketplaces and Platforms capability.
    /// </summary>
    [JsonProperty(PropertyName = "payment_instruction")]
    public PaymentInstruction PaymentInstruction { get; set; }

    /// <summary>
    /// Gets the request path
    /// </summary>
    [JsonIgnore]
    public string Path => $"v2/payments/captures/{Uri.EscapeDataString(CaptureId)}/refund?";

    /// <summary>
    /// Gets the request method
    /// </summary>
    [JsonIgnore]
    public string Method => HttpMethods.Post;

    #endregion
}