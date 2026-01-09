using System.Text.Json.Serialization;

namespace Nop.Plugin.DropShipping.AliExpress.Models;

/// <summary>
/// Response model for AliExpress auth token creation
/// </summary>
public class AuthTokenResponse
{
    [JsonPropertyName("refresh_token_valid_time")]
    public long RefreshTokenValidTime { get; set; }

    [JsonPropertyName("havana_id")]
    public string? HavanaId { get; set; }

    [JsonPropertyName("expire_time")]
    public long ExpireTime { get; set; }

    [JsonPropertyName("locale")]
    public string? Locale { get; set; }

    [JsonPropertyName("user_nick")]
    public string? UserNick { get; set; }

    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }

    [JsonPropertyName("user_id")]
    public string? UserId { get; set; }

    [JsonPropertyName("account_platform")]
    public string? AccountPlatform { get; set; }

    [JsonPropertyName("refresh_expires_in")]
    public int RefreshExpiresIn { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("sp")]
    public string? Sp { get; set; }

    [JsonPropertyName("seller_id")]
    public string? SellerId { get; set; }

    [JsonPropertyName("account")]
    public string? Account { get; set; }

    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("request_id")]
    public string? RequestId { get; set; }

    [JsonPropertyName("_trace_id_")]
    public string? TraceId { get; set; }
}
