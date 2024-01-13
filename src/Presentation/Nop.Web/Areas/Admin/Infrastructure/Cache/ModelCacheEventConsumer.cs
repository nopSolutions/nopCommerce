using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Vendors;
using Nop.Core.Events;
using Nop.Services.Events;
using Nop.Services.Plugins;

namespace Nop.Web.Areas.Admin.Infrastructure.Cache;

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

    protected readonly IStaticCacheManager _staticCacheManager;

    #endregion

    #region Ctor

    public ModelCacheEventConsumer(IStaticCacheManager staticCacheManager)
    {
        _staticCacheManager = staticCacheManager;
    }

    #endregion

    #region Methods

    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityUpdatedEvent<Setting> eventMessage)
    {
        //clear models which depend on settings
        await _staticCacheManager.RemoveAsync(NopModelCacheDefaults.OfficialNewsModelKey); //depends on AdminAreaSettings.HideAdvertisementsOnAdminArea
    }

    //categories
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityInsertedEvent<Category> eventMessage)
    {
        await _staticCacheManager.RemoveAsync(NopModelCacheDefaults.CategoriesListKey);
    }
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityUpdatedEvent<Category> eventMessage)
    {
        await _staticCacheManager.RemoveAsync(NopModelCacheDefaults.CategoriesListKey);
    }
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityDeletedEvent<Category> eventMessage)
    {
        await _staticCacheManager.RemoveAsync(NopModelCacheDefaults.CategoriesListKey);
    }

    //manufacturers
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityInsertedEvent<Manufacturer> eventMessage)
    {
        await _staticCacheManager.RemoveAsync(NopModelCacheDefaults.ManufacturersListKey);
    }
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityUpdatedEvent<Manufacturer> eventMessage)
    {
        await _staticCacheManager.RemoveAsync(NopModelCacheDefaults.ManufacturersListKey);
    }
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityDeletedEvent<Manufacturer> eventMessage)
    {
        await _staticCacheManager.RemoveAsync(NopModelCacheDefaults.ManufacturersListKey);
    }

    //vendors
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityInsertedEvent<Vendor> eventMessage)
    {
        await _staticCacheManager.RemoveAsync(NopModelCacheDefaults.VendorsListKey);
    }
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityUpdatedEvent<Vendor> eventMessage)
    {
        await _staticCacheManager.RemoveAsync(NopModelCacheDefaults.VendorsListKey);
    }
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityDeletedEvent<Vendor> eventMessage)
    {
        await _staticCacheManager.RemoveAsync(NopModelCacheDefaults.VendorsListKey);
    }

    /// <summary>
    /// Handle plugin updated event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(PluginUpdatedEvent eventMessage)
    {
        await _staticCacheManager.RemoveByPrefixAsync(NopPluginDefaults.AdminNavigationPluginsPrefix);
    }

    #endregion
}