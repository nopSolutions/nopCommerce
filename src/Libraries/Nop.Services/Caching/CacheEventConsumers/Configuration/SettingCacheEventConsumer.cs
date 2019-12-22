using Nop.Core.Domain.Configuration;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Configuration
{
    public partial class SettingCacheEventConsumer : CacheEventConsumer<Setting>
    {
        public override void ClearCashe(Setting entity)
        {
            _cacheManager.RemoveByPrefix(NopCatalogCachingDefaults.ProductPricePrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogCachingDefaults.ProductCategoryIdsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogCachingDefaults.ProductManufacturerIdsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopDiscountCachingDefaults.DiscountCategoryIdsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopDiscountCachingDefaults.DiscountManufacturerIdsPrefixCacheKey);
        }
    }
}