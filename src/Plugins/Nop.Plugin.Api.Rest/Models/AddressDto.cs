using System;

namespace Nop.Plugin.Api.Rest.Models;

/// <summary>
/// An address attached to a customer
/// </summary>
public record AddressDto
{
    /// <summary>
    /// Gets or sets the address identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the first name
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Gets or sets the email address
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the company name
    /// </summary>
    public string? Company { get; set; }

    /// <summary>
    /// Gets or sets the country identifier
    /// </summary>
    public int? CountryId { get; set; }

    /// <summary>
    /// Gets or sets the state or province identifier
    /// </summary>
    public int? StateProvinceId { get; set; }

    /// <summary>
    /// Gets or sets the county
    /// </summary>
    public string? County { get; set; }

    /// <summary>
    /// Gets or sets the city
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Gets or sets the first address line
    /// </summary>
    public string? Address1 { get; set; }

    /// <summary>
    /// Gets or sets the second address line
    /// </summary>
    public string? Address2 { get; set; }

    /// <summary>
    /// Gets or sets the zip or postal code
    /// </summary>
    public string? ZipPostalCode { get; set; }

    /// <summary>
    /// Gets or sets the phone number
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the fax number
    /// </summary>
    public string? FaxNumber { get; set; }

    /// <summary>
    /// Gets or sets the date and time of instance creation (UTC)
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }
}
