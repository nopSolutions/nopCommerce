namespace Nop.Core.Configuration;

/// <summary>
/// Represents cache configuration parameters
/// </summary>
public partial class CacheConfig : IConfig
{
    /// <summary>
    /// Gets or sets the default cache time in minutes
    /// </summary>
    public int DefaultCacheTime { get; protected set; } = 60;

    /// <summary>
    /// Gets or sets whether to disable linq2db query cache
    /// </summary>
    public bool LinqDisableQueryCache { get; protected set; } = false;
}