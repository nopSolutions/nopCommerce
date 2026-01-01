namespace AliExpressSdk.Models;

/// <summary>
/// Response from token creation or refresh operations.
/// </summary>
public class TokenResponse
{
    /// <summary>
    /// The access token for API calls.
    /// </summary>
    public string? AccessToken { get; set; }
    
    /// <summary>
    /// The refresh token for obtaining new access tokens.
    /// </summary>
    public string? RefreshToken { get; set; }
    
    /// <summary>
    /// Account platform identifier.
    /// </summary>
    public string? AccountPlatform { get; set; }
    
    /// <summary>
    /// User nickname.
    /// </summary>
    public string? UserNick { get; set; }
    
    /// <summary>
    /// User ID.
    /// </summary>
    public string? UserId { get; set; }
    
    /// <summary>
    /// Seller ID (same as user_id).
    /// </summary>
    public string? SellerId { get; set; }
    
    /// <summary>
    /// Havana ID for the user.
    /// </summary>
    public string? HavanaId { get; set; }
    
    /// <summary>
    /// User account email or identifier.
    /// </summary>
    public string? Account { get; set; }
    
    /// <summary>
    /// Locale setting.
    /// </summary>
    public string? Locale { get; set; }
    
    /// <summary>
    /// Service provider identifier.
    /// </summary>
    public string? Sp { get; set; }
    
    /// <summary>
    /// Access token expiration time in seconds.
    /// </summary>
    public long ExpiresIn { get; set; }
    
    /// <summary>
    /// Refresh token expiration time in seconds.
    /// </summary>
    public long RefreshExpiresIn { get; set; }
    
    /// <summary>
    /// Absolute expiration timestamp.
    /// </summary>
    public long ExpireTime { get; set; }
    
    /// <summary>
    /// Refresh token absolute expiration timestamp.
    /// </summary>
    public long RefreshTokenValidTime { get; set; }
    
    /// <summary>
    /// Response code.
    /// </summary>
    public string? Code { get; set; }
    
    /// <summary>
    /// Request ID for tracking.
    /// </summary>
    public string? RequestId { get; set; }
}
