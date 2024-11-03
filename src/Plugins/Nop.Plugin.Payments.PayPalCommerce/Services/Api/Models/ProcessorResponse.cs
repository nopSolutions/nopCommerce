using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the object that provides additional processor information for a direct credit card transaction
/// </summary>
public class ProcessorResponse
{
    #region Properties

    /// <summary>
    /// Gets or sets the address verification code for Visa, Discover, Mastercard, or American Express transactions.
    /// </summary>
    [JsonProperty(PropertyName = "avs_code")]
    public string AvsCode { get; set; }

    /// <summary>
    /// Gets or sets the card verification value code for for Visa, Discover, Mastercard, or American Express.
    /// </summary>
    [JsonProperty(PropertyName = "cvv_code")]
    public string CvvCode { get; set; }

    /// <summary>
    /// Gets or sets the processor response code for the non-PayPal payment processor errors.
    /// </summary>
    [JsonProperty(PropertyName = "response_code")]
    public string ResponseCode { get; set; }

    /// <summary>
    /// Gets or sets the declined payment transactions might have payment advice codes. The card networks, like Visa and Mastercard, return payment advice codes.
    /// </summary>
    [JsonProperty(PropertyName = "payment_advice_code")]
    public string PaymentAdviceCode { get; set; }

    #endregion
}