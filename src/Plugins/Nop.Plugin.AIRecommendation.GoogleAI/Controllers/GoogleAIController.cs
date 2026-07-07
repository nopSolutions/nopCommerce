using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.AIRecommendation.GoogleAI.Models;
using Nop.Plugin.AIRecommendation.GoogleAI.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.AIRecommendation.GoogleAI.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
public class GoogleAiController : BasePluginController
{
    #region Fields

    private readonly GoogleAiSettings _googleAiSettings;
    private readonly GoogleAiService _googleAiService;
    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    private readonly ISettingService _settingService;

    #endregion

    #region Ctor

    public GoogleAiController(GoogleAiSettings googleAiSettings,
        GoogleAiService googleAiService,
        ILocalizationService localizationService,
        INotificationService notificationService,
        ISettingService settingService)
    {
        _googleAiSettings = googleAiSettings;
        _googleAiService = googleAiService;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _settingService = settingService;
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public IActionResult Configure()
    {
        var model = new ConfigurationModel
        {
            Enabled = _googleAiSettings.Enabled,
            ProjectId = _googleAiSettings.ProjectId,
            LocationId = _googleAiSettings.LocationId,
            CatalogId = _googleAiSettings.CatalogId,
            BranchId = _googleAiSettings.BranchId,
            SyncAllowed = _googleAiSettings.SyncAllowed,
            LogRequests = _googleAiSettings.LogRequests,
            SearchAllowed = _googleAiSettings.SearchAllowed
        };

        return View("~/Plugins/AIRecommendation.GoogleAI/Views/Configure.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        _googleAiSettings.Enabled = model.Enabled;
        _googleAiSettings.ProjectId = model.ProjectId;
        _googleAiSettings.LocationId = model.LocationId;
        _googleAiSettings.CatalogId = model.CatalogId;
        _googleAiSettings.BranchId = model.BranchId;
        _googleAiSettings.SyncAllowed = model.SyncAllowed;
        _googleAiSettings.LogRequests = model.LogRequests;
        _googleAiSettings.SearchAllowed = model.SearchAllowed;

        await _settingService.SaveSettingAsync(_googleAiSettings);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return Configure();
    }

    [HttpPost, ActionName("Configure")]
    [FormValueRequired("sync-catalog")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public async Task<IActionResult> SyncCatalog()
    {
        try
        {
            var (successCount, failureCount) = await _googleAiService.SyncProductsAsync();
            _notificationService.SuccessNotification(string.Format(await _localizationService.GetResourceAsync("Plugin.AIRecommendation.GoogleAI.CatalogImportedSuccessfully"), successCount, failureCount));
        }
        catch (Exception ex)
        {
            await _notificationService.ErrorNotificationAsync(ex);
        }

        return Configure();
    }

    #endregion
}
