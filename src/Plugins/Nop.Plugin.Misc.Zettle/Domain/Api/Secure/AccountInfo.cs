using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Secure;

/// <summary>
/// Represents merchant account details
/// </summary>
public class AccountInfo : ApiResponse
{
    /// <summary>
    /// Gets or sets the unique identifier as UUID version 1
    /// </summary>
    [JsonProperty(PropertyName = "uuid")]
    public string Uuid { get; set; }

    /// <summary>
    /// Gets or sets the owner unique identifier as UUID version 1
    /// </summary>
    [JsonProperty(PropertyName = "ownerUuid")]
    public string OwnerUuid { get; set; }

    /// <summary>
    /// Gets or sets the name
    /// </summary>
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the receipt name
    /// </summary>
    [JsonProperty(PropertyName = "receiptName")]
    public string ReceiptName { get; set; }

    /// <summary>
    /// Gets or sets the country
    /// </summary>
    [JsonProperty(PropertyName = "country")]
    public string Country { get; set; }

    /// <summary>
    /// Gets or sets the city
    /// </summary>
    [JsonProperty(PropertyName = "city")]
    public string City { get; set; }

    /// <summary>
    /// Gets or sets the zip code
    /// </summary>
    [JsonProperty(PropertyName = "zipCode")]
    public string ZipCode { get; set; }

    /// <summary>
    /// Gets or sets the address line
    /// </summary>
    [JsonProperty(PropertyName = "address")]
    public string Address { get; set; }

    /// <summary>
    /// Gets or sets the address line 2
    /// </summary>
    [JsonProperty(PropertyName = "addressLine2")]
    public string AddressLine2 { get; set; }

    /// <summary>
    /// Gets or sets the phone number
    /// </summary>
    [JsonProperty(PropertyName = "phoneNumber")]
    public string PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the website
    /// </summary>
    [JsonProperty(PropertyName = "webSite")]
    public string Website { get; set; }

    /// <summary>
    /// Gets or sets the contact email
    /// </summary>
    [JsonProperty(PropertyName = "contactEmail")]
    public string ContactEmail { get; set; }

    /// <summary>
    /// Gets or sets the receipt email
    /// </summary>
    [JsonProperty(PropertyName = "receiptEmail")]
    public string ReceiptEmail { get; set; }

    /// <summary>
    /// Gets or sets the language
    /// </summary>
    [JsonProperty(PropertyName = "language")]
    public string Language { get; set; }

    /// <summary>
    /// Gets or sets the currency
    /// </summary>
    [JsonProperty(PropertyName = "currency")]
    public string Currency { get; set; }

    /// <summary>
    /// Gets or sets the timezone
    /// </summary>
    [JsonProperty(PropertyName = "timeZone")]
    public string TimeZone { get; set; }

    /// <summary>
    /// Gets or sets the optional text (opening hours, Instagram account, other information as applicable)
    /// </summary>
    [JsonProperty(PropertyName = "optionalText")]
    public string OptionalText { get; set; }

    /// <summary>
    /// Gets or sets the legal entity type
    /// </summary>
    [JsonProperty(PropertyName = "legalEntityType")]
    public string LegalEntityType { get; set; }

    /// <summary>
    /// Gets or sets the legal entity number
    /// </summary>
    [JsonProperty(PropertyName = "legalntityNr")]
    public string LegalEntityNumber { get; set; }

    /// <summary>
    /// Gets or sets the legal name
    /// </summary>
    [JsonProperty(PropertyName = "legalName")]
    public string LegalName { get; set; }

    /// <summary>
    /// Gets or sets the legal address line
    /// </summary>
    [JsonProperty(PropertyName = "legalAddress")]
    public string LegalAddress { get; set; }

    /// <summary>
    /// Gets or sets the legal address line 2
    /// </summary>
    [JsonProperty(PropertyName = "legalAddressLine2")]
    public string LegalAddressLine2 { get; set; }

    /// <summary>
    /// Gets or sets the legal zip code
    /// </summary>
    [JsonProperty(PropertyName = "legalZipCode")]
    public string LegalZipCode { get; set; }

    /// <summary>
    /// Gets or sets the legal city
    /// </summary>
    [JsonProperty(PropertyName = "legalCity")]
    public string LegalCity { get; set; }

    /// <summary>
    /// Gets or sets the legal state
    /// </summary>
    [JsonProperty(PropertyName = "legalState")]
    public string LegalState { get; set; }

    /// <summary>
    /// Gets or sets the customer type
    /// </summary>
    [JsonProperty(PropertyName = "customerType")]
    public string CustomerType { get; set; }

    /// <summary>
    /// Gets or sets the customer status
    /// </summary>
    [JsonProperty(PropertyName = "customerStatus")]
    public string CustomerStatus { get; set; }

    /// <summary>
    /// Gets or sets the profile image URL
    /// </summary>
    [JsonProperty(PropertyName = "profileImageUrl")]
    public string ProfileImageUrl { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the VAT is used
    /// </summary>
    [JsonProperty(PropertyName = "usesVat")]
    public bool? VatUsed { get; set; }

    /// <summary>
    /// Gets or sets the taxation type
    /// </summary>
    [JsonProperty(PropertyName = "taxationType")]
    public string TaxationType { get; set; }

    /// <summary>
    /// Gets or sets the taxation mode
    /// </summary>
    [JsonProperty(PropertyName = "taxationMode")]
    public string TaxationMode { get; set; }

    /// <summary>
    /// Gets or sets the VAT number
    /// </summary>
    [JsonProperty(PropertyName = "vatNr")]
    public string VatNumber { get; set; }

    /// <summary>
    /// Gets or sets the VAT percentage
    /// </summary>
    [JsonProperty(PropertyName = "vatPercentage")]
    public decimal? VatPercentage { get; set; }

    /// <summary>
    /// Gets or sets the created date
    /// </summary>
    [JsonProperty(PropertyName = "created")]
    public DateTime? Created { get; set; }
}