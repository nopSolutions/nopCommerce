using Nop.Core.Domain.Discounts;
using Nop.Services.Caching;

namespace Nop.Services.Discounts.Caching
{
    /// <summary>
    /// Represents a discount requirement cache event consumer
    /// </summary>
    public partial class DiscountRequirementCacheEventConsumer : CacheEventConsumer<DiscountRequirement>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(DiscountRequirement entity)
        {
            await RemoveAsync(NopDiscountDefaults.DiscountRequirementsByDiscountCacheKey, entity.DiscountId);

            if (entity.ParentId.HasValue)
                await RemoveAsync(NopDiscountDefaults.DiscountRequirementsByParentCacheKey, entity.ParentId);
        }

        /// <summary>
        /// Clear cache by entity event type
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(DiscountRequirement entity, EntityEventType entityEventType)
        {
            if (entityEventType == EntityEventType.Delete)
                await RemoveAsync(NopDiscountDefaults.DiscountRequirementsByParentCacheKey, entity);

            await base.ClearCacheAsync(entity, entityEventType);
        }
    }
}
