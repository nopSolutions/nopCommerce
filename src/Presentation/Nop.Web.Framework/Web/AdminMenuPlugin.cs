using Nop.Core.Plugins;
using Telerik.Web.Mvc.UI;

namespace Nop.Web.Framework.Web
{
    public interface IAdminMenuPlugin : IPlugin
    {
        void BuildMenuItem();
        void BuildMenuItem(MenuItemBuilder menuItemBuilder);
    }
}
