using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Plugin.Feed.ChatGptShopping.Models;
using Nop.Plugin.Feed.ChatGptShopping.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.ScheduleTasks;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Feed.ChatGptShopping.Controllers;

[Area(AreaNames.ADMIN)]
[AuthorizeAdmin]
[AutoValidateAntiforgeryToken]
public class ChatGptShoppingController : BasePluginController
{
    #region Fields

    private readonly ChatGptShoppingService _chatGptShoppingService;
    private readonly IBaseAdminModelFactory _baseAdminModelFactory;
    private readonly ILocalizationService _localizationService;
    private readonly INopFileProvider _nopFileProvider;
    private readonly INotificationService _notificationService;
    private readonly IScheduleTaskService _scheduleTaskService;
    private readonly ISettingService _settingService;
    private readonly IStoreContext _storeContext;
    private readonly IStoreService _storeService;

    #endregion

    #region Ctor

    public ChatGptShoppingController(ChatGptShoppingService chatGptShoppingService,
        IBaseAdminModelFactory baseAdminModelFactory,
        ILocalizationService localizationService,
        INopFileProvider nopFileProvider,
        INotificationService notificationService,
        IScheduleTaskService scheduleTaskService,
        ISettingService settingService,
        IStoreContext storeContext,
        IStoreService storeService)
    {
        _chatGptShoppingService = chatGptShoppingService;
        _baseAdminModelFactory = baseAdminModelFactory;
        _localizationService = localizationService;
        _nopFileProvider = nopFileProvider;
        _notificationService = notificationService;
        _scheduleTaskService = scheduleTaskService;
        _settingService = settingService;
        _storeContext = storeContext;
        _storeService = storeService;
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> Configure()
    {
        //load settings for a chosen store scope
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var chatGptShoppingSettings = await _settingService.LoadSettingAsync<ChatGptShoppingSettings>(storeScope);

        //prepare model
        var model = new ConfigurationModel
        {
            CurrencyId = chatGptShoppingSettings.CurrencyId,
            LanguageId = chatGptShoppingSettings.LanguageId,
            ProductPictureSize = chatGptShoppingSettings.ProductPictureSize,
            TargetCountries = chatGptShoppingSettings.TargetCountries,
            StoreCountry = chatGptShoppingSettings.StoreCountry
        };

        var scheduleTask = await _scheduleTaskService.GetTaskByTypeAsync(ChatGptShoppingDefaults.SynchronizationTask.Type);
        if (scheduleTask is not null)
        {
            model.AutoSyncEnabled = scheduleTask.Enabled;
            model.AutoSyncPeriod = scheduleTask.Seconds / 60;
        }
        else
        {
            var error = await _localizationService.GetResourceAsync("Plugins.Feed.ChatGptShopping.Configuration.AutoSync.Error");
            _notificationService.ErrorNotification(error);
        }

        //prepare available currencies
        await _baseAdminModelFactory.PrepareCurrenciesAsync(model.AvailableCurrencies,
            defaultItemText: await _localizationService.GetResourceAsync("Admin.Common.EmptyItemText"));

        //prepare available languages
        await _baseAdminModelFactory.PrepareLanguagesAsync(model.AvailableLanguages, false);

        //file paths
        var stores = storeScope > 0
            ? [await _storeService.GetStoreByIdAsync(storeScope)]
            : (await _storeService.GetAllStoresAsync()).ToList();
        foreach (var store in stores)
        {
            var pathToFile = string.Format(ChatGptShoppingDefaults.PathToFeedFile, store.Id);
            var localFilePath = _nopFileProvider.GetAbsolutePath(pathToFile);
            if (_nopFileProvider.FileExists(localFilePath))
            {
                model.GeneratedFiles.Add(new()
                {
                    StoreName = store.Name,
                    FileUrl = $"{store.Url}{pathToFile}"
                });
            }
        }

        model.ActiveStoreScopeConfiguration = storeScope;
        if (storeScope > 0)
        {
            model.CurrencyId_OverrideForStore = await _settingService.SettingExistsAsync(chatGptShoppingSettings, x => x.CurrencyId, storeScope);
            model.LanguageId_OverrideForStore = await _settingService.SettingExistsAsync(chatGptShoppingSettings, x => x.LanguageId, storeScope);
            model.ProductPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(chatGptShoppingSettings, x => x.ProductPictureSize, storeScope);
            model.TargetCountries_OverrideForStore = await _settingService.SettingExistsAsync(chatGptShoppingSettings, x => x.TargetCountries, storeScope);
            model.StoreCountry_OverrideForStore = await _settingService.SettingExistsAsync(chatGptShoppingSettings, x => x.StoreCountry, storeScope);
        }

        return View("~/Plugins/Feed.ChatGptShopping/Views/Configure.cshtml", model);
    }

    [HttpPost]
    [FormValueRequired("save")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!ModelState.IsValid)
            return await Configure();

        //load settings for a chosen store scope
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var chatGptShoppingSettings = await _settingService.LoadSettingAsync<ChatGptShoppingSettings>(storeScope);

        //set new settings values
        chatGptShoppingSettings.CurrencyId = model.CurrencyId;
        chatGptShoppingSettings.LanguageId = model.LanguageId;
        chatGptShoppingSettings.ProductPictureSize = model.ProductPictureSize;
        chatGptShoppingSettings.TargetCountries = model.TargetCountries;
        chatGptShoppingSettings.StoreCountry = model.StoreCountry;

        /* We do not clear cache after each setting update.
         * This behavior can increase performance because cached settings will not be cleared 
         * and loaded from database after each update */
        await _settingService.SaveSettingOverridablePerStoreAsync(chatGptShoppingSettings, x => x.CurrencyId, model.CurrencyId_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(chatGptShoppingSettings, x => x.LanguageId, model.LanguageId_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(chatGptShoppingSettings, x => x.ProductPictureSize, model.ProductPictureSize_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(chatGptShoppingSettings, x => x.TargetCountries, model.TargetCountries_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(chatGptShoppingSettings, x => x.StoreCountry, model.StoreCountry_OverrideForStore, storeScope, false);

        //now clear settings cache
        await _settingService.ClearCacheAsync();

        var scheduleTask = await _scheduleTaskService.GetTaskByTypeAsync(ChatGptShoppingDefaults.SynchronizationTask.Type);
        if (scheduleTask is not null && (scheduleTask.Enabled != model.AutoSyncEnabled || scheduleTask.Seconds != model.AutoSyncPeriod * 60))
        {
            if (!scheduleTask.Enabled && model.AutoSyncEnabled)
                scheduleTask.LastEnabledUtc = DateTime.UtcNow;
            scheduleTask.Enabled = model.AutoSyncEnabled;
            scheduleTask.Seconds = model.AutoSyncPeriod * 60;
            await _scheduleTaskService.UpdateTaskAsync(scheduleTask);

            var warning = await _localizationService.GetResourceAsync("Plugins.Feed.ChatGptShopping.Configuration.AutoSync.Warning");
            _notificationService.WarningNotification(warning);
        }

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return RedirectToAction(nameof(Configure));
    }

    [HttpPost, ActionName("Configure")]
    [FormValueRequired("generate")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> GenerateFeed()
    {
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();

        var messages = await _chatGptShoppingService.GenerateChatGptFeedAsync(storeScope);
        foreach (var message in messages)
            _notificationService.Notification(message.Type, message.Message, false);

        if (!messages.Any(message => message.Type == NotifyType.Error))
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Feed.ChatGptShopping.SuccessResult"));

        return await Configure();
    }

    #endregion
}
