using Microsoft.Extensions.Caching.Hybrid;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;

namespace Nop.Services.Caching;

public abstract class SynchronizedHybridCache : HybridCache
{

}

/// <summary>
/// Represents a memory cache manager with distributed synchronization
/// </summary>
/// <remarks>
/// This class should be registered on IoC as singleton instance
/// </remarks>
public partial class SynchronizedMemoryCacheManager : StaticCacheManager
{
    public SynchronizedMemoryCacheManager(AppSettings appSettings,
        SynchronizedHybridCache memoryCache,
        ICacheKeyManager cacheKeyManager,
        IConcurrentCollection<object> concurrentCollection) : base(appSettings, memoryCache, cacheKeyManager, concurrentCollection)
    {
    }
}