using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Services.Configuration;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Plugin.Api.Rest.Models;

namespace Nop.Plugin.Api.Rest.Controllers;

/// <summary>
/// Represents the plugin configuration controller
/// </summary>
[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
public class ApiRestController : BasePluginController
{
    #region Fields

    private readonly IEncryptionService _encryptionService;
    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    private readonly ISettingService _settingService;
    private readonly IStoreContext _storeContext;
    private readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public ApiRestController(IEncryptionService encryptionService,
        ILocalizationService localizationService,
        INotificationService notificationService,
        ISettingService settingService,
        IStoreContext storeContext,
        IWebHelper webHelper)
    {
        _encryptionService = encryptionService;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _settingService = settingService;
        _storeContext = storeContext;
        _webHelper = webHelper;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Gets the absolute path of the plugin configuration view
    /// </summary>
    protected string ConfigureViewPath =>
        $"~/{NopPluginDefaults.PathName}/{ApiRestDefaults.OutputFolderName}/Views/{nameof(Configure)}.cshtml";

    #endregion

    #region Methods

    /// <summary>
    /// Configure the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public virtual async Task<IActionResult> Configure()
    {
        //load the settings for the store chosen in the store scope dropdown, not the injected instance,
        //because the injected one follows the host of the admin panel instead of the selected store
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var settings = await _settingService.LoadSettingAsync<ApiRestSettings>(storeScope);
        var storeLocation = _webHelper.GetStoreLocation();

        var model = new ConfigurationModel
        {
            SwaggerUiUrl = $"{storeLocation}{ApiRestDefaults.SwaggerUiPath}",
            SwaggerJsonUrl = $"{storeLocation}{ApiRestDefaults.SwaggerJsonPath}",
            ActiveStoreScopeConfiguration = storeScope,
            ApiKey = settings.ApiKey,
            RateLimitPerMinute = settings.RateLimitPerMinute,
            RequireApiKeyForReads = settings.RequireApiKeyForReads
        };

        //the override checkboxes only mean something once a specific store is selected
        if (storeScope > 0)
        {
            model.ApiKey_OverrideForStore = await _settingService.SettingExistsAsync(settings, x => x.ApiKey, storeScope);
            model.RateLimitPerMinute_OverrideForStore =
                await _settingService.SettingExistsAsync(settings, x => x.RateLimitPerMinute, storeScope);
            model.RequireApiKeyForReads_OverrideForStore =
                await _settingService.SettingExistsAsync(settings, x => x.RequireApiKeyForReads, storeScope);
        }

        return View(ConfigureViewPath, model);
    }

    /// <summary>
    /// Save the plugin settings
    /// </summary>
    /// <param name="model">Settings to save</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    [HttpPost, ActionName(nameof(Configure))]
    [FormValueRequired("save")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public virtual async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!ModelState.IsValid)
            return await Configure();

        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var settings = await _settingService.LoadSettingAsync<ApiRestSettings>(storeScope);

        //a blank key field means "leave the current key alone", so an admin editing the rate limit does
        //not silently wipe the credential
        if (!string.IsNullOrWhiteSpace(model.ApiKey))
            settings.ApiKey = model.ApiKey.Trim();

        settings.RateLimitPerMinute = model.RateLimitPerMinute;
        settings.RequireApiKeyForReads = model.RequireApiKeyForReads;

        //save each field per property so the "override for store" checkboxes work, and so the shared row
        //stays intact for the stores that do not override
        await _settingService.SaveSettingOverridablePerStoreAsync(settings, x => x.ApiKey, model.ApiKey_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(settings, x => x.RateLimitPerMinute, model.RateLimitPerMinute_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(settings, x => x.RequireApiKeyForReads, model.RequireApiKeyForReads_OverrideForStore, storeScope, false);

        await _settingService.ClearCacheAsync();

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await Configure();
    }

    /// <summary>
    /// Generate a new API key for the selected store and save it straight away
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    [HttpPost]
    [FormValueRequired("generate-api-key")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public virtual async Task<IActionResult> GenerateApiKey()
    {
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var settings = await _settingService.LoadSettingAsync<ApiRestSettings>(storeScope);

        settings.ApiKey = _encryptionService.CreateSaltKey(32);

        //a generated key always belongs to the store that asked for it, otherwise a tenant would keep
        //inheriting the shared key and generate requests would appear to have no effect
        await _settingService.SaveSettingAsync(settings, x => x.ApiKey, storeScope);
        await _settingService.ClearCacheAsync();

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugin.Api.Rest.ApiKeyGenerated"));

        return await Configure();
    }

    #endregion
}