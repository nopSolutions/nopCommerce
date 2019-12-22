using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    public partial class ProductAttributeValueCacheEventConsumer : CacheEventConsumer<ProductAttributeValue>
    {
        public override void ClearCashe(ProductAttributeValue entity)
        {
            _cacheManager.RemoveByPrefix(NopCatalogCachingDefaults.ProductAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogCachingDefaults.ProductAttributeMappingsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogCachingDefaults.ProductAttributeValuesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogCachingDefaults.ProductAttributeCombinationsPrefixCacheKey);
        }
    }
}
