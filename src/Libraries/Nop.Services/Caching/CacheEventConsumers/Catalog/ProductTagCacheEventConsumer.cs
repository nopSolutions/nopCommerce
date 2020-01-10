using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    public partial class ProductTagCacheEventConsumer : CacheEventConsumer<ProductTag>
    {
        protected override void ClearCache(ProductTag entity)
        {
            RemoveByPrefix(NopCatalogCachingDefaults.ProductTagPrefixCacheKey);
        }
    }
}
