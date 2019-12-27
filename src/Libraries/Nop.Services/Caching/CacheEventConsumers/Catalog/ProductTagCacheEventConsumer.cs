using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    public partial class ProductTagCacheEventConsumer : CacheEventConsumer<ProductTag>
    {
        public override void ClearCashe(ProductTag entity)
        {
            RemoveByPrefix(NopCatalogCachingDefaults.ProductTagPrefixCacheKey);
        }
    }
}
