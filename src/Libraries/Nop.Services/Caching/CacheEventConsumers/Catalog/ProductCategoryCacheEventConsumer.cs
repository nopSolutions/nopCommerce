using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    /// <summary>
    /// Represents a product category cache event consumer
    /// </summary>
    public partial class ProductCategoryCacheEventConsumer : CacheEventConsumer<ProductCategory>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(ProductCategory entity)
        {
            var prefix = NopCatalogCachingDefaults.ProductCategoriesByProductPrefixCacheKey.ToCacheKey(entity);
            RemoveByPrefix(prefix);

            RemoveByPrefix(NopCatalogCachingDefaults.CategoryNumberOfProductsPrefixCacheKey);
            
            prefix = NopCatalogCachingDefaults.ProductPricePrefixCacheKey.ToCacheKey(entity.ProductId);
            RemoveByPrefix(prefix);
        }
    }
}
