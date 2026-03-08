using Newtonsoft.Json;
namespace Nop.Plugin.Payments.Momo.Models;

public class TokenResponse
{
    private const string BEARER_TOKEN_TYPE = "Bearer";

    [JsonProperty("access_token")]
    public string AccessToken { get; set; }
    
    // Always return "Bearer" regardless of what the API returns
    [JsonProperty("token_type")]
    public string TokenType => BEARER_TOKEN_TYPE;

    public DateTime ExpiresAt { get; private set; }

    [JsonProperty("expires_in")]
    public int ExpiresIn 
    { 
        get => (int)(ExpiresAt - DateTime.UtcNow).TotalSeconds;
        set => ExpiresAt = DateTime.UtcNow.AddSeconds(value);
    }
    
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
}
