using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Vendors;
using Nop.Core.Events;
using Nop.Services.Events;

namespace Nop.Web.Areas.Admin.Infrastructure.Cache
{
    /// <summary>
    /// Model cache event consumer (used for caching of presentation layer models)
    /// </summary>
    public partial class ModelCacheEventConsumer: 
        //settings
        IConsumer<EntityUpdated<Setting>>,
        //specification attributes
        IConsumer<EntityInserted<SpecificationAttribute>>,
        IConsumer<EntityUpdated<SpecificationAttribute>>,
        IConsumer<EntityDeleted<SpecificationAttribute>>,
        //categories
        IConsumer<EntityInserted<Category>>,
        IConsumer<EntityUpdated<Category>>,
        IConsumer<EntityDeleted<Category>>,
        //manufacturers
        IConsumer<EntityInserted<Manufacturer>>,
        IConsumer<EntityUpdated<Manufacturer>>,
        IConsumer<EntityDeleted<Manufacturer>>,
        //vendors
        IConsumer<EntityInserted<Vendor>>,
        IConsumer<EntityUpdated<Vendor>>,
        IConsumer<EntityDeleted<Vendor>>
    {
        /// <summary>
        /// Key for nopCommerce.com news cache
        /// </summary>
        public const string OFFICIAL_NEWS_MODEL_KEY = "Nop.pres.admin.official.news";
        public const string OFFICIAL_NEWS_PATTERN_KEY = "Nop.pres.admin.official.news";
        
        /// <summary>
        /// Key for specification attributes caching (product details page)
        /// </summary>
        public const string SPEC_ATTRIBUTES_MODEL_KEY = "Nop.pres.admin.product.specs";
        public const string SPEC_ATTRIBUTES_PATTERN_KEY = "Nop.pres.admin.product.specs";

        /// <summary>
        /// Key for categories caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        public const string CATEGORIES_LIST_KEY = "Nop.pres.admin.categories.list-{0}";
        public const string CATEGORIES_LIST_PATTERN_KEY = "Nop.pres.admin.categories.list";

        /// <summary>
        /// Key for manufacturers caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        public const string MANUFACTURERS_LIST_KEY = "Nop.pres.admin.manufacturers.list-{0}";
        public const string MANUFACTURERS_LIST_PATTERN_KEY = "Nop.pres.admin.manufacturers.list";

        /// <summary>
        /// Key for vendors caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        public const string VENDORS_LIST_KEY = "Nop.pres.admin.vendors.list-{0}";
        public const string VENDORS_LIST_PATTERN_KEY = "Nop.pres.admin.vendors.list";

        private readonly ICacheManager _cacheManager;
        
        public ModelCacheEventConsumer(IStaticCacheManager cacheManager)
        {
            this._cacheManager = cacheManager;
        }

        public void HandleEvent(EntityUpdated<Setting> eventMessage)
        {
            //clear models which depend on settings
            _cacheManager.RemoveByPattern(OFFICIAL_NEWS_PATTERN_KEY); //depends on AdminAreaSettings.HideAdvertisementsOnAdminArea
        }
        
        //specification attributes
        public void HandleEvent(EntityInserted<SpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(SPEC_ATTRIBUTES_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<SpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(SPEC_ATTRIBUTES_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<SpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(SPEC_ATTRIBUTES_PATTERN_KEY);
        }

        //categories
        public void HandleEvent(EntityInserted<Category> eventMessage)
        {
            _cacheManager.RemoveByPattern(CATEGORIES_LIST_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Category> eventMessage)
        {
            _cacheManager.RemoveByPattern(CATEGORIES_LIST_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Category> eventMessage)
        {
            _cacheManager.RemoveByPattern(CATEGORIES_LIST_PATTERN_KEY);
        }

        //manufacturers
        public void HandleEvent(EntityInserted<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(MANUFACTURERS_LIST_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(MANUFACTURERS_LIST_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(MANUFACTURERS_LIST_PATTERN_KEY);
        }

        //vendors
        public void HandleEvent(EntityInserted<Vendor> eventMessage)
        {
            _cacheManager.RemoveByPattern(VENDORS_LIST_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Vendor> eventMessage)
        {
            _cacheManager.RemoveByPattern(VENDORS_LIST_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Vendor> eventMessage)
        {
            _cacheManager.RemoveByPattern(VENDORS_LIST_PATTERN_KEY);
        }
    }
}