using Nop.Core.Domain.Vendors;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Vendors
{
    public partial class VendorAttributeValueCacheEventConsumer : CacheEventConsumer<VendorAttributeValue>
    {
        public override void ClearCashe(VendorAttributeValue entity)
        {
            _cacheManager.RemoveByPrefix(NopVendorsServiceCachingDefaults.VendorAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopVendorsServiceCachingDefaults.VendorAttributeValuesPrefixCacheKey);
        }
    }
}
