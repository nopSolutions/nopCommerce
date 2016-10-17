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
            var subMenu1 = new SiteMapNode()
            {
                SystemName = "AdminDemoItems",
                Title = "Admin Demo Items",
                ControllerName = "AdminDemoItems",
                ActionName = "Index",
                Visible = true,
                IconClass = "fa-dot-circle-o",
                RouteValues = new RouteValueDictionary() { { "area", null } },
            };

            var subMenu2 = new SiteMapNode()
            {
                SystemName = "AdminDemoItem",
                Title = "Admin Demo Item",
                ControllerName = "AdminDemoItems",
                ActionName = "GetDemoItem",
                Visible = true,
                IconClass = "fa-smile-o",
                RouteValues = new RouteValueDictionary() { { "area", null } },
            };

            var mainMenu = new SiteMapNode()
            {
                SystemName = "AdminDemoPLugin",
                Title = "Admin Demo Plugin",
                Visible = true,
                IconClass = "fa-database",
                RouteValues = new RouteValueDictionary() { { "area", null } },
            };

            mainMenu.ChildNodes.Add(subMenu1);
            mainMenu.ChildNodes.Add(subMenu2);

            rootNode.ChildNodes.Add(mainMenu);

            //rootNode.ChildNodes.Insert(0, mainMenu);
            //rootNode.ChildNodes.Insert(2, mainMenu);

            //var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Third party plugins");

            //if (pluginNode != null)
            //{
            //    pluginNode.ChildNodes.Add(menuItem);
            //}
            //else
            //{
            //    rootNode.ChildNodes.Add(menuItem);       
            //}
        }

        public override void Uninstall()
        {
            this.Context.Uninstall();
            base.Uninstall();
        }

    }
}
