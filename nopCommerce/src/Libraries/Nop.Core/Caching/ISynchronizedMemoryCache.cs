using Microsoft.Extensions.Caching.Memory;

namespace Nop.Core.Caching;

/// <summary>
/// Represents a local in-memory cache with distributed synchronization
/// </summary>
public partial interface ISynchronizedMemoryCache : IMemoryCache
{
    /// <summary>
    /// Clear all cache data
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task ClearCacheAsync();

    /// <summary>
    /// Remove items by prefix
    /// </summary>
    /// <param name="prefix">Prefix to remove cache items</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task RemoveByPrefixAsync(string prefix);
}