using Nop.Core.Domain.Discounts;
using Nop.Services.Caching;

namespace Nop.Services.Discounts.Caching
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
            RemoveByPrefix(NopDiscountDefaults.DiscountAllPrefixCacheKey);
            var cacheKey = NopDiscountDefaults.DiscountRequirementModelCacheKey.FillCacheKey(entity);
            Remove(cacheKey);

            var prefix = NopDiscountDefaults.DiscountCategoryIdsByDiscountPrefixCacheKey.ToCacheKey(entity);
            RemoveByPrefix(prefix);

            prefix = NopDiscountDefaults.DiscountManufacturerIdsByDiscountPrefixCacheKey.ToCacheKey(entity);
            RemoveByPrefix(prefix);
        }
    }
}
