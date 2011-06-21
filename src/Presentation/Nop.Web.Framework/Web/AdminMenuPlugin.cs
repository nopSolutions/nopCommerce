using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Nop.Core.Plugins;
using Telerik.Web.Mvc.UI;

namespace Nop.Web.Framework.Web
{
    public interface IAdminMenuPlugin : IPlugin
    {
        void BuildMenuItem(MenuItemBuilder menuItemBuilder);
    }
}
