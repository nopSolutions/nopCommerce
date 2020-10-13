using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;

namespace Nop.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a product attribute mapping cache event consumer
    /// </summary>
    public partial class ProductAttributeMappingCacheEventConsumer : CacheEventConsumer<ProductAttributeMapping>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCache(ProductAttributeMapping entity)
        {
            await Remove(NopCatalogDefaults.ProductAttributeMappingsByProductCacheKey, entity.ProductId);
            await Remove(NopCatalogDefaults.ProductAttributeValuesByAttributeCacheKey, entity);
            await Remove(NopCatalogDefaults.ProductAttributeCombinationsByProductCacheKey, entity.ProductId);
        }
    }
}
