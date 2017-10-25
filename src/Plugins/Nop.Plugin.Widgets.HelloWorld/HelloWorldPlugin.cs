using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Plugins;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;

namespace Nop.Plugin.Widgets.HelloWorld
{
    /// <summary>
    /// 插件核心类
    /// </summary>
    public class HelloWorldPlugin : BasePlugin, IWidgetPlugin
    {
        public HelloWorldPlugin() { }

        /// <summary>
        /// 获得配置页路由信息
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="routeValues"></param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "WidgetsHelloWorld";
            routeValues = new RouteValueDictionary() {
                { "Namespaces", "Nop.Plugin.Widgets.HelloWorld.Controllers" },
                { "area", null }
            };
        }

        /// <summary>
        /// 获得显示路由信息
        /// </summary>
        /// <param name="widgetZone"></param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="routeValues"></param>
        public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PublicInfo";
            controllerName = "WidgetsHelloWorld";
            routeValues = new RouteValueDictionary() {
                { "Namespaces", "Nop.Plugin.Widgets.HelloWorld.Controllers" },
                { "area", null },
                { "widgetZone", widgetZone }
            };
        }

        /// <summary>
        /// 获得显示位置
        /// </summary>
        /// <returns></returns>
        public IList<string> GetWidgetZones()
        {
            return new List<string>() { "home_page_helloworld" };
        }

        /// <summary>
        /// 安装插件
        /// </summary>
        public override void Install()
        {
            base.Install();
        }
        
        /// <summary>
        /// 卸载插件
        /// </summary>
        public override void Uninstall()
        {
            base.Uninstall();
        }
    }
}
