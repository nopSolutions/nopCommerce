namespace Nop.Services.Installation.SampleData;

public partial class SampleAddress
{
    /// <summary>
    /// Gets or sets the state/province identifier
    /// </summary>
    public string StateProvince { get; set; }

    /// <summary>
    /// Gets or sets the city
    /// </summary>
    public string City { get; set; }

    /// <summary>
    /// Gets or sets the address 1
    /// </summary>
    public string Address1 { get; set; }

    /// <summary>
    /// Gets or sets the zip/postal code
    /// </summary>
    public string ZipPostalCode { get; set; }

    /// <summary>
    /// Three-letter country ISO code
    /// </summary>
    public string CountryThreeLetterIsoCode { get; set; }

    /// <summary>
    /// Gets or sets the company
    /// </summary>
    public string Company { get; set; }

    /// <summary>
    /// Gets or sets the phone number
    /// </summary>
    public string PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the first name
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// Gets or sets the email
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets fax number
    /// </summary>
    public string FaxNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the address 2
    /// </summary>
    public string Address2 { get; set; }
}