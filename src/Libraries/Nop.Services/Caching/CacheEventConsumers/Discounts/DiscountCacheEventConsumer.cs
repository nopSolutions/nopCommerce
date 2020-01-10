using Nop.Core.Domain.Discounts;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Discounts
{
    public partial class DiscountCacheEventConsumer : CacheEventConsumer<Discount>
    {
        protected override void ClearCache(Discount entity)
        {
            RemoveByPrefix(NopDiscountCachingDefaults.DiscountAllPrefixCacheKey);
            RemoveByPrefix(NopDiscountCachingDefaults.DiscountRequirementPrefixCacheKey);
            RemoveByPrefix(NopDiscountCachingDefaults.DiscountCategoryIdsPrefixCacheKey);
            RemoveByPrefix(NopDiscountCachingDefaults.DiscountManufacturerIdsPrefixCacheKey);
        }
    }
}
