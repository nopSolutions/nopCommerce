namespace Nop.Plugin.Api.Rest.Models;

/// <summary>
/// A bearer token issued by one of the token endpoints
/// </summary>
/// <remarks>
/// One shape for both scopes. <see cref="CustomerId"/>, <see cref="CustomerGuid"/> and
/// <see cref="Username"/> are only populated for a customer token; an administrator token grants access
/// to every record rather than to one customer's, so it identifies no single customer.
/// </remarks>
public record ApiTokenDto
{
    /// <summary>
    /// Gets or sets the token to send as <c>Authorization: Bearer &lt;token&gt;</c>
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// Gets or sets the authorization scheme the token is sent with
    /// </summary>
    public string TokenType { get; set; }

    /// <summary>
    /// Gets or sets how many seconds the token stays valid from the moment it was issued
    /// </summary>
    public int ExpiresInSeconds { get; set; }

    /// <summary>
    /// Gets or sets the scope the token grants, either <c>ApiKey</c> or <c>CustomerToken</c>
    /// </summary>
    public string CredentialType { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the customer a customer token was issued to
    /// </summary>
    public int? CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the customer GUID, the same value the official nopCommerce Web API returns
    /// </summary>
    public string CustomerGuid { get; set; }

    /// <summary>
    /// Gets or sets the email address the token was issued against
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Gets or sets the HTTP methods guarded by a credential
    /// </summary>
    /// <remarks>
    /// Reported from the same rule the middleware enforces, so enabling the read setting is reflected
    /// here instead of still advertising GET as public
    /// </remarks>
    public string[] SecuredMethods { get; set; }
}