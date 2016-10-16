using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Localization;
using DataShop.DemoPlugin.Domain;
using DataShop.DemoPlugin.Data;
using DataShop.DemoPlugin.Services;
using Nop.Web.Framework.Menu;

namespace DataShop.DemoPlugin
{
    public class DemoPluginProvider : BasePlugin, IAdminMenuPlugin
    {
        private readonly DataContext Context;

        public DemoPluginProvider(DataContext context)
        {
            this.Context = context;
        }

        public override void Install()
        {
            this.Context.Install();
            base.Install();
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {

            //var menuItemBuilder = new SiteMapNode()
            //{
            //    SystemName = "Demo Plugin",
            //    Title = "Demo Plugin Menu",
            //    Visible = true,
            //    RouteValues = new RouteValueDictionary() { { "Area", "Admin" } }
            //};

            //menuItemBuilder.ChildNodes.Add(new SiteMapNode
            //{
            //    SystemName = "DemoItems",
            //    Title = "Demo Items",
            //    //ControllerName = "AdminDemoItems",
            //    //ActionName = "Index",
            //    Url = "Admin/DemoPlugin/DemoItems",
            //    Visible = true,
            //    RouteValues = new RouteValueDictionary() { { "Area", "Admin" } }
            //});

            //rootNode.ChildNodes.Add(menuItemBuilder);

            var menuItem = new SiteMapNode()
            {
                SystemName = "AdminDemoItems",
                Title = "Admin Demo Items",
                ControllerName = "AdminDemoItems",
                ActionName = "Index",
                Visible = true,
                IconClass = "fa-dot-circle-o",
                RouteValues = new RouteValueDictionary() { { "area", null } },
            };

            var menuItem2 = new SiteMapNode()
            {
                SystemName = "AdminDemoItem",
                Title = "Admin Demo Item",
                ControllerName = "AdminDemoItems",
                ActionName = "GetDemoItem",
                Visible = true,
                IconClass = "fa-dot-circle-o",
                RouteValues = new RouteValueDictionary() { { "area", null } },
            };

            var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Third party plugins");

            if (pluginNode != null)
            {
                pluginNode.ChildNodes.Add(menuItem);
                pluginNode.ChildNodes.Add(menuItem2);
            }
            else
            {
                rootNode.ChildNodes.Add(menuItem);
                rootNode.ChildNodes.Add(menuItem2);
            }

        }

        public override void Uninstall()
        {
            this.Context.Uninstall();
            base.Uninstall();
        }

    }
}
