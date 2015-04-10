using Nop.Core.Plugins;

namespace Nop.Web.Framework.Menu
{
    /// <summary>
    /// Interface for plugins which have some items in the admin area menu
    /// </summary>
    public interface IAdminMenuPlugin : IPlugin
    {
        /// <summary>
        /// Build menu item
        /// </summary>
        /// <returns>Site map item</returns>
        SiteMapNode BuildMenuItem();
    }
}
