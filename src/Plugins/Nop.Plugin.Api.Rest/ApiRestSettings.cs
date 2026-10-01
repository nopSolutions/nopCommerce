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
    /// Gets or sets a value indicating whether the read operations also require a credential
    /// </summary>
    /// <remarks>
    /// When disabled, the catalog can be browsed anonymously but the back office customer and order reads
    /// are public too. A customer's own data under /api/rest/customer/me is never public either way, and
    /// the storefront catalog moves between anonymous and needing a customer token with this setting. The
    /// API key is not accepted on any public store route, so this does not govern it.
    /// </remarks>
    public bool RequireApiKeyForReads { get; set; }

    /// <summary>
    /// Gets or sets how long a token issued to an administrator stays valid
    /// </summary>
    /// <remarks>
    /// Shorter than the customer lifetime by default, because an administrator token grants catalog,
    /// order and customer write access, while a customer token is scoped to that customer's own data.
    /// </remarks>
    public int AdminTokenLifetimeHours { get; set; } = 24;

    /// <summary>
    /// Gets or sets how many days a token issued to a store customer stays valid
    /// </summary>
    /// <remarks>
    /// Seven days by default, matching the lifetime the official nopCommerce Web API issues its own
    /// tokens for. Both lifetimes only take effect for tokens issued after the setting was saved.
    /// </remarks>
    public int CustomerTokenLifetimeDays { get; set; } = 7;

    /// <summary>
    /// Gets how long a newly issued administrator token stays valid
    /// </summary>
    /// <remarks>
    /// Falls back to a day if the stored value is not positive, so a hand edited or partially migrated
    /// setting cannot mint a token that is already expired or that never expires
    /// </remarks>
    public System.TimeSpan AdminTokenLifetime
        => System.TimeSpan.FromHours(AdminTokenLifetimeHours > 0 ? AdminTokenLifetimeHours : 24);

    /// <summary>
    /// Gets how long a newly issued customer token stays valid
    /// </summary>
    /// <inheritdoc cref="AdminTokenLifetime" path="/remarks"/>
    public System.TimeSpan CustomerTokenLifetime
        => System.TimeSpan.FromDays(CustomerTokenLifetimeDays > 0 ? CustomerTokenLifetimeDays : 7);
}