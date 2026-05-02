using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the addres details
/// </summary>
public class Address
{
    #region Properties

    /// <summary>
    /// Gets or sets the first line of the address, such as number and street, for example, `173 Drury Lane`. Needed for data entry, and Compliance and Risk checks. This field needs to pass the full address.
    /// </summary>
    [JsonProperty(PropertyName = "address_line_1")]
    public string AddressLine1 { get; set; }

    /// <summary>
    /// Gets or sets the second line of the address, for example, a suite or apartment number.
    /// </summary>
    [JsonProperty(PropertyName = "address_line_2")]
    public string AddressLine2 { get; set; }

    /// <summary>
    /// Gets or sets the city, town, or village. Smaller than `admin_area_level_1`.
    /// </summary>
    [JsonProperty(PropertyName = "admin_area_2")]
    public string AdminArea2 { get; set; }

    /// <summary>
    /// Gets or sets the highest-level sub-division in a country, which is usually a province, state, or ISO-3166-2 subdivision. This data is formatted for postal delivery, for example, `CA` and not `California`.
    /// </summary>
    [JsonProperty(PropertyName = "admin_area_1")]
    public string AdminArea1 { get; set; }

    /// <summary>
    /// Gets or sets the postal code, which is the ZIP code or equivalent. Typically required for countries with a postal code or an equivalent. See [postal code](https://en.wikipedia.org/wiki/Postal_code).
    /// </summary>
    [JsonProperty(PropertyName = "postal_code")]
    public string PostalCode { get; set; }

    /// <summary>
    /// Gets or sets the [2-character ISO 3166-1 code](https://developer.paypal.com/api/rest/reference/country-codes/) that identifies the country or region.
    /// </summary>
    [JsonProperty(PropertyName = "country_code")]
    public string CountryCode { get; set; }

    #endregion
}