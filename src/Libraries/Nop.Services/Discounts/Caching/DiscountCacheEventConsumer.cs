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
            Remove(NopDiscountDefaults.DiscountRequirementsByDiscountCacheKey, entity);
            RemoveByPrefix(NopDiscountDefaults.CategoryIdsByDiscountPrefix, entity);
            RemoveByPrefix(NopDiscountDefaults.ManufacturerIdsByDiscountPrefix, entity);
        }
    }
}
