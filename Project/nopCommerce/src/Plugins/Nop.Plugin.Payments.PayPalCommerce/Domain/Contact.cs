using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Domain;

/// <summary>
/// Represents the contact details
/// </summary>
public class Contact
{
    #region Properties

    /// <summary>
    /// Gets or sets the phone number for the contact
    /// </summary>
    [JsonProperty(PropertyName = "phoneNumber")]
    public string Phone { get; set; }

    /// <summary>
    /// Gets or sets the email address for the contact
    /// </summary>
    [JsonProperty(PropertyName = "emailAddress")]
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the contact’s given name
    /// </summary>
    [JsonProperty(PropertyName = "givenName")]
    public string FirstName { get; set; }

    /// <summary>
    /// Gets or sets the contact’s family name
    /// </summary>
    [JsonProperty(PropertyName = "familyName")]
    public string LastName { get; set; }

    /// <summary>
    /// Gets or sets the street portion of the address for the contact
    /// </summary>
    [JsonProperty(PropertyName = "addressLines")]
    public List<string> AddressLines { get; set; } = new();

    /// <summary>
    /// Gets or sets the city for the contact
    /// </summary>
    [JsonProperty(PropertyName = "locality")]
    public string City { get; set; }

    /// <summary>
    /// Gets or sets the subadministrative area (such as a county or other region)
    /// </summary>
    [JsonProperty(PropertyName = "subAdministrativeArea")]
    public string County { get; set; }

    /// <summary>
    /// Gets or sets the state for the contact
    /// </summary>
    [JsonProperty(PropertyName = "administrativeArea")]
    public string State { get; set; }

    /// <summary>
    /// Gets or sets the contact’s two-letter ISO 3166 country code
    /// </summary>
    [JsonProperty(PropertyName = "countryCode")]
    public string Country { get; set; }

    /// <summary>
    /// Gets or sets the zip code or postal code for the contact
    /// </summary>
    [JsonProperty(PropertyName = "postalCode")]
    public string PostalCode { get; set; }

    /// <summary>
    /// Gets or sets the shipping type that indicates how to ship purchased items (for shipping contact only)
    /// </summary>
    [JsonProperty(PropertyName = "shippingType")]
    public bool PickupInStore { get; set; }

    #endregion
}