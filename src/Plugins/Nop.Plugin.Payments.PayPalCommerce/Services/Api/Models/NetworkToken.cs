using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the 3rd party network token
/// </summary>
public class NetworkToken
{
    #region Properties

    /// <summary>
    /// Gets or sets the third party network token number.
    /// </summary>
    [JsonProperty(PropertyName = "number")]
    public string Number { get; set; }

    /// <summary>
    /// Gets or sets the Encrypted one-time use value that's sent along with Network Token. This field is not required to be present for recurring transactions.
    /// </summary>
    [JsonProperty(PropertyName = "cryptogram")]
    public string Cryptogram { get; set; }

    /// <summary>
    /// Gets or sets the TRID, or a Token Requestor ID, is an identifier used by merchants to request network tokens from card networks. A TRID is a precursor to obtaining a network token for a credit card primary account number (PAN), and will aid in enabling secure card on file (COF) payments and reducing fraud.
    /// </summary>
    [JsonProperty(PropertyName = "token_requestor_id")]
    public string TokenRequestorId { get; set; }

    /// <summary>
    /// Gets or sets the year and month, in ISO-8601 `YYYY-MM` date format. See [Internet date and time format](https://tools.ietf.org/html/rfc3339#section-5.6).
    /// </summary>
    [JsonProperty(PropertyName = "expiry")]
    public string Expiry { get; set; }

    /// <summary>
    /// Gets or sets the Electronic Commerce Indicator (ECI). The ECI value is part of the 2 data elements that indicate the transaction was processed electronically. This should be passed on the authorization transaction to the Gateway/Processor.
    /// </summary>
    [JsonProperty(PropertyName = "eci_flag")]
    public string EciFlag { get; set; }

    #endregion
}