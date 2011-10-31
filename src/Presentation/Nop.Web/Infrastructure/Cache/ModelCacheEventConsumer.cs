using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Topics;
using Nop.Core.Events;
using Nop.Core.Infrastructure;

namespace Nop.Web.Infrastructure.Cache
{
    /// <summary>
    /// Model cache event consumer (used for caching presentation layer models)
    /// </summary>
    public partial class ModelCacheEventConsumer: 
        //languages
        IConsumer<EntityInserted<Language>>,
        IConsumer<EntityUpdated<Language>>,
        IConsumer<EntityDeleted<Language>>,
        //manufacturers
        IConsumer<EntityInserted<Manufacturer>>,
        IConsumer<EntityUpdated<Manufacturer>>,
        IConsumer<EntityDeleted<Manufacturer>>,
        //categories
        IConsumer<EntityInserted<Category>>,
        IConsumer<EntityUpdated<Category>>,
        IConsumer<EntityDeleted<Category>>,
        //product tags
        IConsumer<EntityInserted<ProductTag>>,
        IConsumer<EntityUpdated<ProductTag>>,
        IConsumer<EntityDeleted<ProductTag>>,
        //specification attributes
        IConsumer<EntityInserted<SpecificationAttribute>>,
        IConsumer<EntityUpdated<SpecificationAttribute>>,
        IConsumer<EntityDeleted<SpecificationAttribute>>,
        //specification attribute options
        IConsumer<EntityInserted<SpecificationAttributeOption>>,
        IConsumer<EntityUpdated<SpecificationAttributeOption>>,
        IConsumer<EntityDeleted<SpecificationAttributeOption>>,
        //Product specification attribute
        IConsumer<EntityInserted<ProductSpecificationAttribute>>,
        IConsumer<EntityUpdated<ProductSpecificationAttribute>>,
        IConsumer<EntityDeleted<ProductSpecificationAttribute>>,
        //Topics
        IConsumer<EntityInserted<Topic>>,
        IConsumer<EntityUpdated<Topic>>,
        IConsumer<EntityDeleted<Topic>>
    {
        /// <summary>
        /// Key for ManufacturerNavigationModel caching
        /// </summary>
        /// <remarks>
        /// {0} : current manufacturer id
        /// {1} : language id
        /// </remarks>
        public const string MANUFACTURER_NAVIGATION_MODEL_KEY = "nop.pres.manufacturer.navigation-{0}-{1}";
        public const string MANUFACTURER_NAVIGATION_PATTERN_KEY = "nop.pres.manufacturer.navigation";
        
        /// <summary>
        /// Key for ProductTagModel caching
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// </remarks>
        public const string PRODUCTTAG_BY_PRODUCT_MODEL_KEY = "nop.pres.producttag.byproduct-{0}";
        public const string PRODUCTTAG_BY_PRODUCT_PATTERN_KEY = "nop.pres.producttag.byproduct";

        /// <summary>
        /// Key for PopularProductTagsModel caching
        /// </summary>
        public const string PRODUCTTAG_POPULAR_MODEL_KEY = "nop.pres.producttag.popular";
        public const string PRODUCTTAG_POPULAR_PATTERN_KEY = "nop.pres.producttag.popular";

        /// <summary>
        /// Key for ProductSpecificationModel caching
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : language id
        /// </remarks>
        public const string PRODUCT_SPECS_MODEL_KEY = "nop.pres.product.specs-{0}-{1}";
        public const string PRODUCT_SPECS_PATTERN_KEY = "nop.pres.product.specs";

        /// <summary>
        /// Key for PopularProductTagsModel caching
        /// </summary>
        /// <remarks>
        /// {0} : topic id
        /// {1} : language id
        /// </remarks>
        public const string TOPIC_MODEL_KEY = "nop.pres.topic.details-{0}-{1}";
        public const string TOPIC_PATTERN_KEY = "nop.pres.topic.";

        private readonly ICacheManager _cacheManager;
        
        public ModelCacheEventConsumer()
        {
            //TODO inject static cache manager using constructor
            this._cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static");
        }
        
        //languages
        public void HandleEvent(EntityInserted<Language> eventMessage)
        {
            //clear all localizable models
            _cacheManager.RemoveByPattern(MANUFACTURER_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(TOPIC_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Language> eventMessage)
        {
            //clear all localizable models
            _cacheManager.RemoveByPattern(MANUFACTURER_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(TOPIC_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Language> eventMessage)
        {
            //clear all localizable models
            _cacheManager.RemoveByPattern(MANUFACTURER_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(TOPIC_PATTERN_KEY);
        }
        
        //manufacturers
        public void HandleEvent(EntityInserted<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(MANUFACTURER_NAVIGATION_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(MANUFACTURER_NAVIGATION_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(MANUFACTURER_NAVIGATION_PATTERN_KEY);
        }
        
        //categories
        public void HandleEvent(EntityInserted<Category> eventMessage)
        {
        }
        public void HandleEvent(EntityUpdated<Category> eventMessage)
        {
        }
        public void HandleEvent(EntityDeleted<Category> eventMessage)
        {
        }
        
        //product tags
        public void HandleEvent(EntityInserted<ProductTag> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCTTAG_POPULAR_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTTAG_BY_PRODUCT_MODEL_KEY);
        }
        public void HandleEvent(EntityUpdated<ProductTag> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCTTAG_POPULAR_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTTAG_BY_PRODUCT_MODEL_KEY);
        }
        public void HandleEvent(EntityDeleted<ProductTag> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCTTAG_POPULAR_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTTAG_BY_PRODUCT_MODEL_KEY);
        }
        
        //specification attributes
        public void HandleEvent(EntityInserted<SpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<SpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<SpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
        }
        
        //specification attribute options
        public void HandleEvent(EntityInserted<SpecificationAttributeOption> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<SpecificationAttributeOption> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<SpecificationAttributeOption> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
        }
        
        //Product specification attribute
        public void HandleEvent(EntityInserted<ProductSpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<ProductSpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<ProductSpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
        }

        //Topics
        public void HandleEvent(EntityInserted<Topic> eventMessage)
        {
            _cacheManager.RemoveByPattern(TOPIC_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Topic> eventMessage)
        {
            _cacheManager.RemoveByPattern(TOPIC_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Topic> eventMessage)
        {
            _cacheManager.RemoveByPattern(TOPIC_PATTERN_KEY);
        }
    }
}
