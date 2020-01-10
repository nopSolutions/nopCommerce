using Nop.Core.Domain.Vendors;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Vendors
{
    public partial class VendorAttributeCacheEventConsumer : CacheEventConsumer<VendorAttribute>
    {
        protected override void ClearCache(VendorAttribute entity)
        {
            RemoveByPrefix(NopVendorsServiceCachingDefaults.VendorAttributesPrefixCacheKey);
            RemoveByPrefix(NopVendorsServiceCachingDefaults.VendorAttributeValuesPrefixCacheKey);
        }
    }
}
