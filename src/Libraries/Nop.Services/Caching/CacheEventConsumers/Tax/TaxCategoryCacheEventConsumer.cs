using Nop.Core.Domain.Tax;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Tax
{
    public partial class TaxCategoryCacheEventConsumer : CacheEventConsumer<TaxCategory>
    {
        protected override void ClearCache(TaxCategory entity)
        {
            RemoveByPrefix(NopTaxCachingDefaults.TaxCategoriesPrefixCacheKey);
        }
    }
}
