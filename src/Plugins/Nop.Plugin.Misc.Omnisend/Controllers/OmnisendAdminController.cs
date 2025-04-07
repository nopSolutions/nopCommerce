using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.Omnisend.DTO;
using Nop.Plugin.Misc.Omnisend.Models;
using Nop.Plugin.Misc.Omnisend.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.Omnisend.Controllers;

[Area(AreaNames.ADMIN)]
[AuthorizeAdmin]
[AutoValidateAntiforgeryToken]
public class OmnisendAdminController(ILocalizationService localizationService,
        INotificationService notificationService,
        ISettingService settingService,
        OmnisendService omnisendService,
        OmnisendSettings omnisendSettings) : BasePluginController
{
    #region Utilities

    private async Task FillBatchesAsync(ConfigurationModel model)
    {
        if (!omnisendSettings.BatchesIds.Any())
            return;

        bool needBlock(BatchResponse response, string endpoint)
        {
            return response.Endpoint.Equals(endpoint, StringComparison.InvariantCultureIgnoreCase) &&
                !response.Status.Equals(OmnisendDefaults.BatchFinishedStatus,
                    StringComparison.InvariantCultureIgnoreCase);
        }

        var batches = await omnisendService.GetStoredBatchesAsync();
        model.Batches = batches;

        model.BlockSyncContacts = model.Batches.Any(p => needBlock(p, OmnisendDefaults.ContactsEndpoint));
        model.BlockSyncOrders = model.Batches.Any(p => needBlock(p, OmnisendDefaults.OrdersEndpoint));
        model.BlockSyncProducts = model.Batches.Any(p => needBlock(p, OmnisendDefaults.ProductsEndpoint)) ||
            batches.Any(p => needBlock(p, OmnisendDefaults.CategoriesEndpoint));
    }

    #endregion

    #region Methods

    public async Task<IActionResult> Configure()
    {
        var model = new ConfigurationModel
        {
            ApiKey = omnisendSettings.ApiKey,
            UseTracking = omnisendSettings.UseTracking
        };

        await FillBatchesAsync(model);

        return View("~/Plugins/Misc.Omnisend/Views/Configure.cshtml", model);
    }

    [HttpPost, ActionName("Configure")]
    [FormValueRequired("save")]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!ModelState.IsValid)
            return await Configure();

        if (omnisendSettings.ApiKey != model.ApiKey || string.IsNullOrEmpty(omnisendSettings.BrandId))
        {
            var brandId = await omnisendService.GetBrandIdAsync(model.ApiKey);

            if (brandId != null)
            {
                omnisendSettings.ApiKey = model.ApiKey;
                omnisendSettings.BrandId = brandId;

                await omnisendService.SendCustomerDataAsync();
            }
        }

        //_omnisendSettings.UseTracking = model.UseTracking;
        //recommended not to change this setting
        omnisendSettings.UseTracking = true;

        await settingService.SaveSettingAsync(omnisendSettings);

        if (string.IsNullOrEmpty(omnisendSettings.BrandId))
            notificationService.ErrorNotification(await localizationService.GetResourceAsync("Plugins.Misc.Omnisend.CantGetBrandId"));
        else
            notificationService.SuccessNotification(await localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await Configure();
    }

    [HttpPost, ActionName("Configure")]
    [FormValueRequired("sync-contacts")]
    public async Task<IActionResult> SyncContacts()
    {
        if (!ModelState.IsValid || string.IsNullOrEmpty(omnisendSettings.BrandId))
            return await Configure();

        await omnisendService.SyncContactsAsync();

        return await Configure();
    }

    [HttpPost, ActionName("Configure")]
    [FormValueRequired("sync-products")]
    public async Task<IActionResult> SyncProducts()
    {
        if (!ModelState.IsValid || string.IsNullOrEmpty(omnisendSettings.BrandId))
            return await Configure();

        await omnisendService.SyncCategoriesAsync();
        await omnisendService.SyncProductsAsync();

        return await Configure();
    }

    [HttpPost, ActionName("Configure")]
    [FormValueRequired("sync-orders")]
    public async Task<IActionResult> SyncOrders()
    {
        if (!ModelState.IsValid || string.IsNullOrEmpty(omnisendSettings.BrandId))
            return await Configure();

        await omnisendService.SyncOrdersAsync();
        await omnisendService.SyncCartsAsync();

        return await Configure();
    }

    #endregion
}