using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the phone number
/// </summary>
public class PhoneNumber
{
    #region Properties

    /// <summary>
    /// Gets or sets the national number, in its canonical international [E.164 numbering plan format](https://www.itu.int/rec/T-REC-E.164/en). The combined length of the country calling code (CC) and the national number must not be greater than 15 digits. The national number consists of a national destination code (NDC) and subscriber number (SN).
    /// </summary>
    [JsonProperty(PropertyName = "national_number")]
    public string NationalNumber { get; set; }

    #endregion
}