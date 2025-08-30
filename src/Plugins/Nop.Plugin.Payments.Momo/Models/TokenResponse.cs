using System;

namespace Nop.Plugin.Payments.Momo.Models;

public class TokenResponse
{
    private const string BEARER_TOKEN_TYPE = "Bearer";

    public string AccessToken { get; set; }
    
    // Always return "Bearer" regardless of what the API returns
    public string TokenType => BEARER_TOKEN_TYPE;

    public DateTime ExpiresAt { get; private set; }
    public int ExpiresIn 
    { 
        get => (int)(ExpiresAt - DateTime.UtcNow).TotalSeconds;
        set => ExpiresAt = DateTime.UtcNow.AddSeconds(value);
    }
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
}
