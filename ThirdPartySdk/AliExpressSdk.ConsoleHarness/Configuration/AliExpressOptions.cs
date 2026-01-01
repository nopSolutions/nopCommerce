namespace AliExpressSdk.ConsoleHarness.Configuration;

/// <summary>
/// Configuration options for AliExpress API connection.
/// </summary>
public class AliExpressOptions
{
    public const string SectionName = "AliExpress";
    
    public string AppKey { get; set; } = string.Empty;
    public string AppSecret { get; set; } = string.Empty;
    public string? Session { get; set; }
    public string AuthorizationUrl { get; set; } = "https://openservice.aliexpress.com/oauth/authorize";
    public string ApiBaseUrl { get; set; } = "https://api-sg.aliexpress.com";
    public string RedirectUri { get; set; } = "https://localhost";
}
