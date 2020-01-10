using Nop.Core.Domain.Vendors;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Vendors
{
    public partial class VendorAttributeValueCacheEventConsumer : CacheEventConsumer<VendorAttributeValue>
    {
        protected override void ClearCache(VendorAttributeValue entity)
        {
            RemoveByPrefix(NopVendorsServiceCachingDefaults.VendorAttributesPrefixCacheKey);
            RemoveByPrefix(NopVendorsServiceCachingDefaults.VendorAttributeValuesPrefixCacheKey);
        }
    }
}
