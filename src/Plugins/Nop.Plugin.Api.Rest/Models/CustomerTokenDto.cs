namespace Nop.Plugin.Api.Rest.Models;

/// <summary>
/// A token identifying a store customer
/// </summary>
public record CustomerTokenDto
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
    /// Gets or sets the identifier of the customer the token was issued to
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the customer GUID, the same value the official nopCommerce Web API returns
    /// </summary>
    public string? CustomerGuid { get; set; }

    /// <summary>
    /// Gets or sets the email address the token was issued against
    /// </summary>
    public string? Username { get; set; }
}
