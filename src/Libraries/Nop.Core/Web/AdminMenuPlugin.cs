using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Nop.Core.Plugins;
using Telerik.Web.Mvc.UI;

namespace Nop.Core.Web
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

    public class AdminMenuPluginAttribute : BasePluginAttribute, IAdminMenuPlugin
    {
        private readonly string _title;
        private readonly string _url;

        public AdminMenuPluginAttribute(string friendlyName, string systemName,
            string title, string url, int displayOrder)
            : base(friendlyName, "IAdminMenuPlugin." + systemName, "", "", displayOrder)
        {
            _url = url;
            _title = title;
        }

        public void BuildMenuItem(MenuItemBuilder menuItemBuilder)
        {
            menuItemBuilder.Text(_title).Url(_url);
        }
    }
}
