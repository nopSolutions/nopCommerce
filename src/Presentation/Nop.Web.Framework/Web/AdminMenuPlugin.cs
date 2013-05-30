using Nop.Core.Plugins;
using Telerik.Web.Mvc.UI;

namespace Nop.Web.Framework.Web
{
    public interface IAdminMenuPlugin : IPlugin
    {
        /// <summary>
        /// Authenticate a user (can he see this plugin menu item?)
        /// </summary>
        /// <returns></returns>
        bool Authenticate();

        /// <summary>
        /// Build menu item
        /// </summary>
        /// <param name="menuItemBuilder">Menu item builder</param>
        void BuildMenuItem(MenuItemBuilder menuItemBuilder);
    }
}
