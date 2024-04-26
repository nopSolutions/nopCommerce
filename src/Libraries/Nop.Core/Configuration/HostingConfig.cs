namespace Nop.Core.Configuration;

/// <summary>
/// Represents hosting configuration parameters
/// </summary>
public partial class HostingConfig : IConfig
{
    /// <summary>
    /// Gets or sets a value indicating whether to use proxy servers and load balancers
    /// </summary>
    public bool UseProxy { get; protected set; }

    /// <summary>
    /// Gets or sets the header used to retrieve the value for the originating scheme (HTTP/HTTPS)
    /// </summary>
    public string ForwardedProtoHeaderName { get; protected set; } = string.Empty;

    /// <summary>
    /// Gets or sets the header used to retrieve the originating client IP
    /// </summary>
    public string ForwardedForHeaderName { get; protected set; } = string.Empty;

    /// <summary>
    /// Gets or sets addresses of known proxies to accept forwarded headers from
    /// </summary>
    public string KnownProxies { get; protected set; } = string.Empty;

    /// <summary>
    /// Gets or sets addresses of known networks to accept forwarded headers from
    /// </summary>
    public string KnownNetworks { get; protected set; } = string.Empty;
}