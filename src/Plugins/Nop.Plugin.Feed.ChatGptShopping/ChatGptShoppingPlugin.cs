using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Feed.ChatGptShopping;

/// <summary>
/// Represents ChatGPT Shopping plugin
/// </summary>
public class ChatGptShoppingPlugin : BasePlugin, IMiscPlugin
{
    #region Fields

    private readonly CurrencySettings _currencySettings;
    private readonly ICustomerService _customerService;
    private readonly ILocalizationService _localizationService;
    private readonly INopUrlHelper _nopUrlHelper;
    private readonly IScheduleTaskService _scheduleTaskService;
    private readonly ISettingService _settingService;
    private readonly LocalizationSettings _localizationSettings;

    #endregion

    #region Ctor

    public ChatGptShoppingPlugin(CurrencySettings currencySettings,
        ICustomerService customerService,
        ILocalizationService localizationService,
        INopUrlHelper nopUrlHelper,
        IScheduleTaskService scheduleTaskService,
        ISettingService settingService,
        LocalizationSettings localizationSettings)
    {
        _currencySettings = currencySettings;
        _customerService = customerService;
        _localizationService = localizationService;
        _nopUrlHelper = nopUrlHelper;
        _scheduleTaskService = scheduleTaskService;
        _settingService = settingService;
        _localizationSettings = localizationSettings;
    }

    #endregion    

    #region Methods

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
        return _nopUrlHelper.RouteUrl(ChatGptShoppingDefaults.ConfigurationRouteName);
    }

    /// <summary>
    /// Install the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
        //ensure MediaSettings.UseAbsoluteImagePath is enabled (used for images uploading)
        await _settingService.SetSettingAsync($"{nameof(MediaSettings)}.{nameof(MediaSettings.UseAbsoluteImagePath)}", true, clearCache: false);

        await _settingService.SaveSettingAsync(new ChatGptShoppingSettings
        {
            CurrencyId = _currencySettings.PrimaryStoreCurrencyId,
            LanguageId = _localizationSettings.DefaultAdminLanguageId,
            StoreCountry = "US",
            TargetCountries = "US",
            ProductPictureSize = 125,
            CustomerId = (await _customerService.GetOrCreateSearchEngineUserAsync()).Id
        });

        if (await _scheduleTaskService.GetTaskByTypeAsync(ChatGptShoppingDefaults.SynchronizationTask.Type) is null)
        {
            await _scheduleTaskService.InsertTaskAsync(new()
            {
                Enabled = false,
                StopOnError = false,
                LastEnabledUtc = DateTime.UtcNow,
                Name = ChatGptShoppingDefaults.SynchronizationTask.Name,
                Type = ChatGptShoppingDefaults.SynchronizationTask.Type,
                Seconds = ChatGptShoppingDefaults.SynchronizationTask.Period
            });
        }

        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.Feed.ChatGptShopping.Configuration"] = "Configuration",
            ["Plugins.Feed.ChatGptShopping.Configuration.Currency"] = "Currency",
            ["Plugins.Feed.ChatGptShopping.Configuration.Currency.Hint"] = "Select the currency that will be used to generate the feed.",
            ["Plugins.Feed.ChatGptShopping.Configuration.Language"] = "Language",
            ["Plugins.Feed.ChatGptShopping.Configuration.Language.Hint"] = "Select the language that will be used to generate the feed.",
            ["Plugins.Feed.ChatGptShopping.Configuration.TargetCountries"] = "Target countries",
            ["Plugins.Feed.ChatGptShopping.Configuration.TargetCountries.Hint"] = "Specify the target countries for which the feed file(s) will be generated. Use ISO 3166-1 alpha-2 country codes, separated by commas (e.g., US,CA,GB). Omitted or empty does not mean worldwide.",
            ["Plugins.Feed.ChatGptShopping.Configuration.StoreCountry"] = "Store country",
            ["Plugins.Feed.ChatGptShopping.Configuration.StoreCountry.Hint"] = "Specify the store country for which the feed file(s) will be generated. Use an ISO 3166-1 alpha-2 country code (e.g., US). Omitted or empty does not select another market.",
            ["Plugins.Feed.ChatGptShopping.Configuration.StaticFilePath"] = "Generated file path (static)",
            ["Plugins.Feed.ChatGptShopping.Configuration.StaticFilePath.Hint"] = "A file path of the generated file. It's static for your store and can be shared with the ChatGptShopping service.",
            ["Plugins.Feed.ChatGptShopping.Configuration.ProductPictureSize"] = "Product thumbnail image size",
            ["Plugins.Feed.ChatGptShopping.Configuration.ProductPictureSize.Hint"] = "The default size (pixels) for product thumbnail images.",
            ["Plugins.Feed.ChatGptShopping.Configuration.AutoSync.Error"] = "Auto synchronization settings cannot be modified. Please reinstall the plugin",
            ["Plugins.Feed.ChatGptShopping.Configuration.AutoSync.Warning"] = "Do not forget to restart the application once auto synchronization settings have been modified",
            ["Plugins.Feed.ChatGptShopping.Configuration.AutoSyncEnabled"] = "Enable auto synchronization",
            ["Plugins.Feed.ChatGptShopping.Configuration.AutoSyncEnabled.Hint"] = "Enable or disable automatic synchronization of the feed. If enabled, the feed will be automatically synchronized at the specified interval.",
            ["Plugins.Feed.ChatGptShopping.Configuration.AutoSyncPeriod"] = "Auto synchronization period",
            ["Plugins.Feed.ChatGptShopping.Configuration.AutoSyncPeriod.Hint"] = "Set the period (in minutes) for auto synchronization.",
            ["Plugins.Feed.ChatGptShopping.Configuration.AutoSyncPeriod.Invalid"] = "Period is invalid",
            ["Plugins.Feed.ChatGptShopping.Generate"] = "Generate feed",
            ["Plugins.Feed.ChatGptShopping.SuccessResult"] = "ChatGPT Shopping feed has been successfully generated.",
        });

        await base.InstallAsync();
    }

    /// <summary>
    /// Uninstall the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UninstallAsync()
    {
        await _settingService.DeleteSettingAsync<ChatGptShoppingSettings>();

        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Feed.ChatGptShopping");

        var scheduleTask = await _scheduleTaskService.GetTaskByTypeAsync(ChatGptShoppingDefaults.SynchronizationTask.Type);
        if (scheduleTask is not null)
            await _scheduleTaskService.DeleteTaskAsync(scheduleTask);

        await base.UninstallAsync();
    }

    #endregion
}
