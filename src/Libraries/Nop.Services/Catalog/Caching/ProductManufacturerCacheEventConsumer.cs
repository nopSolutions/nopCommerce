using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;

namespace Nop.Services.Catalog.Caching
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
        protected override async Task ClearCache(ProductManufacturer entity)
        {
            await RemoveByPrefix(NopCatalogDefaults.ProductManufacturersByProductPrefix, entity.ProductId);
            await RemoveByPrefix(NopCatalogDefaults.ProductPricePrefix, entity.ProductId);
        }
    }
}
