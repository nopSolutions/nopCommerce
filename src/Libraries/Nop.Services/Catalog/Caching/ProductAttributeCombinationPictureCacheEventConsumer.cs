using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;

namespace Nop.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a product attribute combination picture cache event consumer
    /// </summary>
    public partial class ProductAttributeCombinationPictureCacheEventConsumer : CacheEventConsumer<ProductAttributeCombinationPicture>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(ProductAttributeCombinationPicture entity)
        {
            await RemoveAsync(NopCatalogDefaults.ProductAttributeCombinationPicturesByCombinationCacheKey, entity.ProductAttributeCombinationId);
        }
    }
}
