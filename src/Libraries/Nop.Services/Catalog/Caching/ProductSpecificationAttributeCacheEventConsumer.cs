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
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(ProductSpecificationAttribute entity)
        {
            await RemoveByPrefixAsync(NopCatalogDefaults.ProductSpecificationAttributeByProductPrefix, entity.ProductId);
            await RemoveByPrefixAsync(NopCatalogDefaults.FilterableSpecificationAttributeOptionsPrefix);
            await RemoveAsync(NopCatalogDefaults.SpecificationAttributeGroupByProductCacheKey, entity.ProductId);
        }
    }
}
