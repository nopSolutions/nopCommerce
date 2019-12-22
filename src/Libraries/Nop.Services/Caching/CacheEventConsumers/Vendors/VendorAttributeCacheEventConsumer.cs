using Nop.Core.Domain.Vendors;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Vendors
{
    public partial class VendorAttributeCacheEventConsumer : CacheEventConsumer<VendorAttribute>
    {
        public override void ClearCashe(VendorAttribute entity)
        {
            _cacheManager.RemoveByPrefix(NopVendorsServiceCachingDefaults.VendorAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopVendorsServiceCachingDefaults.VendorAttributeValuesPrefixCacheKey);
        }
    }
}
