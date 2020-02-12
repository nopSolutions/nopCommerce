using Nop.Core.Domain.Discounts;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Discounts
{
    /// <summary>
    /// Represents a discount cache event consumer
    /// </summary>
    public partial class DiscountCacheEventConsumer : CacheEventConsumer<Discount>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(Discount entity)
        {
            RemoveByPrefix(NopDiscountCachingDefaults.DiscountAllPrefixCacheKey);
            RemoveByPrefix(NopDiscountCachingDefaults.DiscountRequirementPrefixCacheKey);
            RemoveByPrefix(NopDiscountCachingDefaults.DiscountCategoryIdsPrefixCacheKey);
            RemoveByPrefix(NopDiscountCachingDefaults.DiscountManufacturerIdsPrefixCacheKey);
        }
    }
}
