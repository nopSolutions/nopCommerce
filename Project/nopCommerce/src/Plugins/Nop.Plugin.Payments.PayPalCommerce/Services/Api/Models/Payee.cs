using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the merchant who receives the funds and fulfills the order. The merchant is also known as the payee.
/// </summary>
public class Payee
{
    #region Properties

    /// <summary>
    /// Gets or sets the email address of merchant.
    /// </summary>
    [JsonProperty(PropertyName = "email_address")]
    public string EmailAddress { get; set; }

    /// <summary>
    /// Gets or sets the encrypted PayPal account ID of the merchant.
    /// </summary>
    [JsonProperty(PropertyName = "merchant_id")]
    public string MerchantId { get; set; }

    #endregion
}