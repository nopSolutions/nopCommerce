using Nop.Services.Plugins;

namespace Nop.Web.Framework.Menu;

/// <summary>
/// Interface for plugins which have some items in the admin area menu
/// </summary>
[Obsolete("This interface is deprecated and will be removed in a future release. Please use AdminMenuCreatedEvent and/or ThirdPartyPluginsMenuItemCreatedEvent event handling.")]
public partial interface IAdminMenuPlugin : IPlugin
{
    /// <summary>
    /// Manage admin menu. You can use "SystemName" of menu items to manage existing item or add a new one.
    /// </summary>
    /// <param name="rootNode">Root item of the menu</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task ManageSiteMapAsync(AdminMenuItem rootNode);
}