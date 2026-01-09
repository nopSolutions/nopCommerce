using System.Security.Cryptography;
using System.Text;

namespace AliExpressSdk.Services;

/// <summary>
/// Service for signing AliExpress API requests according to the documented specification.
/// </summary>
public class SignatureService
{
    private readonly string _appSecret;

    public SignatureService(string appSecret)
    {
        if (string.IsNullOrWhiteSpace(appSecret))
        {
            throw new ArgumentException("App secret cannot be null or empty", nameof(appSecret));
        }
        
        _appSecret = appSecret;
    }

    /// <summary>
    /// Signs a request according to AliExpress API signature algorithm.
    /// Steps:
    /// 1. Sort all parameters by name
    /// 2. Concatenate parameter names and values
    /// 3. Prepend the API path (for system APIs that use /path format)
    /// 4. Compute HMAC-SHA256 hash
    /// 5. Return hex string in uppercase
    /// </summary>
    public string Sign(string apiPath, IDictionary<string, string> parameters)
    {
        if (string.IsNullOrWhiteSpace(apiPath))
        {
            throw new ArgumentException("API path cannot be null or empty", nameof(apiPath));
        }
        
        var baseString = BuildSignatureBaseString(apiPath, parameters);
        return ComputeSignature(baseString);
    }

    private string BuildSignatureBaseString(string apiPath, IDictionary<string, string> parameters)
    {
        var sortedParams = parameters
            .Where(kvp => !string.IsNullOrEmpty(kvp.Value))
            .OrderBy(kvp => kvp.Key, StringComparer.Ordinal);

        var sb = new StringBuilder();
        
        // For system APIs (those starting with /), prepend the API path
        if (apiPath.StartsWith('/'))
        {
            sb.Append(apiPath);
        }

        foreach (var kvp in sortedParams)
        {
            sb.Append(kvp.Key);
            sb.Append(kvp.Value);
        }

        return sb.ToString();
    }

    private string ComputeSignature(string baseString)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_appSecret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(baseString));
        return BitConverter.ToString(hash).Replace("-", string.Empty);
    }
}
