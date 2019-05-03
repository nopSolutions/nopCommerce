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
            _cacheManager = cacheManager;
        }

        #endregion

        #region Methods

        public void HandleEvent(EntityUpdatedEvent<Setting> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductPricePrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductCategoryIdsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductManufacturerIdsPrefixCacheKey);
        }

        #region Categories

        public void HandleEvent(EntityInsertedEvent<Category> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductCategoryIdsPrefixCacheKey);
        }

        public void HandleEvent(EntityUpdatedEvent<Category> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductCategoryIdsPrefixCacheKey);
        }

        public void HandleEvent(EntityDeletedEvent<Category> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductCategoryIdsPrefixCacheKey);
        }
        
        #endregion

        #region Manufacturers

        public void HandleEvent(EntityInsertedEvent<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductManufacturerIdsPrefixCacheKey);
        }

        public void HandleEvent(EntityUpdatedEvent<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductManufacturerIdsPrefixCacheKey);
        }

        public void HandleEvent(EntityDeletedEvent<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductManufacturerIdsPrefixCacheKey);
        }

        #endregion

        #region Product categories

        public void HandleEvent(EntityInsertedEvent<ProductCategory> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductPricePrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductCategoryIdsPrefixCacheKey);
        }

        public void HandleEvent(EntityUpdatedEvent<ProductCategory> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductPricePrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductCategoryIdsPrefixCacheKey);
        }

        public void HandleEvent(EntityDeletedEvent<ProductCategory> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductPricePrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductCategoryIdsPrefixCacheKey);
        }

        #endregion

        #region Product manufacturers

        public void HandleEvent(EntityInsertedEvent<ProductManufacturer> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductPricePrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductManufacturerIdsPrefixCacheKey);
        }

        public void HandleEvent(EntityUpdatedEvent<ProductManufacturer> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductPricePrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductManufacturerIdsPrefixCacheKey);
        }

        public void HandleEvent(EntityDeletedEvent<ProductManufacturer> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductPricePrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductManufacturerIdsPrefixCacheKey);
        }

        #endregion

        #region Products

        public void HandleEvent(EntityInsertedEvent<Product> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductPricePrefixCacheKey);
        }

        public void HandleEvent(EntityUpdatedEvent<Product> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductPricePrefixCacheKey);
        }

        public void HandleEvent(EntityDeletedEvent<Product> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductPricePrefixCacheKey);
        }

        #endregion

        #region Tier prices

        public void HandleEvent(EntityInsertedEvent<TierPrice> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductPricePrefixCacheKey);
        }

        public void HandleEvent(EntityUpdatedEvent<TierPrice> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductPricePrefixCacheKey);
        }

        public void HandleEvent(EntityDeletedEvent<TierPrice> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductPricePrefixCacheKey);
        }

        #endregion

        #region Orders

        public void HandleEvent(EntityInsertedEvent<Order> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductPricePrefixCacheKey);
        }

        public void HandleEvent(EntityUpdatedEvent<Order> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductPricePrefixCacheKey);
        }

        public void HandleEvent(EntityDeletedEvent<Order> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductPricePrefixCacheKey);
        }

        #endregion

        #endregion
    }
}