using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Api.Rest.Models.Requests;

/// <summary>
/// Store credentials accepted by the token endpoints
/// </summary>
/// <remarks>
/// Shared by both token endpoints. The administrator one only serves the Administrators role and the
/// customer one only serves registered customers, so which credential is acceptable is decided by the
/// controller rather than described here.
/// </remarks>
public record GetTokenRequest
{
    /// <summary>
    /// Gets or sets the account email address
    /// </summary>
    /// <remarks>
    /// Used as the username instead when the store enables usernames and <see cref="Username"/> is supplied
    /// </remarks>
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the account username
    /// </summary>
    /// <remarks>
    /// Only used when the store enables usernames, in which case it takes precedence over <see cref="Email"/>
    /// </remarks>
    public string Username { get; set; }

    /// <summary>
    /// Gets or sets the account password
    /// </summary>
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}
