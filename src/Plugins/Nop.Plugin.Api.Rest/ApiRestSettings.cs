using Nop.Core.Configuration;

namespace Nop.Plugin.Api.Rest;

/// <summary>
/// Represents the plugin settings, stored per store so each tenant can hold its own API key
/// </summary>
public class ApiRestSettings : ISettings
{
    /// <summary>
    /// Gets or sets the API key required by the write operations
    /// </summary>
    /// <remarks>
    /// When empty, the write operations are refused and the key endpoint reports that none is configured
    /// </remarks>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of API requests allowed per minute for a single client
    /// </summary>
    public int RateLimitPerMinute { get; set; } = 60;

    /// <summary>
    /// Gets or sets a value indicating whether the read operations also require the API key
    /// </summary>
    /// <remarks>
    /// When disabled, the catalog can be browsed anonymously but customer and order reads are public too
    /// </remarks>
    public bool RequireApiKeyForReads { get; set; }
}