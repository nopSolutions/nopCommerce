using Nop.Core.Domain.Discounts;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Discounts
{
    public partial class DiscountCacheEventConsumer : CacheEventConsumer<Discount>
    {
        public override void ClearCashe(Discount entity)
        {
            RemoveByPrefix(NopDiscountCachingDefaults.DiscountAllPrefixCacheKey);
            RemoveByPrefix(NopDiscountCachingDefaults.DiscountRequirementPrefixCacheKey);
            RemoveByPrefix(NopDiscountCachingDefaults.DiscountCategoryIdsPrefixCacheKey);
            RemoveByPrefix(NopDiscountCachingDefaults.DiscountManufacturerIdsPrefixCacheKey);
        }
    }
}
