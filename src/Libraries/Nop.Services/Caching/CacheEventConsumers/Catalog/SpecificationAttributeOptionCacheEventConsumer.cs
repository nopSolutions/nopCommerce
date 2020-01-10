using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    public partial class SpecificationAttributeOptionCacheEventConsumer : CacheEventConsumer<SpecificationAttributeOption>
    {
        protected override void ClearCache(SpecificationAttributeOption entity)
        {
            RemoveByPrefix(NopCatalogCachingDefaults.ProductSpecificationAttributePrefixCacheKey);
        }
    }
}
