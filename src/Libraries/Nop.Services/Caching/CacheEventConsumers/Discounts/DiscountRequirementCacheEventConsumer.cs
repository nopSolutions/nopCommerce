using Nop.Core.Domain.Discounts;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Discounts
{
    public partial class DiscountRequirementCacheEventConsumer : CacheEventConsumer<DiscountRequirement>
    {
        public override void ClearCashe(DiscountRequirement entity)
        {
            _cacheManager.RemoveByPrefix(NopDiscountCachingDefaults.DiscountRequirementPrefixCacheKey);
        }
    }
}
