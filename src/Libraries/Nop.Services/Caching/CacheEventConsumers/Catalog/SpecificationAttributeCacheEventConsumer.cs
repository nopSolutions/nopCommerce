using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    public partial class SpecificationAttributeCacheEventConsumer : CacheEventConsumer<SpecificationAttribute>
    {
        protected override void ClearCache(SpecificationAttribute entity)
        {
            RemoveByPrefix(NopCatalogCachingDefaults.ProductSpecificationAttributePrefixCacheKey);
        }
    }
}
