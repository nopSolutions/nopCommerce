namespace AliExpressSdk.Models;

/// <summary>
/// Token request containing parameters needed for signing.
/// Used for /auth/token/create requests.
/// </summary>
public class TokenRequest : TokenRequestBase
{
    /// <summary>
    /// Authorization code obtained from the OAuth flow.
    /// </summary>
    public required string Code { get; init; }
    
    /// <summary>
    /// Optional UUID for the request.
    /// </summary>
    public string? Uuid { get; init; }
    
    /// <summary>
    /// API endpoint path (e.g., "/auth/token/create").
    /// </summary>
    public required string ApiPath { get; init; }
    
    public override IDictionary<string, string> ToParameters()
    {
        var parameters = base.ToParameters();
        parameters["code"] = Code;
        
        if (!string.IsNullOrWhiteSpace(Uuid))
        {
            parameters["uuid"] = Uuid;
        }
        
        return parameters;
    }
}
