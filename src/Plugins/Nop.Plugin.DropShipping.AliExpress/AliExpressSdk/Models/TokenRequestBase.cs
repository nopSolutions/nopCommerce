using System.Globalization;

namespace AliExpressSdk.Models;

/// <summary>
/// Base class for all token-related requests containing common properties.
/// </summary>
public abstract class TokenRequestBase
{
    /// <summary>
    /// Application key provided by AliExpress.
    /// </summary>
    public required string AppKey { get; init; }
    
    /// <summary>
    /// Timestamp in milliseconds since Unix epoch.
    /// </summary>
    public required long Timestamp { get; init; }
    
    /// <summary>
    /// Signature method (e.g., "sha256").
    /// </summary>
    public required string SignMethod { get; init; }
    
    /// <summary>
    /// Creates a new timestamp for the current time.
    /// </summary>
    public static long CreateTimestamp()
        => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    
    /// <summary>
    /// Converts this request to a parameter dictionary suitable for signing.
    /// </summary>
    public virtual IDictionary<string, string> ToParameters()
    {
        return new Dictionary<string, string>
        {
            ["app_key"] = AppKey,
            ["timestamp"] = Timestamp.ToString(CultureInfo.InvariantCulture),
            ["sign_method"] = SignMethod
        };
    }
}
