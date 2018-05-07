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
    public partial class PriceCacheEventConsumer: 
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
        /// <summary>
        /// Key for product prices
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : overridden product price
        /// {2} : additional charge
        /// {3} : include discounts (true, false)
        /// {4} : quantity
        /// {5} : roles of the current user
        /// {6} : current store ID
        /// </remarks>
        public const string PRODUCT_PRICE_MODEL_KEY = "Nop.totals.productprice-{0}-{1}-{2}-{3}-{4}-{5}-{6}";
        public const string PRODUCT_PRICE_PATTERN_KEY = "Nop.totals.productprice";

        /// <summary>
        /// Key for category IDs of a product
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public const string PRODUCT_CATEGORY_IDS_MODEL_KEY = "Nop.totals.product.categoryids-{0}-{1}-{2}";
        public const string PRODUCT_CATEGORY_IDS_PATTERN_KEY = "Nop.totals.product.categoryids";

        /// <summary>
        /// Key for manufacturer IDs of a product
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public const string PRODUCT_MANUFACTURER_IDS_MODEL_KEY = "Nop.totals.product.manufacturerids-{0}-{1}-{2}";
        public const string PRODUCT_MANUFACTURER_IDS_PATTERN_KEY = "Nop.totals.product.manufacturerids";

        private readonly IStaticCacheManager _cacheManager;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        public PriceCacheEventConsumer(IStaticCacheManager cacheManager)
        {
            this._cacheManager = cacheManager;
        }

        //settings
        public void HandleEvent(EntityUpdatedEvent<Setting> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_PRICE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_CATEGORY_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_MANUFACTURER_IDS_PATTERN_KEY);
        }

        //categories
        public void HandleEvent(EntityInsertedEvent<Category> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_CATEGORY_IDS_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<Category> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_CATEGORY_IDS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<Category> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_CATEGORY_IDS_PATTERN_KEY);
        }

        //manufacturers
        public void HandleEvent(EntityInsertedEvent<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_MANUFACTURER_IDS_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_MANUFACTURER_IDS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_MANUFACTURER_IDS_PATTERN_KEY);
        }

        //product categories
        public void HandleEvent(EntityInsertedEvent<ProductCategory> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_PRICE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_CATEGORY_IDS_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<ProductCategory> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_PRICE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_CATEGORY_IDS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<ProductCategory> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_PRICE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_CATEGORY_IDS_PATTERN_KEY);
        }

        //product manufacturers
        public void HandleEvent(EntityInsertedEvent<ProductManufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_PRICE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_MANUFACTURER_IDS_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<ProductManufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_PRICE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_MANUFACTURER_IDS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<ProductManufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_PRICE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_MANUFACTURER_IDS_PATTERN_KEY);
        }

        //products
        public void HandleEvent(EntityInsertedEvent<Product> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_PRICE_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<Product> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_PRICE_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<Product> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_PRICE_PATTERN_KEY);
        }

        //tier prices
        public void HandleEvent(EntityInsertedEvent<TierPrice> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_PRICE_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<TierPrice> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_PRICE_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<TierPrice> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_PRICE_PATTERN_KEY);
        }

        //orders
        public void HandleEvent(EntityInsertedEvent<Order> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_PRICE_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<Order> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_PRICE_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<Order> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_PRICE_PATTERN_KEY);
        }
    }
}
