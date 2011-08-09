using System.Collections.Generic;
using System.Web.Routing;
using Nop.Core.Domain.Cms;
using Nop.Core.Plugins;
using Nop.Services.Cms;
using Nop.Services.Configuration;

namespace Nop.Plugin.Widgets.GoogleAnalytics
{
    /// <summary>
    /// Live person provider
    /// </summary>
    public class GoogleAnalyticPlugin : BasePlugin, IWidgetPlugin
    {
        private readonly ISettingService _settingService;

        public GoogleAnalyticPlugin(ISettingService settingService)
        {
            this._settingService = settingService;
        }
        /// <summary>
        /// Get a list of supported widget zones; if empty list is returned, then all zones are supported
        /// </summary>
        /// <returns>A list of supported widget zones</returns>
        public IList<WidgetZone> SupportedWidgetZones()
        {
            //limit widget to the following zones
            return new List<WidgetZone>() 
            { 
                WidgetZone.HeadHtmlTag, 
                WidgetZone.BeforeBodyEndHtmlTag
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
            controllerName = "WidgetsGoogleAnalytics";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Widgets.GoogleAnalytics.Controllers" }, { "area", null }, { "widgetId", widgetId } };
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
            controllerName = "WidgetsGoogleAnalytics";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Widgets.GoogleAnalytics.Controllers" }, { "area", null }, { "widgetId", widgetId } };
        }

        public override void Install()
        {
            var settings = new GoogleAnalyticsSettings()
            {
                GoogleId = "UA-0000000-0",
                JavaScript = "<script type=\"text/javascript\"> var gaJsHost = ((\"https:\" == document.location.protocol) ? \"https://ssl.\" : \"http://www.\"); document.write(unescape(\"%3Cscript src='\" + gaJsHost + \"google-analytics.com/ga.js' type='text/javascript'%3E%3C/script%3E\")); </script> <script type=\"text/javascript\"> try { var pageTracker = _gat._getTracker(\"UA-0000000-0\"); pageTracker._trackPageview(); } catch(err) {}</script>",
            };
            _settingService.SaveSetting(settings);

            base.Install();
        }
        
    }
}
