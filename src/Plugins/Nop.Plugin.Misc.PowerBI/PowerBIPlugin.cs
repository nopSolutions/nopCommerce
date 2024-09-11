using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Misc.PowerBI;

/// <summary>
/// Represents the Power BI helper plugin
/// </summary>
public class PowerBIPlugin : BasePlugin, IAdminMenuPlugin, IMiscPlugin
{
    #region Fields

    private readonly IPermissionService _permissionService;
    private readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public PowerBIPlugin(IPermissionService permissionService,
        IWebHelper webHelper)
    {
        _permissionService = permissionService;
        _webHelper = webHelper;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
        return $"{_webHelper.GetStoreLocation()}Admin/PowerBI/Configure";
    }

    /// <summary>
    /// Install the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
        await base.InstallAsync();
    }

    public async Task ManageSiteMapAsync(SiteMapNode rootNode)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermission.Configuration.MANAGE_PLUGINS))
            return;

        var reports = rootNode.ChildNodes.FirstOrDefault(node => node.SystemName.Equals("Reports"));
        if (reports == null)
            return;

        var report = reports.ChildNodes.FirstOrDefault(node => node.SystemName.Equals("Sales summary"));

        if (report == null)
            return;

        var index = reports.ChildNodes.IndexOf(report);

        if (index < 0)
            return;

        reports.ChildNodes.Insert(index, new SiteMapNode
        {
            SystemName = "Power BI integration",
            Title = "Power BI (advanced reports)",
            ControllerName = "PowerBI",
            ActionName = "Configure",
            IconClass = "far fa-dot-circle",
            Visible = true,
            RouteValues = new RouteValueDictionary { { "area", AreaNames.ADMIN } }
        });
    }

    /// <summary>
    /// Uninstall the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UninstallAsync()
    {
        await base.UninstallAsync();
    }

    #endregion
}
