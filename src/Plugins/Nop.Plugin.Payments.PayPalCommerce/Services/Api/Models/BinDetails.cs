using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the Bank Identification Number (BIN) details used to fund a payment
/// </summary>
public class BinDetails
{
    #region Properties

    /// <summary>
    /// Gets or sets the Bank Identification Number (BIN) signifies the number that is being used to identify the granular level details (except the PII information) of the card.
    /// </summary>
    [JsonProperty(PropertyName = "bin")]
    public string Bin { get; set; }

    /// <summary>
    /// Gets or sets the issuer of the card instrument.
    /// </summary>
    [JsonProperty(PropertyName = "issuing_bank")]
    public string IssuingBank { get; set; }

    /// <summary>
    /// Gets or sets the [two-character ISO-3166-1 country code](https://developer.paypal.com/docs/integration/direct/rest/country-codes/) of the bank.
    /// </summary>
    [JsonProperty(PropertyName = "bin_country_code")]
    public string BinCountryCode { get; set; }

    /// <summary>
    /// Gets or sets the type of card product assigned to the BIN by the issuer. These values are defined by the issuer and may change over time.
    /// </summary>
    [JsonProperty(PropertyName = "products")]
    public List<string> Products { get; set; }

    #endregion
}