using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    /// <summary>
    /// Represents a product tag cache event consumer
    /// </summary>
    public partial class ProductTagCacheEventConsumer : CacheEventConsumer<ProductTag>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(ProductTag entity)
        {
            RemoveByPrefix(NopCatalogCachingDefaults.ProductTagPrefixCacheKey);
        }
    }
}
