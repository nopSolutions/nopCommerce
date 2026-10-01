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
    /// Used as the username instead when the store enables usernames and <see cref="Username"/> is
    /// supplied. Nullable, and deliberately not marked required: this plugin compiles with the nullable
    /// annotations context on, and [ApiController] turns every non-nullable reference type property into
    /// an implicit [Required]. A bare "string" here would therefore be refused with a 400 before the action
    /// ever ran whenever the caller sent "username" alone, which is exactly what a store with usernames
    /// enabled and the official API's own clients do. The format is still checked when an address is
    /// supplied, and each action rejects a request that carries neither identifier.
    /// </remarks>
    [EmailAddress]
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the account username
    /// </summary>
    /// <remarks>
    /// Only used when the store enables usernames, in which case it takes precedence over
    /// <see cref="Email"/>. Nullable for the same reason as <see cref="Email"/>: a store without usernames
    /// sends the address alone, and a non-nullable string would have that request refused with a 400.
    /// </remarks>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the account password
    /// </summary>
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}
