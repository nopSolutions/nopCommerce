namespace AliExpressSdk.Models;

/// <summary>
/// A token request with its computed signature, ready to be sent to the API.
/// </summary>
public class SignedTokenRequest
{
    /// <summary>
    /// The original token request.
    /// </summary>
    public required TokenRequest Request { get; init; }
    
    /// <summary>
    /// The computed signature for the request.
    /// </summary>
    public required string Signature { get; init; }
    
    /// <summary>
    /// Converts the signed request to a complete parameter dictionary.
    /// </summary>
    public IDictionary<string, string> ToParameters()
    {
        var parameters = Request.ToParameters();
        parameters["sign"] = Signature;
        return parameters;
    }
}
