using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Media;
using Nop.Plugin.Misc.Omnisend.Components;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Misc.Omnisend;

/// <summary>
/// Represents Omnisend plugin
/// </summary>
public class OmnisendPlugin : BasePlugin, IMiscPlugin, IWidgetPlugin
{
    #region Fields

    private readonly IActionContextAccessor _actionContextAccessor;
    private readonly ILocalizationService _localizationService;
    private readonly ISettingService _settingService;
    private readonly IUrlHelperFactory _urlHelperFactory;
    private readonly WidgetSettings _widgetSettings;

    #endregion

    #region Ctor

    public OmnisendPlugin(IActionContextAccessor actionContextAccessor,
        ILocalizationService localizationService,
        ISettingService settingService,
        IUrlHelperFactory urlHelperFactory,
        WidgetSettings widgetSettings)
    {
        _actionContextAccessor = actionContextAccessor;
        _localizationService = localizationService;
        _settingService = settingService;
        _urlHelperFactory = urlHelperFactory;
        _widgetSettings = widgetSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
        if (_actionContextAccessor.ActionContext != null)
            return _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext)
                .RouteUrl(OmnisendDefaults.ConfigurationRouteName);

        return base.GetConfigurationPageUrl();
    }

    /// <summary>
    /// Gets a type of a view component for displaying widget
    /// </summary>
    /// <param name="widgetZone">Name of the widget zone</param>
    /// <returns>View component type</returns>
    public Type GetWidgetViewComponent(string widgetZone)
    {
        if (widgetZone is null)
            throw new ArgumentNullException(nameof(widgetZone));

        var zones = GetWidgetZonesAsync().Result;

        return zones.Any(widgetZone.Equals) ? typeof(WidgetsOmnisendViewComponent) : null;
    }

    /// <summary>
    /// Gets widget zones where this widget should be rendered
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the widget zones
    /// </returns>
    public Task<IList<string>> GetWidgetZonesAsync()
    {
        return Task.FromResult<IList<string>>(new List<string> { PublicWidgetZones.BodyStartHtmlTagAfter, PublicWidgetZones.ProductDetailsBottom });
    }

    /// <summary>
    /// Install plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
        //ensure MediaSettings.UseAbsoluteImagePath is enabled (used for images uploading)
        await _settingService.SetSettingAsync($"{nameof(MediaSettings)}.{nameof(MediaSettings.UseAbsoluteImagePath)}", true, clearCache: false);

        var omnisendTrackingScript =
            @$"<!-- Omnisend starts -->
<script type=""text/javascript"">
    //OMNISEND-SNIPPET-SOURCE-CODE-V1
    window.omnisend = window.omnisend || [];
    omnisend.push([""accountID"", ""{OmnisendDefaults.BrandId}""]);
    omnisend.push([""track"", ""$pageViewed""]);
    !function(){{var e=document.createElement(""script"");e.type=""text/javascript"",e.async=!0,e.src=""https://omnisnippet1.com/inshop/launcher-v2.js"";var t=document.getElementsByTagName(""script"")[0];t.parentNode.insertBefore(e,t)}}();
</script>
<!-- Omnisend stops -->";
        var omnisendProductPageScript =
            @$"<!-- Omnisend starts -->
<script type=""text/javascript"">
    omnisend.push([""track"", ""$productViewed"",{{
     $productID:""{OmnisendDefaults.ProductId}"",
     $variantID: ""{OmnisendDefaults.Sku}"",
     $currency: ""{OmnisendDefaults.Currency}"",
     $price: {OmnisendDefaults.Price},
     $title:""{OmnisendDefaults.Title}"",
     $imageUrl: ""{OmnisendDefaults.ImageUrl}"",
     $productUrl:""{OmnisendDefaults.ProductUrl}""
    }}]);
</script>
<!-- Omnisend stops -->";
        var identifyContactScript = @$"<script type=""text/javascript"">
    omnisend.identifyContact({{
        ""email"": ""{OmnisendDefaults.Email}""
    }})
</script>";

        var settings = new OmnisendSettings
        {
            ApiKey = "",
            TrackingScript = omnisendTrackingScript,
            ProductScript = omnisendProductPageScript,
            IdentifyContactScript = identifyContactScript,
            UseTracking = true,
            LogRequests = false,
            LogRequestErrors = true,
            PageSize = OmnisendDefaults.PageSize,
            RequestTimeout = OmnisendDefaults.RequestTimeout
        };

        await _settingService.SaveSettingAsync(settings);

        if (!_widgetSettings.ActiveWidgetSystemNames.Contains(OmnisendDefaults.SystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Add(OmnisendDefaults.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }

        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.Misc.Omnisend.CantGetBrandId"] = "Failed to get the required data from the Omnisend server. Please check if the API key is correct and save the settings again. Error details can be found in the Log page",
            ["Plugins.Misc.Omnisend.Credentials"] = "Credentials",
            ["Plugins.Misc.Omnisend.Synchronization"] = "Synchronization",
            ["Plugins.Misc.Omnisend.SyncContacts"] = "Sync contacts",
            ["Plugins.Misc.Omnisend.SyncProducts"] = "Sync products",
            ["Plugins.Misc.Omnisend.SyncOrders"] = "Sync orders",
            ["Plugins.Misc.Omnisend.SyncCategories"] = "Sync categories",
            ["Plugins.Misc.Omnisend.BatchesInProcess"] = "Batches in process",
            ["Plugins.Misc.Omnisend.BatchesInProcess.StartedAt"] = "Started at",
            ["Plugins.Misc.Omnisend.BatchesInProcess.Status"] = "Status",
            ["Plugins.Misc.Omnisend.BatchesInProcess.TotalCount"] = "Total count",
            ["Plugins.Misc.Omnisend.BatchesInProcess.FinishedCount"] = "Finished count",
            ["Plugins.Misc.Omnisend.BatchesInProcess.ErrorsCount"] = "Error count",
            ["Plugins.Misc.Omnisend.BatchesInProcess.EndedAt"] = "Ended at",
            ["Plugins.Misc.Omnisend.BatchesInProcess.SyncType"] = "Type of sync",
            ["Plugins.Misc.Omnisend.Fields.ApiKey"] = "API key",
            ["Plugins.Misc.Omnisend.Fields.ApiKey.Hint"] = "Enter the Omnisend integration API key.",
            ["Plugins.Misc.Omnisend.Fields.ApiKey.Required"] = "API key is required",
            ["Plugins.Misc.Omnisend.Fields.UseTracking"] = "Use tracking",
            ["Plugins.Misc.Omnisend.Fields.UseTracking.Hint"] = "Determine whether to use tracking to get statistics with Omnisend, such as the behavior of individual subsribers on your website, including purchases made, movements on your site and subsequent segmentation."
        });

        await base.InstallAsync();
    }

    /// <summary>
    /// Uninstall the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UninstallAsync()
    {
        if (_widgetSettings.ActiveWidgetSystemNames.Contains(OmnisendDefaults.SystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Remove(OmnisendDefaults.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }
        await _settingService.DeleteSettingAsync<OmnisendSettings>();

        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Misc.Omnisend");

        await base.UninstallAsync();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
    /// </summary>
    public bool HideInWidgetList => true;

    #endregion
}