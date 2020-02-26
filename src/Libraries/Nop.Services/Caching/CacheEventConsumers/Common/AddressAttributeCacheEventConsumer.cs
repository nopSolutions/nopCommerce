using Nop.Core.Domain.Common;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Common
{
    /// <summary>
    /// Represents a address attribute cache event consumer
    /// </summary>
    public partial class AddressAttributeCacheEventConsumer : CacheEventConsumer<AddressAttribute>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(AddressAttribute entity)
        {
            Remove(NopCommonCachingDefaults.AddressAttributesAllCacheKey);

            var cacheKey = NopCommonCachingDefaults.AddressAttributeValuesAllCacheKey.ToCacheKey(entity);
            Remove(cacheKey);
        }
    }
}
