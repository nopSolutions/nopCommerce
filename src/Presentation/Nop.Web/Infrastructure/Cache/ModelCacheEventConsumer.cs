using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
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
        IConsumer<EntityDeleted<ProductTag>>
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
        /// Key for PopularProductTagsModel caching
        /// </summary>
        /// <remarks>
        /// {0} : current manufacturer id
        /// {1} : language id
        /// </remarks>
        public const string PRODUCTTAG_POPULAR_MODEL_KEY = "nop.pres.producttag.popular";
        public const string PRODUCTTAG_POPULAR_PATTERN_KEY = "nop.pres.producttag.popular";
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
        }
        public void HandleEvent(EntityUpdated<Language> eventMessage)
        {
            //clear all localizable models
            _cacheManager.RemoveByPattern(MANUFACTURER_NAVIGATION_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Language> eventMessage)
        {
            //clear all localizable models
            _cacheManager.RemoveByPattern(MANUFACTURER_NAVIGATION_PATTERN_KEY);
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
        }
        public void HandleEvent(EntityUpdated<ProductTag> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCTTAG_POPULAR_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<ProductTag> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCTTAG_POPULAR_PATTERN_KEY);
        }
    }
}
