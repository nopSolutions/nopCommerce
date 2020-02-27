using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    /// <summary>
    /// Represents a product-product tag mapping  cache event consumer
    /// </summary>
    public partial class ProductProductTagMappingCacheEventConsumer : CacheEventConsumer<ProductProductTagMapping>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(ProductProductTagMapping entity)
        {
            Remove(NopCatalogCachingDefaults.ProductTagAllByProductIdCacheKey.ToCacheKey(entity.ProductId));
        }
    }
}