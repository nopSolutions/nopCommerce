using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the verify webhook signature
/// </summary>
public class WebhookSignature
{
    #region Properties

    /// <summary>
    /// Gets or sets the algorithm that PayPal uses to generate the signature and that you can use to verify the signature. Extract this value from the `PAYPAL-AUTH-ALGO` response header, which is received with the webhook notification.
    /// </summary>
    [JsonProperty(PropertyName = "auth_algo")]
    public string AuthAlgo { get; set; }

    /// <summary>
    /// Gets or sets the X.509 public key certificate. Download the certificate from this URL and use it to verify the signature. Extract this value from the `PAYPAL-CERT-URL` response header, which is received with the webhook notification.
    /// </summary>
    [JsonProperty(PropertyName = "cert_url")]
    public string CertUrl { get; set; }

    /// <summary>
    /// Gets or sets the ID of the HTTP transmission. Contained in the `PAYPAL-TRANSMISSION-ID` header of the notification message.
    /// </summary>
    [JsonProperty(PropertyName = "transmission_id")]
    public string TransmissionId { get; set; }

    /// <summary>
    /// Gets or sets the PayPal-generated asymmetric signature. Appears in the `PAYPAL-TRANSMISSION-SIG` header of the notification message.
    /// </summary>
    [JsonProperty(PropertyName = "transmission_sig")]
    public string TransmissionSig { get; set; }

    /// <summary>
    /// Gets or sets the date and time of the HTTP transmission, in [Internet date and time format](https://tools.ietf.org/html/rfc3339#section-5.6). Appears in the `PAYPAL-TRANSMISSION-TIME` header of the notification message.
    /// </summary>
    [JsonProperty(PropertyName = "transmission_time")]
    public string TransmissionTime { get; set; }

    /// <summary>
    /// Gets or sets the ID of the webhook as configured in your Developer Portal account.
    /// </summary>
    [JsonProperty(PropertyName = "webhook_id")]
    public string WebhookId { get; set; }

    /// <summary>
    /// Gets or sets the webhook event notification.
    /// </summary>
    [JsonProperty(PropertyName = "webhook_event")]
    public JRaw WebhookEvent { get; set; }

    #endregion
}