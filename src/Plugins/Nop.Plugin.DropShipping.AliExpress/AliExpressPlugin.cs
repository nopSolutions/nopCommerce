using Nop.Core;
using Nop.Plugin.DropShipping.AliExpress.Components;
using Nop.Plugin.DropShipping.AliExpress.Services;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;
using Nop.Web.Framework.Infrastructure;


namespace Nop.Plugin.DropShipping.AliExpress;

/// <summary>
/// AliExpress Drop Shipping plugin
/// </summary>
public class AliExpressPlugin : BasePlugin, IMiscPlugin, IWidgetPlugin
{
    private const string COURIER_GUY_PLUGIN_SYSTEM_NAME = "Shipping.CourierGuy";
    private readonly ISettingService _settingService;
    private readonly ILocalizationService _localizationService;
    private readonly IWebHelper _webHelper;
    private readonly IScheduleTaskService _scheduleTaskService;
    private readonly IWorkContext _workContext;
    private readonly INotificationService _notificationService;
    private readonly IAliExpressService _aliExpressService;

    public AliExpressPlugin(
        ISettingService settingService,
        ILocalizationService localizationService,
        IWebHelper webHelper,
        IScheduleTaskService scheduleTaskService,
        IWorkContext workContext,
        INotificationService notificationService,
        IAliExpressService aliExpressService)
    {
        _settingService = settingService;
        _localizationService = localizationService;
        _webHelper = webHelper;
        _scheduleTaskService = scheduleTaskService;
        _workContext = workContext;
        _notificationService = notificationService;
        _aliExpressService = aliExpressService;
    }

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
        return $"{_webHelper.GetStoreLocation()}Admin/AliExpress/Configure";
    }

    /// <summary>
    /// Install the plugin
    /// </summary>
    public override async Task InstallAsync()
    {
        await EnsureSettingsAsync();
        await EnsureTranslations();
        await EnsureScheduledTasks();
        await base.InstallAsync();
    }

    private async Task EnsureScheduledTasks()
    {
        // Schedule Tasks
        if (await _scheduleTaskService.GetTaskByTypeAsync(
                "Nop.Plugin.DropShipping.AliExpress.ScheduledTasks.TokenRefreshTask") == null)
        {
            await _scheduleTaskService.InsertTaskAsync(new Nop.Core.Domain.ScheduleTasks.ScheduleTask
            {
                Name = "AliExpress Token Refresh",
                Seconds = 86400, // Daily
                Type = "Nop.Plugin.DropShipping.AliExpress.ScheduledTasks.TokenRefreshTask",
                Enabled = true,
                StopOnError = false
            });
        }

        if (await _scheduleTaskService.GetTaskByTypeAsync(
                "Nop.Plugin.DropShipping.AliExpress.ScheduledTasks.ProductSyncTask") == null)
        {
            await _scheduleTaskService.InsertTaskAsync(new Nop.Core.Domain.ScheduleTasks.ScheduleTask
            {
                Name = "AliExpress Product Sync",
                Seconds = 86400, // Daily
                Type = "Nop.Plugin.DropShipping.AliExpress.ScheduledTasks.ProductSyncTask",
                Enabled = true,
                StopOnError = false
            });
        }

        if (await _scheduleTaskService.GetTaskByTypeAsync(
                "Nop.Plugin.DropShipping.AliExpress.ScheduledTasks.OrderTrackingTask") == null)
        {
            await _scheduleTaskService.InsertTaskAsync(new Nop.Core.Domain.ScheduleTasks.ScheduleTask
            {
                Name = "AliExpress Order Tracking",
                Seconds = 3600, // Hourly
                Type = "Nop.Plugin.DropShipping.AliExpress.ScheduledTasks.OrderTrackingTask",
                Enabled = true,
                StopOnError = false
            });
        }
    }

    private async Task EnsureTranslations()
    {
        // Locales
        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.DropShipping.AliExpress.Fields.AppKey"] = "App Key",
            ["Plugins.DropShipping.AliExpress.Fields.AppKey.Hint"] =
                "Enter your AliExpress App Key from the developer console.",
            ["Plugins.DropShipping.AliExpress.Fields.AppSecret"] = "App Secret",
            ["Plugins.DropShipping.AliExpress.Fields.AppSecret.Hint"] =
                "Enter your AliExpress App Secret from the developer console.",
            ["Plugins.DropShipping.AliExpress.Fields.AccessToken"] = "Access Token",
            ["Plugins.DropShipping.AliExpress.Fields.AccessToken.Hint"] =
                "Current access token (populated after authorization).",
            ["Plugins.DropShipping.AliExpress.Fields.RefreshToken"] = "Refresh Token",
            ["Plugins.DropShipping.AliExpress.Fields.RefreshToken.Hint"] =
                "Current refresh token (populated after authorization).",
            ["Plugins.DropShipping.AliExpress.Fields.AccessTokenExpiresOn"] = "Access Token Expires",
            ["Plugins.DropShipping.AliExpress.Fields.AccessTokenExpiresOn.Hint"] =
                "When the current access token expires.",
            ["Plugins.DropShipping.AliExpress.Fields.RefreshTokenExpiresOn"] = "Refresh Token Expires",
            ["Plugins.DropShipping.AliExpress.Fields.RefreshTokenExpiresOn.Hint"] =
                "When the current refresh token expires.",
            ["Plugins.DropShipping.AliExpress.Fields.DefaultMarginPercentage"] = "Default Margin %",
            ["Plugins.DropShipping.AliExpress.Fields.DefaultMarginPercentage.Hint"] =
                "Default profit margin percentage to apply to products.",
            ["Plugins.DropShipping.AliExpress.Fields.VatPercentage"] = "VAT %",
            ["Plugins.DropShipping.AliExpress.Fields.VatPercentage.Hint"] =
                "VAT percentage to include in price calculations.",
            ["Plugins.DropShipping.AliExpress.Fields.CustomsDutyPercentage"] = "Customs Duty %",
            ["Plugins.DropShipping.AliExpress.Fields.CustomsDutyPercentage.Hint"] =
                "Customs duty percentage (typically 10%).",
            ["Plugins.DropShipping.AliExpress.Fields.DefaultShippingCountry"] = "Default Shipping Country",
            ["Plugins.DropShipping.AliExpress.Fields.DefaultShippingCountry.Hint"] =
                "Default country code for shipping calculations (e.g., ZA for South Africa).",
            ["Plugins.DropShipping.AliExpress.Fields.DefaultCurrency"] = "Default Currency",
            ["Plugins.DropShipping.AliExpress.Fields.DefaultCurrency.Hint"] =
                "Default currency code (e.g., ZAR, USD).",
            ["Plugins.DropShipping.AliExpress.Fields.EnableDailySync"] = "Enable Daily Sync",
            ["Plugins.DropShipping.AliExpress.Fields.EnableDailySync.Hint"] =
                "Automatically sync product prices and availability daily.",
            ["Plugins.DropShipping.AliExpress.Fields.DailySyncHour"] = "Daily Sync Hour",
            ["Plugins.DropShipping.AliExpress.Fields.DailySyncHour.Hint"] = "Hour of day (0-23) to run daily sync.",
            ["Plugins.DropShipping.AliExpress.Fields.AutoCreateOrders"] = "Auto Create Orders",
            ["Plugins.DropShipping.AliExpress.Fields.AutoCreateOrders.Hint"] =
                "Automatically create orders on AliExpress when customers place orders.",
            ["Plugins.DropShipping.AliExpress.Fields.AutoCreateLocalShipments"] = "Auto Create Local Shipments",
            ["Plugins.DropShipping.AliExpress.Fields.AutoCreateLocalShipments.Hint"] =
                "Automatically create local shipments with Courier Guy when AliExpress marks orders as delivered.",
            ["Plugins.DropShipping.AliExpress.Fields.RedirectUri"] = "Your Redirect Action URI Route",
            ["Plugins.DropShipping.AliExpress.Fields.RedirectUri.Hint"] =
                "The redirect URI route to handle OAuth callbacks (e.g., /Plugins/DropShippingAliExpress/OAuthCallback).",
            ["Plugins.DropShipping.AliExpress.Fields.RedirectUriHost"] = "Your Redirect URI Host",
            ["Plugins.DropShipping.AliExpress.Fields.RedirectUriHost.Hint"] =
                "The host part of your redirect URI (e.g., https://www.yourstore.com). Default is your store URL.",
            ["Plugins.DropShipping.AliExpress.Fields.TokenRefreshDaysBeforeExpiry"] = "Token Refresh Days",
            ["Plugins.DropShipping.AliExpress.Fields.TokenRefreshDaysBeforeExpiry.Hint"] =
                "Number of days before token expiry to automatically refresh.",
            ["Plugins.DropShipping.AliExpress.Fields.UseSandbox"] = "Use Sandbox",
            ["Plugins.DropShipping.AliExpress.Fields.UseSandbox.Hint"] =
                "Use AliExpress sandbox environment for testing.",
            ["Plugins.DropShipping.AliExpress.AuthSuccess"] = "Authorization successful! Access token obtained.",
            ["Plugins.DropShipping.AliExpress.AuthFailed"] =
                "Authorization failed. Please check your credentials and try again.",
            ["Plugins.DropShipping.AliExpress.Auth.InvalidReferer"] =
                "Invalid referer. Authorization requests must come from api-sg.aliexpress.com."
        });
    }

    private async Task EnsureSettingsAsync()
    {
        var loadSettings = await _settingService.LoadSettingAsync<AliExpressSettings>();
        if (loadSettings.RedirectUriHost.Contains("localhost", StringComparison.OrdinalIgnoreCase))
        {
            var defaults = new AliExpressSettings();

            // Set default RedirectUriHost to store URL if it's still localhost
            loadSettings.RedirectUriHost = defaults.RedirectUriHost;
            loadSettings.AuthorizationUrl = await _aliExpressService.GetAuthorizationUrlAsync();

            await _settingService.SaveSettingAsync(loadSettings);
        }

        if (string.IsNullOrWhiteSpace(loadSettings.AppKey))
        {
            var settings = new AliExpressSettings
            {
                DefaultMarginPercentage = 25m,
                VatPercentage = 15m,
                CustomsDutyPercentage = 10m,
                DefaultShippingCountry = "ZA",
                DefaultCurrency = "ZAR",
                EnableDailySync = true,
                DailySyncHour = 2,
                AutoCreateOrders = true,
                AutoCreateLocalShipments = true,
                TokenRefreshDaysBeforeExpiry = 7,
                UseSandbox = false,
            };
            settings.AuthorizationUrl = await _aliExpressService.GetAuthorizationUrlAsync();
            await _settingService.SaveSettingAsync(settings);
        }
    }

    public override async Task UpdateAsync(
        string currentVersion,
        string targetVersion)
    {
        
        var currentSemVer = currentVersion.Split('.');
        var targetSemVer = targetVersion.Split('.');
        
        // If current 4.90.6 and Taregt 4.91.x Uninstall and Reinstall
        // if current 4.90.6 and Target 4.90.x Update
        // i.e major.minor.patch update patch and reinstall minor and major
        if (currentSemVer.Length == 3 && targetSemVer.Length == 3)
        {
            if (currentSemVer[0] != targetSemVer[0] || currentSemVer[1] != targetSemVer[1])
            {
                // Major or minor version change - uninstall and reinstall
                await UninstallAsync();
                
                
                await InstallAsync();
                return;
            }
        }   
        
        await EnsureSettingsAsync();
        await EnsureScheduledTasks();
        await EnsureTranslations();
        await base.UpdateAsync(currentVersion, targetVersion);
    }

    /// <summary>
    /// Uninstall the plugin
    /// </summary>
    public override async Task UninstallAsync()
    {
        // Settings
        await _settingService.DeleteSettingAsync<AliExpressSettings>();

        // Locales
        await _localizationService.DeleteLocaleResourcesAsync("Plugins.DropShipping.AliExpress");

        // Schedule Tasks
        var tokenRefreshTask =
            await _scheduleTaskService.GetTaskByTypeAsync(
                "Nop.Plugin.DropShipping.AliExpress.ScheduledTasks.TokenRefreshTask");
        if (tokenRefreshTask != null)
            await _scheduleTaskService.DeleteTaskAsync(tokenRefreshTask);

        var productSyncTask =
            await _scheduleTaskService.GetTaskByTypeAsync(
                "Nop.Plugin.DropShipping.AliExpress.ScheduledTasks.ProductSyncTask");
        if (productSyncTask != null)
            await _scheduleTaskService.DeleteTaskAsync(productSyncTask);

        var orderTrackingTask =
            await _scheduleTaskService.GetTaskByTypeAsync(
                "Nop.Plugin.DropShipping.AliExpress.ScheduledTasks.OrderTrackingTask");
        if (orderTrackingTask != null)
            await _scheduleTaskService.DeleteTaskAsync(orderTrackingTask);

        
        await base.UninstallAsync();
        
        
    }

    /// <summary>
    /// Gets widget zones where this plugin should be rendered
    /// </summary>
    /// <returns>Widget zones</returns>
    public Task<IList<string>> GetWidgetZonesAsync()
    {
        return Task.FromResult<IList<string>>(new List<string>
        {
            AdminWidgetZones.ProductDetailsBlock
        });
    }

    /// <summary>
    /// Gets a type of a view component for displaying widget
    /// </summary>
    /// <param name="widgetZone">Name of the widget zone</param>
    /// <returns>View component type</returns>
    public Type GetWidgetViewComponent(string widgetZone)
    {
        if (widgetZone == AdminWidgetZones.ProductDetailsBlock)
            return typeof(AliExpressProductSelectorViewComponent);

        return null;
    }

    /// <summary>
    /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
    /// </summary>
    public bool HideInWidgetList => false;
}