using Nop.Core.Configuration;
using Nop.Core.Infrastructure;

namespace Nop.Core.Caching;

/// <summary>
/// Represents key for caching objects
/// </summary>
public partial class CacheKey
{
    #region Ctor

    /// <summary>
    /// Initialize a new instance with key and prefixes
    /// </summary>
    /// <param name="key">Key</param>
    public CacheKey(string key)
    {
        Key = key;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Create a new instance from the current one and fill it with passed parameters
    /// </summary>
    /// <param name="createCacheKeyParameters">Function to create parameters</param>
    /// <param name="keyObjects">Objects to create parameters</param>
    /// <returns>Cache key</returns>
    public virtual CacheKey Create(Func<object, object> createCacheKeyParameters, params object[] keyObjects)
    {
        var cacheKey = new CacheKey(Key);

        if (!keyObjects.Any())
            return cacheKey;

        cacheKey.Key = string.Format(cacheKey.Key, keyObjects.Select(createCacheKeyParameters).ToArray());

        return cacheKey;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets a cache key
    /// </summary>
    public string Key { get; protected set; }
    
    /// <summary>
    /// Gets or sets a cache time in minutes
    /// </summary>
    public int CacheTime { get; set; } = Singleton<AppSettings>.Instance.Get<CacheConfig>().DefaultCacheTime;

    #endregion
}