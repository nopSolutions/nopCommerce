using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the person name
/// </summary>
public class Name
{
    #region Properties

    /// <summary>
    /// Gets or sets the party's given, or first, name.
    /// </summary>
    [JsonProperty(PropertyName = "given_name")]
    public string GivenName { get; set; }

    /// <summary>
    /// Gets or sets the party's surname or family name. Also known as the last name. Required when the party is a person. Use also to store multiple surnames including the matronymic, or mother's, surname.
    /// </summary>
    [JsonProperty(PropertyName = "surname")]
    public string Surname { get; set; }

    /// <summary>
    /// Gets or sets the party's full name.
    /// </summary>
    [JsonProperty(PropertyName = "full_name")]
    public string FullName { get; set; }

    #endregion
}