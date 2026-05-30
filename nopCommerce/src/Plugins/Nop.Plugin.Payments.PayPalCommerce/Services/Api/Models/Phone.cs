using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the phone details
/// </summary>
public class Phone
{
    #region Properties

    /// <summary>
    /// Gets or sets the phone type.
    /// </summary>
    [JsonProperty(PropertyName = "phone_type")]
    public string PhoneType { get; set; }

    /// <summary>
    /// Gets or sets the phone number, in its canonical international [E.164 numbering plan format](https://www.itu.int/rec/T-REC-E.164/en).
    /// </summary>
    [JsonProperty(PropertyName = "phone_number")]
    public PhoneNumber PhoneNumber { get; set; }

    #endregion
}