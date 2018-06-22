using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Discounts;
using Nop.Core.Events;
using Nop.Services.Events;

namespace Nop.Services.Discounts.Cache
{
    /// <summary>
    /// Cache event consumer (used for caching of discounts)
    /// </summary>
    public partial class DiscountEventConsumer :
        //discounts
        IConsumer<EntityInsertedEvent<Discount>>,
        IConsumer<EntityUpdatedEvent<Discount>>,
        IConsumer<EntityDeletedEvent<Discount>>,
        //discount requirements
        IConsumer<EntityInsertedEvent<DiscountRequirement>>,
        IConsumer<EntityUpdatedEvent<DiscountRequirement>>,
        IConsumer<EntityDeletedEvent<DiscountRequirement>>,
        //settings
        IConsumer<EntityUpdatedEvent<Setting>>,
        //categories
        IConsumer<EntityInsertedEvent<Category>>,
        IConsumer<EntityUpdatedEvent<Category>>,
        IConsumer<EntityDeletedEvent<Category>>,
        //manufacturers
        IConsumer<EntityInsertedEvent<Manufacturer>>,
        IConsumer<EntityUpdatedEvent<Manufacturer>>,
        IConsumer<EntityDeletedEvent<Manufacturer>>
    {
        private readonly IStaticCacheManager _cacheManager;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        public DiscountEventConsumer(IStaticCacheManager cacheManager)
        {
            this._cacheManager = cacheManager;
        }

        //discounts
        public void HandleEvent(EntityInsertedEvent<Discount> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopDiscountDefaults.DiscountAllPatternCacheKey);
            _cacheManager.RemoveByPattern(NopDiscountDefaults.DiscountRequirementPatternCacheKey);
            _cacheManager.RemoveByPattern(NopDiscountDefaults.DiscountCategoryIdsPatternCacheKey);
            _cacheManager.RemoveByPattern(NopDiscountDefaults.DiscountManufacturerIdsPatternCacheKey);
        }
        public void HandleEvent(EntityUpdatedEvent<Discount> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopDiscountDefaults.DiscountAllPatternCacheKey);
            _cacheManager.RemoveByPattern(NopDiscountDefaults.DiscountRequirementPatternCacheKey);
            _cacheManager.RemoveByPattern(NopDiscountDefaults.DiscountCategoryIdsPatternCacheKey);
            _cacheManager.RemoveByPattern(NopDiscountDefaults.DiscountManufacturerIdsPatternCacheKey);
        }
        public void HandleEvent(EntityDeletedEvent<Discount> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopDiscountDefaults.DiscountAllPatternCacheKey);
            _cacheManager.RemoveByPattern(NopDiscountDefaults.DiscountRequirementPatternCacheKey);
            _cacheManager.RemoveByPattern(NopDiscountDefaults.DiscountCategoryIdsPatternCacheKey);
            _cacheManager.RemoveByPattern(NopDiscountDefaults.DiscountManufacturerIdsPatternCacheKey);
        }

        //discount requirements
        public void HandleEvent(EntityInsertedEvent<DiscountRequirement> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopDiscountDefaults.DiscountRequirementPatternCacheKey);
        }
        public void HandleEvent(EntityUpdatedEvent<DiscountRequirement> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopDiscountDefaults.DiscountRequirementPatternCacheKey);
        }
        public void HandleEvent(EntityDeletedEvent<DiscountRequirement> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopDiscountDefaults.DiscountRequirementPatternCacheKey);
        }


        //settings
        public void HandleEvent(EntityUpdatedEvent<Setting> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopDiscountDefaults.DiscountCategoryIdsPatternCacheKey);
            _cacheManager.RemoveByPattern(NopDiscountDefaults.DiscountManufacturerIdsPatternCacheKey);
        }

        //categories
        public void HandleEvent(EntityInsertedEvent<Category> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopDiscountDefaults.DiscountCategoryIdsPatternCacheKey);
        }
        public void HandleEvent(EntityUpdatedEvent<Category> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopDiscountDefaults.DiscountCategoryIdsPatternCacheKey);
        }
        public void HandleEvent(EntityDeletedEvent<Category> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopDiscountDefaults.DiscountCategoryIdsPatternCacheKey);
        }


        //manufacturers
        public void HandleEvent(EntityInsertedEvent<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopDiscountDefaults.DiscountManufacturerIdsPatternCacheKey);
        }
        public void HandleEvent(EntityUpdatedEvent<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopDiscountDefaults.DiscountManufacturerIdsPatternCacheKey);
        }
        public void HandleEvent(EntityDeletedEvent<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopDiscountDefaults.DiscountManufacturerIdsPatternCacheKey);
        }
    }
}
