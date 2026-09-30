using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Api.Rest.Models.Requests;

/// <summary>
/// Credentials accepted by the API key endpoint
/// </summary>
public record GetApiKeyRequest
{
    /// <summary>
    /// Gets or sets the administrator email address
    /// </summary>
    /// <remarks>
    /// Used as the username instead when the store enables usernames and <see cref="Username"/> is supplied
    /// </remarks>
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the administrator username
    /// </summary>
    /// <remarks>
    /// Only used when the store enables usernames, in which case it takes precedence over <see cref="Email"/>
    /// </remarks>
    public string Username { get; set; }

    /// <summary>
    /// Gets or sets the administrator password
    /// </summary>
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}