using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;

namespace Nop.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a product specification attribute cache event consumer
    /// </summary>
    public partial class ProductSpecificationAttributeCacheEventConsumer : CacheEventConsumer<ProductSpecificationAttribute>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCache(ProductSpecificationAttribute entity)
        {
            await RemoveByPrefix(NopCatalogDefaults.ProductSpecificationAttributeByProductPrefix, entity.ProductId);
            await Remove(NopCatalogDefaults.SpecificationAttributeGroupByProductCacheKey, entity.ProductId);
        }
    }
}
