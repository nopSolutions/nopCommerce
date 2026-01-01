using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;

namespace Nop.Plugin.DropShipping.AliExpress;

/// <summary>
/// AliExpress Drop Shipping plugin
/// </summary>
public class AliExpressPlugin : BasePlugin, IMiscPlugin
{
    private readonly ISettingService _settingService;
    private readonly ILocalizationService _localizationService;
    private readonly IWebHelper _webHelper;
    private readonly IScheduleTaskService _scheduleTaskService;

    public AliExpressPlugin(
        ISettingService settingService,
        ILocalizationService localizationService,
        IWebHelper webHelper,
        IScheduleTaskService scheduleTaskService)
    {
        _settingService = settingService;
        _localizationService = localizationService;
        _webHelper = webHelper;
        _scheduleTaskService = scheduleTaskService;
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
        // Settings
        await _settingService.SaveSettingAsync(new AliExpressSettings
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
            UseSandbox = false
        });

        // Locales
        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.DropShipping.AliExpress.Fields.AppKey"] = "App Key",
            ["Plugins.DropShipping.AliExpress.Fields.AppKey.Hint"] = "Enter your AliExpress App Key from the developer console.",
            ["Plugins.DropShipping.AliExpress.Fields.AppSecret"] = "App Secret",
            ["Plugins.DropShipping.AliExpress.Fields.AppSecret.Hint"] = "Enter your AliExpress App Secret from the developer console.",
            ["Plugins.DropShipping.AliExpress.Fields.AccessToken"] = "Access Token",
            ["Plugins.DropShipping.AliExpress.Fields.AccessToken.Hint"] = "Current access token (populated after authorization).",
            ["Plugins.DropShipping.AliExpress.Fields.RefreshToken"] = "Refresh Token",
            ["Plugins.DropShipping.AliExpress.Fields.RefreshToken.Hint"] = "Current refresh token (populated after authorization).",
            ["Plugins.DropShipping.AliExpress.Fields.AccessTokenExpiresOn"] = "Access Token Expires",
            ["Plugins.DropShipping.AliExpress.Fields.AccessTokenExpiresOn.Hint"] = "When the current access token expires.",
            ["Plugins.DropShipping.AliExpress.Fields.RefreshTokenExpiresOn"] = "Refresh Token Expires",
            ["Plugins.DropShipping.AliExpress.Fields.RefreshTokenExpiresOn.Hint"] = "When the current refresh token expires.",
            ["Plugins.DropShipping.AliExpress.Fields.DefaultMarginPercentage"] = "Default Margin %",
            ["Plugins.DropShipping.AliExpress.Fields.DefaultMarginPercentage.Hint"] = "Default profit margin percentage to apply to products.",
            ["Plugins.DropShipping.AliExpress.Fields.VatPercentage"] = "VAT %",
            ["Plugins.DropShipping.AliExpress.Fields.VatPercentage.Hint"] = "VAT percentage to include in price calculations.",
            ["Plugins.DropShipping.AliExpress.Fields.CustomsDutyPercentage"] = "Customs Duty %",
            ["Plugins.DropShipping.AliExpress.Fields.CustomsDutyPercentage.Hint"] = "Customs duty percentage (typically 10%).",
            ["Plugins.DropShipping.AliExpress.Fields.DefaultShippingCountry"] = "Default Shipping Country",
            ["Plugins.DropShipping.AliExpress.Fields.DefaultShippingCountry.Hint"] = "Default country code for shipping calculations (e.g., ZA for South Africa).",
            ["Plugins.DropShipping.AliExpress.Fields.DefaultCurrency"] = "Default Currency",
            ["Plugins.DropShipping.AliExpress.Fields.DefaultCurrency.Hint"] = "Default currency code (e.g., ZAR, USD).",
            ["Plugins.DropShipping.AliExpress.Fields.EnableDailySync"] = "Enable Daily Sync",
            ["Plugins.DropShipping.AliExpress.Fields.EnableDailySync.Hint"] = "Automatically sync product prices and availability daily.",
            ["Plugins.DropShipping.AliExpress.Fields.DailySyncHour"] = "Daily Sync Hour",
            ["Plugins.DropShipping.AliExpress.Fields.DailySyncHour.Hint"] = "Hour of day (0-23) to run daily sync.",
            ["Plugins.DropShipping.AliExpress.Fields.AutoCreateOrders"] = "Auto Create Orders",
            ["Plugins.DropShipping.AliExpress.Fields.AutoCreateOrders.Hint"] = "Automatically create orders on AliExpress when customers place orders.",
            ["Plugins.DropShipping.AliExpress.Fields.AutoCreateLocalShipments"] = "Auto Create Local Shipments",
            ["Plugins.DropShipping.AliExpress.Fields.AutoCreateLocalShipments.Hint"] = "Automatically create local shipments with Courier Guy when AliExpress marks orders as delivered.",
            ["Plugins.DropShipping.AliExpress.Fields.TokenRefreshDaysBeforeExpiry"] = "Token Refresh Days",
            ["Plugins.DropShipping.AliExpress.Fields.TokenRefreshDaysBeforeExpiry.Hint"] = "Number of days before token expiry to automatically refresh.",
            ["Plugins.DropShipping.AliExpress.Fields.UseSandbox"] = "Use Sandbox",
            ["Plugins.DropShipping.AliExpress.Fields.UseSandbox.Hint"] = "Use AliExpress sandbox environment for testing.",
            ["Plugins.DropShipping.AliExpress.AuthSuccess"] = "Authorization successful! Access token obtained.",
            ["Plugins.DropShipping.AliExpress.AuthFailed"] = "Authorization failed. Please check your credentials and try again."
        });

        // Schedule Tasks
        if (await _scheduleTaskService.GetTaskByTypeAsync("Nop.Plugin.DropShipping.AliExpress.ScheduledTasks.TokenRefreshTask") == null)
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

        if (await _scheduleTaskService.GetTaskByTypeAsync("Nop.Plugin.DropShipping.AliExpress.ScheduledTasks.ProductSyncTask") == null)
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

        if (await _scheduleTaskService.GetTaskByTypeAsync("Nop.Plugin.DropShipping.AliExpress.ScheduledTasks.OrderTrackingTask") == null)
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

        await base.InstallAsync();
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
        var tokenRefreshTask = await _scheduleTaskService.GetTaskByTypeAsync("Nop.Plugin.DropShipping.AliExpress.ScheduledTasks.TokenRefreshTask");
        if (tokenRefreshTask != null)
            await _scheduleTaskService.DeleteTaskAsync(tokenRefreshTask);

        var productSyncTask = await _scheduleTaskService.GetTaskByTypeAsync("Nop.Plugin.DropShipping.AliExpress.ScheduledTasks.ProductSyncTask");
        if (productSyncTask != null)
            await _scheduleTaskService.DeleteTaskAsync(productSyncTask);

        var orderTrackingTask = await _scheduleTaskService.GetTaskByTypeAsync("Nop.Plugin.DropShipping.AliExpress.ScheduledTasks.OrderTrackingTask");
        if (orderTrackingTask != null)
            await _scheduleTaskService.DeleteTaskAsync(orderTrackingTask);

        await base.UninstallAsync();
    }
}
