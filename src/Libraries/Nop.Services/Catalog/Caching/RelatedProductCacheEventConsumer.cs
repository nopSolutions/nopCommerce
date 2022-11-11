using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;

namespace Nop.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a related product cache event consumer
    /// </summary>
    public partial class RelatedProductCacheEventConsumer : CacheEventConsumer<RelatedProduct>
    {
        /// <summary>
        /// entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(RelatedProduct entity)
        {
            await RemoveByPrefixAsync(NopCatalogDefaults.RelatedProductsPrefix, entity.ProductId1);
        }
    }
}
