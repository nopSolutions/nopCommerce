using Nop.Core.Configuration;

namespace Nop.Core.Caching;

/// <summary>
/// Represents a memory cache manager with distributed synchronization
/// </summary>
/// <remarks>
/// This class should be registered on IoC as singleton instance
/// </remarks>
public partial class SynchronizedMemoryCacheManager : MemoryCacheManager
{
    public SynchronizedMemoryCacheManager(AppSettings appSettings,
        ISynchronizedMemoryCache memoryCache,
        ICacheKeyManager cacheKeyManager) : base(appSettings, memoryCache, cacheKeyManager)
    {
    }
}