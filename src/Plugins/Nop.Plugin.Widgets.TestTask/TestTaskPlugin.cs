using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Plugins;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.TestTask
{
    public class TestTaskPlugin : BasePlugin, IWidgetPlugin
    {
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;

        public TestTaskPlugin(ISettingService settingService, IWebHelper webHelper)
        {
            this._settingService = settingService;
            this._webHelper = webHelper;
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            return new List<string> { PublicWidgetZones.ProductDetailsTop};
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/WidgetsTestTask/Configure";
        }

        /// <summary>
        /// Gets a name of a view component for displaying widget
        /// </summary>
        /// <param name="widgetZone">Name of the widget zone</param>
        /// <returns>View component name</returns>
        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "WidgetsTestTask";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.TestTask.Fields.PromoMessage", "Promo Message");
            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            this.DeletePluginLocaleResource("Plugins.Widgets.TestTask.Fields.PromoMessage");
            base.Uninstall();
        }

    }
}
