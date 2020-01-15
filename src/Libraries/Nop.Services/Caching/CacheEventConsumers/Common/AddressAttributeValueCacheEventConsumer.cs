using Nop.Core.Domain.Common;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Common
{
    /// <summary>
    /// Represents a address attribute value cache event consumer
    /// </summary>
    public partial class AddressAttributeValueCacheEventConsumer : CacheEventConsumer<AddressAttributeValue>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(AddressAttributeValue entity)
        {
            RemoveByPrefix(NopCommonCachingDefaults.AddressAttributesPrefixCacheKey);
            RemoveByPrefix(NopCommonCachingDefaults.AddressAttributeValuesPrefixCacheKey);
        }
    }
}
