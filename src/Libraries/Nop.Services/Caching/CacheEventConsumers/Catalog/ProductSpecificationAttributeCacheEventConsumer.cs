using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    public partial class ProductSpecificationAttributeCacheEventConsumer : CacheEventConsumer<ProductSpecificationAttribute>
    {
        public override void ClearCashe(ProductSpecificationAttribute entity)
        {
            RemoveByPrefix(NopCatalogCachingDefaults.ProductSpecificationAttributePrefixCacheKey);
        }
    }
}
