using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    /// <summary>
    /// Represents a product manufacturer cache event consumer
    /// </summary>
    public partial class ProductManufacturerCacheEventConsumer : CacheEventConsumer<ProductManufacturer>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(ProductManufacturer entity)
        {
            var prefix = NopCatalogCachingDefaults.ProductManufacturersByProductPrefixCacheKey.ToCacheKey(entity.ProductId);
            RemoveByPrefix(prefix);

            prefix = NopCatalogCachingDefaults.ProductManufacturersByManufacturerPrefixCacheKey.ToCacheKey(entity.ManufacturerId);
            RemoveByPrefix(prefix);

            prefix = NopCatalogCachingDefaults.ProductPricePrefixCacheKey.ToCacheKey(entity.ProductId);
            RemoveByPrefix(prefix);
        }
    }
}
