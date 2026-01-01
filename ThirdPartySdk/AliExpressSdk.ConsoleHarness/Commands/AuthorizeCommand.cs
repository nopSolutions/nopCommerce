using AliExpressSdk.ConsoleHarness.Configuration;
using AliExpressSdk.ConsoleHarness.Services;
using Microsoft.Extensions.Options;

namespace AliExpressSdk.ConsoleHarness.Commands;

/// <summary>
/// Handles the authorization command flow.
/// </summary>
public class AuthorizeCommand
{
    private readonly AuthorizationUrlBuilder _urlBuilder;
    private readonly ConsolePrompt _prompt;
    private readonly AuthenticationHandler _authHandler;
    private readonly IOptions<AliExpressOptions> _options;

    public AuthorizeCommand(
        AuthorizationUrlBuilder urlBuilder,
        ConsolePrompt prompt,
        AuthenticationHandler authHandler, 
        IOptions<AliExpressOptions> options)
    {
        _urlBuilder = urlBuilder;
        _prompt = prompt;
        _authHandler = authHandler;
        _options = options;
    }

    public async Task<int> ExecuteAsync(string? authCode = null)
    {
        var authUrl = _urlBuilder.BuildUrl(redirectUri: _options.Value.RedirectUri);
        authCode = authCode ?? _prompt.PromptForAuthCode(authUrl);
        
        Console.WriteLine();
        Console.WriteLine("Creating access token...");
        
        var token = await _authHandler.CreateToken(authCode);
        
        if (token != null)
        {
            Console.WriteLine("Authorization successful!");
            Console.WriteLine($"Access token: {token.AccessToken}");
            Console.WriteLine($"Refresh token: {token.RefreshToken}");
            Console.WriteLine($"User ID: {token.UserId}");
            Console.WriteLine($"Expires in: {token.ExpiresIn} seconds");
            return 0;
        }
        
        Console.Error.WriteLine("Authorization failed. Please check the error details in api-calls/auth-token-create/response.json");
        Console.Error.WriteLine("Note: Authorization codes expire in 30 minutes. You may need to generate a new code.");
        return 1;
    }
}
