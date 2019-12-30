using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    public partial class ProductAttributeCombinationCacheEventConsumer : CacheEventConsumer<ProductAttributeCombination>
    {
        public override void ClearCashe(ProductAttributeCombination entity)
        {
            RemoveByPrefix(NopCatalogCachingDefaults.ProductAttributesPrefixCacheKey);
            RemoveByPrefix(NopCatalogCachingDefaults.ProductAttributeMappingsPrefixCacheKey);
            RemoveByPrefix(NopCatalogCachingDefaults.ProductAttributeValuesPrefixCacheKey);
            RemoveByPrefix(NopCatalogCachingDefaults.ProductAttributeCombinationsPrefixCacheKey);
        }
    }
}
