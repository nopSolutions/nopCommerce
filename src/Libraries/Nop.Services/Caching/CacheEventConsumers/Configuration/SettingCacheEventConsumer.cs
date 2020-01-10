using Nop.Core.Domain.Configuration;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Configuration
{
    public partial class SettingCacheEventConsumer : CacheEventConsumer<Setting>
    {
        protected override void ClearCache(Setting entity)
        {
            RemoveByPrefix(NopCatalogCachingDefaults.ProductPricePrefixCacheKey);
            RemoveByPrefix(NopCatalogCachingDefaults.ProductCategoryIdsPrefixCacheKey);
            RemoveByPrefix(NopCatalogCachingDefaults.ProductManufacturerIdsPrefixCacheKey);
            RemoveByPrefix(NopDiscountCachingDefaults.DiscountCategoryIdsPrefixCacheKey);
            RemoveByPrefix(NopDiscountCachingDefaults.DiscountManufacturerIdsPrefixCacheKey);
        }
    }
}