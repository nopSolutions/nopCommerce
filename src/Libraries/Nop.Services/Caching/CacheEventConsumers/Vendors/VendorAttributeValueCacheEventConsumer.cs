using Nop.Core.Domain.Vendors;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Vendors
{
    /// <summary>
    /// Represents a vendor attribute value cache event consumer
    /// </summary>
    public partial class VendorAttributeValueCacheEventConsumer : CacheEventConsumer<VendorAttributeValue>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(VendorAttributeValue entity)
        {
            var cacheKey = NopVendorCachingDefaults.VendorAttributeValuesAllCacheKey.ToCacheKey(entity.VendorAttributeId);

            Remove(cacheKey);
        }
    }
}
