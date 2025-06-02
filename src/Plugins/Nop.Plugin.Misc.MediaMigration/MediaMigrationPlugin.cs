using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core;
using Nop.Plugin.Misc.MediaMigration.Components;
using Nop.Plugin.Misc.MediaMigration.Data;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;


namespace Nop.Plugin.Misc.MediaMigration;

public class MediaMigrationPlugin : BasePlugin, IWidgetPlugin
{
    private readonly IWebHelper _webHelper;

    private readonly ISettingService _settingService;

    public bool HideInWidgetList => false;

    public MediaMigrationPlugin(ISettingService settingService, IWebHelper webHelper)
    {
        _settingService = settingService;
        _webHelper = webHelper;

    }


    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
        return $"{_webHelper.GetStoreLocation()}Admin/MediaMigrationPlugin/Configure";
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
        return Task.FromResult<IList<string>>(new List<string>
        {
            AdminWidgetZones.CategoryListButtons,
            AdminWidgetZones.SpecificationAttributeListButtons,
            AdminWidgetZones.ProductListButtons,
            AdminWidgetZones.CustomerListButtons
        });
    }

    /// <summary>
    /// Gets a name of a view component for displaying widget
    /// </summary>
    /// <param name="widgetZone">Name of the widget zone</param>
    /// <returns>View component name</returns>
    public Type GetWidgetViewComponent(string widgetZone)
    {
        //if (new List<string>
        //    {
        //    AdminWidgetZones.CategoryListButtons
        //    }.Contains(widgetZone))
        //    return typeof(WoocommerceImportButtonViewComponent);

        //if (new List<string>
        //    {
        //    AdminWidgetZones.SpecificationAttributeListButtons
        //    }.Contains(widgetZone))
        //    return typeof(WoocommerceSpecificationAttributeButtonViewComponent);

        if (new List<string>
            {
            AdminWidgetZones.ProductListButtons,
            }.Contains(widgetZone))
            return typeof(ProductMediaImportButtonViewComponent);

        //if (new List<string>
        //    {
        //    AdminWidgetZones.CustomerListButtons,
        //    }.Contains(widgetZone))
        //    return typeof(WoocommerceCustomerImportButtonViewComponent);

        return null;
    }



    public override async Task InstallAsync()
    {
        await base.InstallAsync();
    }

    public override async Task UninstallAsync()
    {
        await base.UninstallAsync();
    }



}
