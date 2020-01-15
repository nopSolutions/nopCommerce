using Nop.Core.Domain.Directory;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Directory
{
    /// <summary>
    /// Represents a country cache event consumer
    /// </summary>
    public partial class CountryCacheEventConsumer : CacheEventConsumer<Country>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(Country entity)
        {
            RemoveByPrefix(NopDirectoryCachingDefaults.CountriesPrefixCacheKey);
        }
    }
}