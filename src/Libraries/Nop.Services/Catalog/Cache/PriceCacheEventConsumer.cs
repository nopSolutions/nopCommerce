using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Services.Events;

namespace Nop.Services.Catalog.Cache
{
    /// <summary>
    /// Price cache event consumer (used for caching of prices)
    /// </summary>
    public partial class PriceCacheEventConsumer :
        //settings
        IConsumer<EntityUpdatedEvent<Setting>>,
        //categories
        IConsumer<EntityInsertedEvent<Category>>,
        IConsumer<EntityUpdatedEvent<Category>>,
        IConsumer<EntityDeletedEvent<Category>>,
        //manufacturers
        IConsumer<EntityInsertedEvent<Manufacturer>>,
        IConsumer<EntityUpdatedEvent<Manufacturer>>,
        IConsumer<EntityDeletedEvent<Manufacturer>>,
        //product categories
        IConsumer<EntityInsertedEvent<ProductCategory>>,
        IConsumer<EntityUpdatedEvent<ProductCategory>>,
        IConsumer<EntityDeletedEvent<ProductCategory>>,
        //product manufacturers
        IConsumer<EntityInsertedEvent<ProductManufacturer>>,
        IConsumer<EntityUpdatedEvent<ProductManufacturer>>,
        IConsumer<EntityDeletedEvent<ProductManufacturer>>,
        //products
        IConsumer<EntityInsertedEvent<Product>>,
        IConsumer<EntityUpdatedEvent<Product>>,
        IConsumer<EntityDeletedEvent<Product>>,
        //tier prices
        IConsumer<EntityInsertedEvent<TierPrice>>,
        IConsumer<EntityUpdatedEvent<TierPrice>>,
        IConsumer<EntityDeletedEvent<TierPrice>>,
        //orders
        IConsumer<EntityInsertedEvent<Order>>,
        IConsumer<EntityUpdatedEvent<Order>>,
        IConsumer<EntityDeletedEvent<Order>>
    {
        #region Fields

        private readonly IStaticCacheManager _cacheManager;

        #endregion

        #region Ctor

        public PriceCacheEventConsumer(IStaticCacheManager cacheManager)
        {
            this._cacheManager = cacheManager;
        }

        #endregion

        #region Methods

        public void HandleEvent(EntityUpdatedEvent<Setting> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductPricePatternCacheKey);
            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductCategoryIdsPatternCacheKey);
            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductManufacturerIdsPatternCacheKey);
        }

        #region Categories

        public void HandleEvent(EntityInsertedEvent<Category> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductCategoryIdsPatternCacheKey);
        }

        public void HandleEvent(EntityUpdatedEvent<Category> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductCategoryIdsPatternCacheKey);
        }

        public void HandleEvent(EntityDeletedEvent<Category> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductCategoryIdsPatternCacheKey);
        }
        
        #endregion

        #region Manufacturers

        public void HandleEvent(EntityInsertedEvent<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductManufacturerIdsPatternCacheKey);
        }

        public void HandleEvent(EntityUpdatedEvent<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductManufacturerIdsPatternCacheKey);
        }

        public void HandleEvent(EntityDeletedEvent<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductManufacturerIdsPatternCacheKey);
        }

        #endregion

        #region Product categories

        public void HandleEvent(EntityInsertedEvent<ProductCategory> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductPricePatternCacheKey);
            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductCategoryIdsPatternCacheKey);
        }

        public void HandleEvent(EntityUpdatedEvent<ProductCategory> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductPricePatternCacheKey);
            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductCategoryIdsPatternCacheKey);
        }

        public void HandleEvent(EntityDeletedEvent<ProductCategory> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductPricePatternCacheKey);
            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductCategoryIdsPatternCacheKey);
        }

        #endregion

        #region Product manufacturers

        public void HandleEvent(EntityInsertedEvent<ProductManufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductPricePatternCacheKey);
            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductManufacturerIdsPatternCacheKey);
        }

        public void HandleEvent(EntityUpdatedEvent<ProductManufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductPricePatternCacheKey);
            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductManufacturerIdsPatternCacheKey);
        }

        public void HandleEvent(EntityDeletedEvent<ProductManufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductPricePatternCacheKey);
            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductManufacturerIdsPatternCacheKey);
        }

        #endregion

        #region Products

        public void HandleEvent(EntityInsertedEvent<Product> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductPricePatternCacheKey);
        }

        public void HandleEvent(EntityUpdatedEvent<Product> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductPricePatternCacheKey);
        }

        public void HandleEvent(EntityDeletedEvent<Product> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductPricePatternCacheKey);
        }

        #endregion

        #region Tier prices

        public void HandleEvent(EntityInsertedEvent<TierPrice> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductPricePatternCacheKey);
        }

        public void HandleEvent(EntityUpdatedEvent<TierPrice> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductPricePatternCacheKey);
        }

        public void HandleEvent(EntityDeletedEvent<TierPrice> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductPricePatternCacheKey);
        }

        #endregion

        #region Orders

        public void HandleEvent(EntityInsertedEvent<Order> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductPricePatternCacheKey);
        }

        public void HandleEvent(EntityUpdatedEvent<Order> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductPricePatternCacheKey);
        }

        public void HandleEvent(EntityDeletedEvent<Order> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductPricePatternCacheKey);
        }

        #endregion

        #endregion
    }
}