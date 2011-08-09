using System.Collections.Generic;
using System.Web.Routing;
using Nop.Core.Domain.Cms;
using Nop.Core.Plugins;
using Nop.Services.Cms;

namespace Nop.Plugin.Widgets.LivePersonChat
{
    /// <summary>
    /// Live person provider
    /// </summary>
    public class LivePersonChatPlugin : BasePlugin, IWidgetPlugin
    {
        /// <summary>
        /// Get a list of supported widget zones; if empty list is returned, then all zones are supported
        /// </summary>
        /// <returns>A list of supported widget zones</returns>
        public IList<WidgetZone> SupportedWidgetZones()
        {
            //limit widget to the following zones
            return new List<WidgetZone>() 
            { 
                WidgetZone.BeforeLeftSideColumn, 
                WidgetZone.AfterLeftSideColumn, 
                WidgetZone.BeforeRightSideColumn, 
                WidgetZone.AfterRightSideColumn
            };
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="widgetId">Widget identifier</param>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(int widgetId, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "WidgetsLivePersonChat";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Widgets.LivePersonChat.Controllers" }, { "area", null }, { "widgetId", widgetId } };
        }

        /// <summary>
        /// Gets a route for displaying widget
        /// </summary>
        /// <param name="widgetId">Widget identifier</param>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetDisplayWidgetRoute(int widgetId, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PublicInfo";
            controllerName = "WidgetsLivePersonChat";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Widgets.LivePersonChat.Controllers" }, { "area", null }, { "widgetId", widgetId } };
        }
        
    }
}
