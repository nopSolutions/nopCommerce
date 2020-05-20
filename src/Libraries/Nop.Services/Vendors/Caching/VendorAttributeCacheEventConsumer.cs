using Nop.Core.Domain.Vendors;
using Nop.Services.Caching;

namespace Nop.Services.Vendors.Caching
{
    /// <summary>
    /// Represents a vendor attribute cache event consumer
    /// </summary>
    public partial class VendorAttributeCacheEventConsumer : CacheEventConsumer<VendorAttribute>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(VendorAttribute entity)
        {
            base.Remove(NopVendorDefaults.VendorAttributesAllCacheKey);

            var cacheKey = _cacheKeyService.PrepareKey(NopVendorDefaults.VendorAttributeValuesAllCacheKey, entity);

            Remove(cacheKey);
        }
    }
}
