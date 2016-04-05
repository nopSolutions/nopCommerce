using Nop.Core.Plugins;
using Nop.Services.Cms;
using System;
using System.Collections.Generic;
using System.Web.Routing;

namespace Nop.Plugin.Widgets.PromoSilder
{
    public class PromoSilderPlugin : BasePlugin, IWidgetPlugin
    {
        public PromoSilderPlugin()
        {

        }

        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            //actionName = "PromoSilderWidget";
            //controllerName = "PromoSilder";
            //routeValues = new RouteValueDictionary()
            //{
            //    { "Namespace", "Nop.Plugin.Widgets.PromoSilder.Controllers" },
            //    { "area", null }
            //};
            throw new NotImplementedException();
        }

        public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            //actionName = "PromoSilderWidget";
            //controllerName = "PromoSilder";
            //routeValues = new RouteValueDictionary()
            //{
            //    { "Namespace", "Nop.Plugin.Widgets.PromoSilder.Controllers" },
            //    { "area", null },
            //    { "widgetZone", widgetZone }
            //};
            throw new NotImplementedException();
        }

        public IList<string> GetWidgetZones()
        {
            return new List<string>();
        }

        public override void Install()
        {
            base.Install();
        }

        public override void Uninstall()
        {
            base.Uninstall();
        }
    }

}
