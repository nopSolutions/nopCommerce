using System;

namespace Nop.Plugin.Api.Rest.Models;

/// <summary>
/// The profile of the customer a customer token was issued to
/// </summary>
/// <remarks>
/// Deliberately narrower than <see cref="CustomerDto"/>, which is the admin view of a customer. A
/// storefront client only needs to know who it is acting for, not the store's record of that person.
/// </remarks>
public record MeDto
{
    /// <summary>
    /// Gets or sets the customer identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the email address
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the username, when the store enables usernames
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the first name
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Gets or sets the company name
    /// </summary>
    public string? Company { get; set; }

    /// <summary>
    /// Gets or sets the phone number
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the account is active
    /// </summary>
    public bool Active { get; set; }

    /// <summary>
    /// Gets or sets the date and time of instance creation (UTC)
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the date and time of the last recorded activity (UTC)
    /// </summary>
    public DateTime LastActivityDateUtc { get; set; }
}
