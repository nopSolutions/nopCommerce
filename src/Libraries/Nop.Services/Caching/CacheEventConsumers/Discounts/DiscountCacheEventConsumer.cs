using Nop.Core.Domain.Discounts;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Discounts
{
    public partial class DiscountCacheEventConsumer : CacheEventConsumer<Discount>
    {
        public override void ClearCashe(Discount entity)
        {
            _cacheManager.RemoveByPrefix(NopDiscountCachingDefaults.DiscountAllPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopDiscountCachingDefaults.DiscountRequirementPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopDiscountCachingDefaults.DiscountCategoryIdsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopDiscountCachingDefaults.DiscountManufacturerIdsPrefixCacheKey);
        }
    }
}
