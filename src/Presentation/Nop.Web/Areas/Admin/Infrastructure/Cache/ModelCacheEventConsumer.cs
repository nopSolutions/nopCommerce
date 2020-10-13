using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Vendors;
using Nop.Core.Events;
using Nop.Services.Events;
using Nop.Services.Plugins;

namespace Nop.Web.Areas.Admin.Infrastructure.Cache
{
    /// <summary>
    /// Model cache event consumer (used for caching of presentation layer models)
    /// </summary>
    public partial class ModelCacheEventConsumer :
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
        //vendors
        IConsumer<EntityInsertedEvent<Vendor>>,
        IConsumer<EntityUpdatedEvent<Vendor>>,
        IConsumer<EntityDeletedEvent<Vendor>>,

        IConsumer<PluginUpdatedEvent>
    {
        #region Fields

        private readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        public ModelCacheEventConsumer(IStaticCacheManager staticCacheManager)
        {
            _staticCacheManager = staticCacheManager;
        }

        #endregion

        #region Methods

        public async Task HandleEvent(EntityUpdatedEvent<Setting> eventMessage)
        {
            //clear models which depend on settings
            await _staticCacheManager.Remove(NopModelCacheDefaults.OfficialNewsModelKey); //depends on AdminAreaSettings.HideAdvertisementsOnAdminArea
        }

        //categories
        public async Task HandleEvent(EntityInsertedEvent<Category> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.CategoriesListPrefixCacheKey);
        }
        public async Task HandleEvent(EntityUpdatedEvent<Category> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.CategoriesListPrefixCacheKey);
        }
        public async Task HandleEvent(EntityDeletedEvent<Category> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.CategoriesListPrefixCacheKey);
        }

        //manufacturers
        public async Task HandleEvent(EntityInsertedEvent<Manufacturer> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.ManufacturersListPrefixCacheKey);
        }
        public async Task HandleEvent(EntityUpdatedEvent<Manufacturer> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.ManufacturersListPrefixCacheKey);
        }
        public async Task HandleEvent(EntityDeletedEvent<Manufacturer> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.ManufacturersListPrefixCacheKey);
        }

        //vendors
        public async Task HandleEvent(EntityInsertedEvent<Vendor> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.VendorsListPrefixCacheKey);
        }
        public async Task HandleEvent(EntityUpdatedEvent<Vendor> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.VendorsListPrefixCacheKey);
        }
        public async Task HandleEvent(EntityDeletedEvent<Vendor> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.VendorsListPrefixCacheKey);
        }

        /// <summary>
        /// Handle plugin updated event
        /// </summary>
        /// <param name="eventMessage">Event</param>
        public async Task HandleEvent(PluginUpdatedEvent eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopPluginDefaults.AdminNavigationPluginsPrefix);
        }

        #endregion
    }
}