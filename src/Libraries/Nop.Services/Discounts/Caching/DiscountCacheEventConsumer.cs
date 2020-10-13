using System.Threading.Tasks;
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
        protected override async Task ClearCache(Discount entity)
        {
            await Remove(NopDiscountDefaults.DiscountRequirementsByDiscountCacheKey, entity);
            await RemoveByPrefix(NopDiscountDefaults.CategoryIdsByDiscountPrefix, entity);
            await RemoveByPrefix(NopDiscountDefaults.ManufacturerIdsByDiscountPrefix, entity);
        }
    }
}
