using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Payments;

/// <summary>
/// Represents the request to capture an authorized payment
/// </summary>
public class CreateCaptureRequest : Capture, IAuthorizedRequest
{
    #region Properties

    /// <summary>
    /// Gets or sets the ID of the authorized payment
    /// </summary>
    [JsonIgnore]
    public string AuthorizationId { get; set; }

    /// <summary>
    /// Gets or sets the informational note about this settlement. Appears in both the payer's transaction history and the emails that the payer receives.
    /// </summary>
    [JsonProperty(PropertyName = "note_to_payer")]
    public string NoteToPayer { get; set; }

    /// <summary>
    /// Gets or sets the payment descriptor on the payer's account statement.
    /// </summary>
    [JsonProperty(PropertyName = "soft_descriptor")]
    public string SoftDescriptor { get; set; }

    /// <summary>
    /// Gets or sets the additional payment instructions to be consider during payment processing. This processing instruction is applicable for Capturing an order or Authorizing an Order.
    /// </summary>
    [JsonProperty(PropertyName = "payment_instruction")]
    public PaymentInstruction PaymentInstruction { get; set; }

    /// <summary>
    /// Gets the request path
    /// </summary>
    [JsonIgnore]
    public string Path => $"v2/payments/authorizations/{Uri.EscapeDataString(AuthorizationId)}/capture?";

    /// <summary>
    /// Gets the request method
    /// </summary>
    [JsonIgnore]
    public string Method => HttpMethods.Post;

    #endregion
}