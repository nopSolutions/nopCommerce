#nullable enable
using Newtonsoft.Json;

namespace Nop.Plugin.DropShipping.AliExpress.AliExpressSdk.Models;

/// <summary>
/// Response from token creation or refresh operations.
/// </summary>
///
public class TokenResponse
/*
 {"refresh_token_valid_time":1773134239000,"havana_id":"408081944474","expire_time":1770542239000,"locale":"zh_CN","user_nick":"de3031747246rfzae","access_token":"50000601246qlQZxgvHkuEtpDXcesipWukJlSh3GV1kEbEQofurbBXxoQJ1bbbef6aYi","refresh_token":"50001600f46DjmnoaadiyHjqAspgZGfU4sV1lglwL4ESXFEwDPrGfa6iG113e636aaf2","user_id":"4641738246","account_platform":"buyerApp","refresh_expires_in":5184000,"expires_in":2592000,"sp":"ae","seller_id":"4641738246","account":"jmvan.niekerk@gmail.com","code":"0","request_id":"21412efb17679502392691261","_trace_id_":"21411b6217679502392682779eb4b4"}
 */
{
    /// <summary>
    /// The access token for API calls.
    /// </summary>
    [JsonProperty("access_token")]
    public string? AccessToken { get; set; }
    
    /// <summary>
    /// The refresh token for obtaining new access tokens.
    /// </summary>
    [JsonProperty("refresh_token")]
    public string? RefreshToken { get; set; }
    
    /// <summary>
    /// Account platform identifier.
    /// </summary>
    [JsonProperty("account_platform")]
    public string? AccountPlatform { get; set; }
    
    /// <summary>
    /// User nickname.
    /// </summary>
    [JsonProperty("user_nick")]
    public string? UserNick { get; set; }
    
    /// <summary>
    /// User ID.
    /// </summary>
    [JsonProperty("user_id")]
    public string? UserId { get; set; }
    
    /// <summary>
    /// Seller ID (same as user_id).
    /// </summary>
    [JsonProperty("seller_id")]
    public string? SellerId { get; set; }
    
    /// <summary>
    /// Havana ID for the user.
    /// </summary>
    [JsonProperty("havana_id")]
    public string? HavanaId { get; set; }
    
    /// <summary>
    /// User account email or identifier.
    /// </summary>
    [JsonProperty("account")]
    public string? Account { get; set; }
    
    /// <summary>
    /// Locale setting.
    /// </summary>
    [JsonProperty("locale")]
    public string? Locale { get; set; }
    
    /// <summary>
    /// Service provider identifier.
    /// </summary>
    [JsonProperty("sp")]
    public string? Sp { get; set; }
    
    /// <summary>
    /// Access token expiration time in seconds.
    /// </summary>
    [JsonProperty("expires_in")]
    public long ExpiresIn { get; set; }
    
    /// <summary>
    /// Refresh token expiration time in seconds.
    /// </summary>
    [JsonProperty("refresh_expires_in")]
    public long RefreshExpiresIn { get; set; }
    
    /// <summary>
    /// Absolute expiration timestamp.
    /// </summary>
    [JsonProperty("expire_time")]
    public long ExpireTime { get; set; }
    
    /// <summary>
    /// Refresh token absolute expiration timestamp.
    /// </summary>
    [JsonProperty("refresh_token_valid_time")]
    public long RefreshTokenValidTime { get; set; }
    
    /// <summary>
    /// Response code.
    /// </summary>
    [JsonProperty("code")]
    public string? Code { get; set; }
    
    /// <summary>
    /// Request ID for tracking.
    /// </summary>
    [JsonProperty("request_id")]
    public string? RequestId { get; set; }
    
    /// <summary>
    /// Trace ID (optional, not previously mapped).
    /// </summary>
    [JsonProperty("_trace_id_")]
    public string? TraceId { get; set; }
}
#nullable restore
