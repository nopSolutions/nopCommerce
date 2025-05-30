using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the access token
/// </summary>
public class AccessToken
{
    #region Properties

    /// <summary>
    /// Gets or sets the token
    /// </summary>
    [JsonProperty(PropertyName = "access_token")]
    public string Token { get; set; }

    /// <summary>
    /// Gets or sets the token type
    /// </summary>
    [JsonProperty(PropertyName = "token_type")]
    public string TokenType { get; set; }

    /// <summary>
    /// Gets or sets the application id
    /// </summary>
    [JsonProperty(PropertyName = "app_id")]
    public string AppId { get; set; }

    /// <summary>
    /// Gets or sets the user ID token
    /// </summary>
    [JsonProperty(PropertyName = "id_token")]
    public string UserIdToken { get; set; }

    /// <summary>
    /// Gets or sets the scope
    /// </summary>
    [JsonProperty(PropertyName = "scope")]
    public string Scope { get; set; }

    /// <summary>
    /// Gets or sets the nonce
    /// </summary>
    [JsonProperty(PropertyName = "nonce")]
    public string Nonce { get; set; }

    /// <summary>
    /// Gets or sets the time (in seconds) until the access token expires
    /// </summary>
    [JsonProperty(PropertyName = "expires_in")]
    public int ExpiresIn { get; set; }

    /// <summary>
    /// Gets or sets the creation date and time
    /// </summary>
    [JsonIgnore]
    private DateTime CreateDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets a value indicating whether the access token is expired
    /// </summary>
    [JsonIgnore]
    public bool IsExpired => DateTime.UtcNow > CreateDate.Add(TimeSpan.FromSeconds(ExpiresIn));

    #endregion
}