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
            await RemoveByPrefix(NopDiscountDefaults.DiscountAllPrefixCacheKey);
            var cacheKey = _cacheKeyService.PrepareKey(NopDiscountDefaults.DiscountRequirementModelCacheKey, entity);
            await Remove(cacheKey);

            var prefix = _cacheKeyService.PrepareKeyPrefix(NopDiscountDefaults.DiscountCategoryIdsByDiscountPrefixCacheKey, entity);
            await RemoveByPrefix(prefix);

            prefix = _cacheKeyService.PrepareKeyPrefix(NopDiscountDefaults.DiscountManufacturerIdsByDiscountPrefixCacheKey, entity);
            await RemoveByPrefix(prefix);
        }
    }
}
