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
        #region Fields

        private readonly IStaticCacheManager _cacheManager;

        #endregion

        #region Ctor

        public DiscountEventConsumer(IStaticCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        #endregion

        #region Methods

        #region Discounts

        public void HandleEvent(EntityInsertedEvent<Discount> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopDiscountDefaults.DiscountAllPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopDiscountDefaults.DiscountRequirementPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopDiscountDefaults.DiscountCategoryIdsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopDiscountDefaults.DiscountManufacturerIdsPrefixCacheKey);
        }

        public void HandleEvent(EntityUpdatedEvent<Discount> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopDiscountDefaults.DiscountAllPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopDiscountDefaults.DiscountRequirementPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopDiscountDefaults.DiscountCategoryIdsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopDiscountDefaults.DiscountManufacturerIdsPrefixCacheKey);
        }

        public void HandleEvent(EntityDeletedEvent<Discount> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopDiscountDefaults.DiscountAllPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopDiscountDefaults.DiscountRequirementPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopDiscountDefaults.DiscountCategoryIdsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopDiscountDefaults.DiscountManufacturerIdsPrefixCacheKey);
        }

        #endregion

        #region Discount requirements

        public void HandleEvent(EntityInsertedEvent<DiscountRequirement> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopDiscountDefaults.DiscountRequirementPrefixCacheKey);
        }

        public void HandleEvent(EntityUpdatedEvent<DiscountRequirement> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopDiscountDefaults.DiscountRequirementPrefixCacheKey);
        }

        public void HandleEvent(EntityDeletedEvent<DiscountRequirement> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopDiscountDefaults.DiscountRequirementPrefixCacheKey);
        }

        #endregion

        #region Settings

        public void HandleEvent(EntityUpdatedEvent<Setting> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopDiscountDefaults.DiscountCategoryIdsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopDiscountDefaults.DiscountManufacturerIdsPrefixCacheKey);
        }

        #endregion

        #region Categories

        public void HandleEvent(EntityInsertedEvent<Category> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopDiscountDefaults.DiscountCategoryIdsPrefixCacheKey);
        }

        public void HandleEvent(EntityUpdatedEvent<Category> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopDiscountDefaults.DiscountCategoryIdsPrefixCacheKey);
        }

        public void HandleEvent(EntityDeletedEvent<Category> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopDiscountDefaults.DiscountCategoryIdsPrefixCacheKey);
        }

        #endregion

        #region Manufacturers

        public void HandleEvent(EntityInsertedEvent<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopDiscountDefaults.DiscountManufacturerIdsPrefixCacheKey);
        }

        public void HandleEvent(EntityUpdatedEvent<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopDiscountDefaults.DiscountManufacturerIdsPrefixCacheKey);
        }

        public void HandleEvent(EntityDeletedEvent<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopDiscountDefaults.DiscountManufacturerIdsPrefixCacheKey);
        }

        #endregion

        #endregion
    }
}