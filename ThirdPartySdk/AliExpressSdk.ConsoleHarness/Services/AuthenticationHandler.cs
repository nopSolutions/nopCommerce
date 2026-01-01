using Microsoft.Extensions.Options;
using AliExpressSdk.Clients;
using AliExpressSdk.ConsoleHarness.Configuration;
using AliExpressSdk.Models;
using AliExpressSdk.Services;

namespace AliExpressSdk.ConsoleHarness.Services;

/// <summary>
/// Handles authentication flow for obtaining and refreshing tokens.
/// </summary>
public class AuthenticationHandler
{
    private readonly AliExpressOptions _options;
    private readonly SignatureService _signatureService;
    private readonly ApiCallPersistence _persistence;

    public AuthenticationHandler(
        IOptions<AliExpressOptions> options,
        SignatureService signatureService,
        ApiCallPersistence persistence)
    {
        _options = options.Value;
        _signatureService = signatureService;
        _persistence = persistence;
    }

    public async Task<TokenResponse?> CreateToken(string authCode)
    {
        const string apiPath = "/auth/token/create";
        
        // Create parameters dictionary with only the token-specific parameters
        // The Execute method in AESystemClient will add app_key, timestamp, sign_method, etc.
        var parameters = new Dictionary<string, string>
        {
            ["code"] = authCode
        };

        // Save request for debugging (with only the parameters we're passing to GenerateToken)
        await _persistence.SaveRequest(apiPath, parameters);
        
        var client = new AESystemClient(_options.AppKey, _options.AppSecret, "");
        var result = await client.GenerateToken(parameters);

        if (result.Ok && result.Data is { } data)
        {
            await _persistence.SaveResponse(apiPath, data);
            return ParseTokenResponse(data);
        }
        
        // If there's an error response, save it for debugging
        if (!result.Ok && result.ErrorResponse.HasValue)
        {
            await _persistence.SaveResponse(apiPath, result.ErrorResponse.Value);
            Console.Error.WriteLine($"API Error: {result.Message}");
            if (result.ErrorResponse.HasValue)
            {
                Console.Error.WriteLine($"Error Response: {result.ErrorResponse.Value}");
            }
        }

        return null;
    }

    private TokenResponse? ParseTokenResponse(System.Text.Json.JsonElement data)
    {
        try
        {
            // Check if this is an error response
            // Error responses have "code" field that is not "0" (success)
            if (data.TryGetProperty("code", out var codeProperty) && 
                codeProperty.ValueKind == System.Text.Json.JsonValueKind.String)
            {
                var code = codeProperty.GetString();
                if (!string.IsNullOrEmpty(code) && code != "0")
                {
                    // This is an error response
                    if (data.TryGetProperty("message", out var messageProperty))
                    {
                        Console.Error.WriteLine($"API Error ({code}): {messageProperty.GetString()}");
                    }
                    return null;
                }
            }
            
            // The response might be nested under a specific property or be at root
            var tokenData = data;
            
            // Try to find the token data - it might be nested
            if (data.ValueKind == System.Text.Json.JsonValueKind.Object)
            {
                // Check if there's a nested response structure
                foreach (var property in data.EnumerateObject())
                {
                    // Look for common response wrapper properties
                    if (property.Name.Contains("result") || property.Name.Contains("data"))
                    {
                        tokenData = property.Value;
                        break;
                    }
                }
            }

            var token = new TokenResponse
            {
                AccessToken = GetStringProperty(tokenData, "access_token"),
                RefreshToken = GetStringProperty(tokenData, "refresh_token"),
                AccountPlatform = GetStringProperty(tokenData, "account_platform"),
                UserNick = GetStringProperty(tokenData, "user_nick"),
                UserId = GetStringProperty(tokenData, "user_id"),
                SellerId = GetStringProperty(tokenData, "seller_id"),
                HavanaId = GetStringProperty(tokenData, "havana_id"),
                Account = GetStringProperty(tokenData, "account"),
                Locale = GetStringProperty(tokenData, "locale"),
                Sp = GetStringProperty(tokenData, "sp"),
                ExpiresIn = GetLongProperty(tokenData, "expires_in"),
                RefreshExpiresIn = GetLongProperty(tokenData, "refresh_expires_in"),
                ExpireTime = GetLongProperty(tokenData, "expire_time"),
                RefreshTokenValidTime = GetLongProperty(tokenData, "refresh_token_valid_time"),
                Code = GetStringProperty(tokenData, "code"),
                RequestId = GetStringProperty(tokenData, "request_id")
            };

            return token;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error parsing token response: {ex.Message}");
            return null;
        }
    }

    private string? GetStringProperty(System.Text.Json.JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out var property))
        {
            return property.ValueKind == System.Text.Json.JsonValueKind.String 
                ? property.GetString() 
                : property.ToString();
        }
        return null;
    }

    private long GetLongProperty(System.Text.Json.JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out var property))
        {
            if (property.ValueKind == System.Text.Json.JsonValueKind.Number)
            {
                return property.GetInt64();
            }
            // Try to parse as string in case it's returned as a string
            if (property.ValueKind == System.Text.Json.JsonValueKind.String)
            {
                if (long.TryParse(property.GetString(), out var value))
                {
                    return value;
                }
            }
        }
        return 0;
    }
}
