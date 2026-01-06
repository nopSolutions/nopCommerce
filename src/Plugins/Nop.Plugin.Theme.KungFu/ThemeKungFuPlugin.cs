using System.Collections.Generic;
using Nop.Core.Domain.Common;
using Nop.Plugin.Theme.KungFu.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Mvc.Routing;


namespace Nop.Plugin.Theme.KungFu;

public class ThemeKungFuPlugin : BasePlugin
{
    private readonly ILocalizationService _localizationService;
    private readonly INopUrlHelper _nopUrlHelper;
    private readonly ISettingService _settingService;
    private readonly IThemeKungFuService _themeKungFuService;

    public ThemeKungFuPlugin(ILocalizationService localizationService,
        INopUrlHelper nopUrlHelper,
        ISettingService settingService,
        IThemeKungFuService themeKungFuService)
    {
        _localizationService = localizationService;
        _nopUrlHelper = nopUrlHelper;
        _settingService = settingService;
        _themeKungFuService = themeKungFuService;
    }

    public override string GetConfigurationPageUrl()
    {
        return _nopUrlHelper.RouteUrl(ThemeKungFuDefaults.ConfigurationRouteName);
    }

    public override async Task InstallAsync()
    {
        await _settingService.SaveSettingAsync(new ThemeKungFuSettings
        {
            SyncAutomatically = true,
            LastSyncedVersion = null,
            LastSyncedOnUtc = null,
            EnableAISageMessages = false
        });

        // Set branded PDF footer text with wisdom quotes
        var pdfSettings = await _settingService.LoadSettingAsync<PdfSettings>();
        pdfSettings.InvoiceFooterTextColumn1 = "Kung Fu Store - Cultivating Excellence Through Discipline\n\"The journey of a thousand miles begins with a single step.\" - Lao Tzu";
        pdfSettings.InvoiceFooterTextColumn2 = "Thank you for your order\nQuestions? Visit our website or contact support\nTrain with purpose, rest with peace";
        await _settingService.SaveSettingAsync(pdfSettings);

        await _themeKungFuService.EnsureSyncedAsync(force: true);

        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.Theme.KungFu.Fields.LastSyncedOn"] = "Last sync",
            ["Plugins.Theme.KungFu.Fields.SyncAutomatically"] = "Sync changes automatically",
            ["Plugins.Theme.KungFu.Fields.SyncAutomatically.Hint"] = "If enabled, the plugin will keep the theme files, topics, and message templates aligned whenever the version changes.",
            ["Plugins.Theme.KungFu.Notifications.Synced"] = "Kung Fu theme assets, topics, and message templates are synced",
            ["Plugins.Theme.KungFu.Status.Fresh"] = "Everything is live and looking sharp.",
            ["Plugins.Theme.KungFu.Status.Never"] = "No sync has been run yet",
            ["Plugins.Theme.KungFu.Status.Outdated"] = "Updates are waiting to be promoted",
            ["Plugins.Theme.KungFu.Configure.Heading"] = "Kung Fu control center",
            ["Plugins.Theme.KungFu.Configure.Subheading"] = "Ship new theme content with confidence",
            ["Plugins.Theme.KungFu.Configure.Outdated"] = "Outdated",
            ["Plugins.Theme.KungFu.Configure.UpToDate"] = "Up to date",
            ["Plugins.Theme.KungFu.Configure.SyncNow"] = "Resync now",
            ["Plugins.Theme.KungFu.Configure.Version"] = "Plugin version",
            ["Plugins.Theme.KungFu.Fields.AzureOpenAIEndpoint"] = "Azure OpenAI Endpoint",
            ["Plugins.Theme.KungFu.Fields.AzureOpenAIEndpoint.Hint"] = "Your Azure OpenAI endpoint URL (e.g., https://your-resource.openai.azure.com/)",
            ["Plugins.Theme.KungFu.Fields.AzureOpenAIKey"] = "Azure OpenAI API Key",
            ["Plugins.Theme.KungFu.Fields.AzureOpenAIKey.Hint"] = "Your Azure OpenAI API key for authentication",
            ["Plugins.Theme.KungFu.Fields.AzureOpenAIDeploymentName"] = "Deployment Name",
            ["Plugins.Theme.KungFu.Fields.AzureOpenAIDeploymentName.Hint"] = "The name of your Azure OpenAI deployment (e.g., gpt-4, gpt-35-turbo)",
            ["Plugins.Theme.KungFu.Fields.EnableAISageMessages"] = "Enable AI Sage Messages",
            ["Plugins.Theme.KungFu.Fields.EnableAISageMessages.Hint"] = "When enabled, customers receive a personalized message from a Chinese sage after completing payment, with wisdom about their order",
            ["Plugins.Theme.KungFu.Configure.AISection"] = "AI Sage Configuration",
            ["Plugins.Theme.KungFu.Configure.AISection.Description"] = "Configure Azure OpenAI to generate personalized sage messages for your customers after payment completion"
        });

        await base.InstallAsync();
    }

    public override async Task UninstallAsync()
    {
        await _settingService.DeleteSettingAsync<ThemeKungFuSettings>();
        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Theme.KungFu.");

        await base.UninstallAsync();
    }

    public override Task UpdateAsync(string currentVersion, string targetVersion)
    {
        return _themeKungFuService.EnsureSyncedAsync(force: true);
    }
}
