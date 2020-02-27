using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    /// <summary>
    /// Represents a product cache event consumer
    /// </summary>
    public partial class ProductCacheEventConsumer : CacheEventConsumer<Product>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(Product entity)
        {
            var prefix = NopCatalogCachingDefaults.ProductManufacturersByProductPrefixCacheKey.ToCacheKey(entity);
            RemoveByPrefix(prefix);

            Remove(NopCatalogCachingDefaults.ProductsAllDisplayedOnHomepageCacheKey);
            RemoveByPrefix(NopCatalogCachingDefaults.ProductsByIdsPrefixCacheKey);

            prefix = NopCatalogCachingDefaults.ProductPricePrefixCacheKey.ToCacheKey(entity);
            RemoveByPrefix(prefix);

            RemoveByPrefix(NopOrderCachingDefaults.ShoppingCartPrefixCacheKey);
        }
    }
}
