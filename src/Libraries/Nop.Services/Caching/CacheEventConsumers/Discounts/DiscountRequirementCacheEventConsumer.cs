using Nop.Core.Domain.Discounts;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Discounts
{
    public partial class DiscountRequirementCacheEventConsumer : CacheEventConsumer<DiscountRequirement>
    {
        public override void ClearCache(DiscountRequirement entity)
        {
            RemoveByPrefix(NopDiscountCachingDefaults.DiscountRequirementPrefixCacheKey);
        }
    }
}
