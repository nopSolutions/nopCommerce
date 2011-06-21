using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Nop.Core.Plugins;
using Telerik.Web.Mvc.UI;

namespace Nop.Web.Framework.Web
{
    public abstract class AdminMenuPlugin : BasePlugin, IAdminMenuPlugin
    {
        protected AdminMenuPlugin(string friendlyName, string systemName, int displayOrder = 0)
            : base(friendlyName, "IAdminMenuPlugin." + systemName,
            "", "", displayOrder)
        {

        }

        public abstract void BuildMenuItem(MenuItemBuilder menuItemBuilder);
    }

    public interface IAdminMenuPlugin : IPlugin
    {
        void BuildMenuItem(MenuItemBuilder menuItemBuilder);
    }
}
