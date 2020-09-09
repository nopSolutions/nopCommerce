using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;
using Nop.Services.Orders;

namespace Nop.Services.Catalog.Caching
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
        protected override async Task ClearCache(Product entity)
        {
            var prefix = _cacheKeyService.PrepareKeyPrefix(NopCatalogDefaults.ProductManufacturersByProductPrefixCacheKey, entity);
            await RemoveByPrefix(prefix);

            await Remove(NopCatalogDefaults.ProductsAllDisplayedOnHomepageCacheKey);
            await RemoveByPrefix(NopCatalogDefaults.ProductsByIdsPrefixCacheKey);

            prefix = _cacheKeyService.PrepareKeyPrefix(NopCatalogDefaults.ProductPricePrefixCacheKey, entity);
            await RemoveByPrefix(prefix);

            await RemoveByPrefix(NopOrderDefaults.ShoppingCartPrefixCacheKey);
        }
    }
}
