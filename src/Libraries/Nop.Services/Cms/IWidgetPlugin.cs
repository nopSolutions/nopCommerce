using System.Collections.Generic;
using System.Web.Routing;
using Nop.Core.Domain.Cms;
using Nop.Core.Plugins;

namespace Nop.Services.Cms
{
    /// <summary>
    /// Provides an interface for creating tax providers
    /// </summary>
    public partial interface IWidgetPlugin : IPlugin
    {
        /// <summary>
        /// Get a list of supported widget zones; if empty list is returned, then all zones are supported
        /// </summary>
        /// <returns>A list of supported widget zones</returns>
        IList<WidgetZone> SupportedWidgetZones();

        /// <summary>
        /// Gets a route for plugin configuration
        /// </summary>
        /// <param name="widgetId">Widget identifier</param>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        void GetConfigurationRoute(int widgetId, out string actionName, out string controllerName, out RouteValueDictionary routeValues);
        

        /// <summary>
        /// Gets a route for displaying widget
        /// </summary>
        /// <param name="widgetId">Widget identifier</param>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        void GetDisplayWidgetRoute(int widgetId, out string actionName, out string controllerName, out RouteValueDictionary routeValues);
    }
}
