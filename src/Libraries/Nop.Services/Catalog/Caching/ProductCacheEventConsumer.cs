using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Services.Caching;
using Nop.Services.Discounts;

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
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(Product entity, EntityEventType entityEventType)
        {
            await RemoveByPrefixAsync(NopCatalogDefaults.ProductManufacturersByProductPrefix, entity);
            await RemoveAsync(NopCatalogDefaults.ProductsHomepageCacheKey);
            await RemoveByPrefixAsync(NopCatalogDefaults.ProductPricePrefix, entity);
            await RemoveByPrefixAsync(NopEntityCacheDefaults<ShoppingCartItem>.AllPrefix);
            await RemoveByPrefixAsync(NopCatalogDefaults.FeaturedProductIdsPrefix);

            if (entityEventType == EntityEventType.Delete)
            {
                await RemoveByPrefixAsync(NopCatalogDefaults.FilterableSpecificationAttributeOptionsPrefix);
                await RemoveByPrefixAsync(NopCatalogDefaults.ManufacturersByCategoryPrefix);
            }

            await RemoveAsync(NopDiscountDefaults.AppliedDiscountsCacheKey, nameof(Product), entity);

            await base.ClearCacheAsync(entity, entityEventType);
        }
    }
}
