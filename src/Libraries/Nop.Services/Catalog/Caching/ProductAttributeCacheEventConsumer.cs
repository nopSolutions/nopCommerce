using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;

namespace Nop.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a product attribute cache event consumer
    /// </summary>
    public partial class ProductAttributeCacheEventConsumer : CacheEventConsumer<ProductAttribute>
    {
        /// <summary>
        /// entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        protected override async Task ClearCache(ProductAttribute entity, EntityEventType entityEventType)
        {
            if (entityEventType != EntityEventType.Delete) 
                return;

            await RemoveByPrefix(NopCatalogDefaults.ProductAttributeMappingsPrefixCacheKey);
            await RemoveByPrefix(NopCatalogDefaults.ProductAttributeValuesAllPrefixCacheKey);
            await RemoveByPrefix(NopCatalogDefaults.ProductAttributeCombinationsAllPrefixCacheKey);
        }
    }
}
