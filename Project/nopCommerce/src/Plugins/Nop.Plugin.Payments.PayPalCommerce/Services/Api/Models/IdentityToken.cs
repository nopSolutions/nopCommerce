using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the client token to uniquely identify the customer
/// </summary>
public class IdentityToken
{
    #region Properties

    /// <summary>
    /// Gets or sets the client token.
    /// </summary>
    [JsonProperty(PropertyName = "client_token")]
    public string ClientToken { get; set; }

    /// <summary>
    /// Gets or sets the time (in seconds) until the client token expires
    /// </summary>
    [JsonProperty(PropertyName = "expires_in")]
    public int ExpiresIn { get; set; }

    /// <summary>
    /// Gets or sets the creation date and time
    /// </summary>
    [JsonIgnore]
    private DateTime CreateDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets a value indicating whether the client token is expired
    /// </summary>
    [JsonIgnore]
    public bool IsExpired => DateTime.UtcNow > CreateDate.Add(TimeSpan.FromSeconds(ExpiresIn));

    #endregion
}