using Nop.Core;
using Nop.Services.Cms;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Plugin.Demo.Database
{
    internal class DemoDatabase : BasePlugin, IWidgetPlugin
    {
        public bool HideInWidgetList => false;
        private readonly IWebHelper _webHelper;

        public DemoDatabase(IWebHelper webHelper)
        {
            _webHelper = webHelper;
        }
        /// <summary>
        /// Gets a name of a view component for displaying widget
        /// </summary>
        /// <param name="widgetZone">Name of the widget zone</param>
        /// <returns>View component name</returns>
        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "Room";
        }

        /// <summary>
        /// Gets a configuration page URL. This url used at Admin local Plugin list. Your plugin name have 2 button edit and configure. so when you click on configure button you will redirect on this url.
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/MultiLevelMenu/Configure";
        }
        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> { PublicWidgetZones.ProductDetailsTop });
        }
    }
}