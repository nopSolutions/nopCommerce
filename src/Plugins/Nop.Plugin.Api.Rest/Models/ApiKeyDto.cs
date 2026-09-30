namespace Nop.Plugin.Api.Rest.Models;

/// <summary>
/// API key issued to an authenticated administrator
/// </summary>
public record ApiKeyDto
{
    /// <summary>
    /// Gets or sets the API key to send in the <see cref="ApiRestDefaults.ApiKeyHeaderName"/> header
    /// </summary>
    public string ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the name of the header that carries the API key
    /// </summary>
    public string HeaderName { get; set; }

    /// <summary>
    /// Gets or sets the authentication scheme name
    /// </summary>
    public string TokenType { get; set; }

    /// <summary>
    /// Gets or sets the HTTP methods that require the API key
    /// </summary>
    public string[] SecuredMethods { get; set; }
}