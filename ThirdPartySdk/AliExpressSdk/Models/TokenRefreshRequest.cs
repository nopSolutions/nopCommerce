namespace AliExpressSdk.Models;

/// <summary>
/// Request to refresh an existing token.
/// Used for /auth/token/refresh requests.
/// </summary>
public class TokenRefreshRequest : TokenRequestBase
{
    /// <summary>
    /// The refresh token obtained from a previous authentication.
    /// </summary>
    public required string RefreshToken { get; init; }
    
    /// <summary>
    /// API endpoint path (e.g., "/auth/token/refresh").
    /// </summary>
    public required string ApiPath { get; init; }
    
    public override IDictionary<string, string> ToParameters()
    {
        var parameters = base.ToParameters();
        parameters["refresh_token"] = RefreshToken;
        return parameters;
    }
}
