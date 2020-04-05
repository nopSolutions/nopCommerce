using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.DateTimeFormat
{
    public class DateTimeFormatPlugin : BasePlugin, IWidgetPlugin
    {
        #region Fields
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly ISettingService _settingService;
        #endregion

        #region Ctor
        public DateTimeFormatPlugin(ILocalizationService localizationService,
            IWebHelper webHelper,
            ISettingService settingService)
        {
            _localizationService = localizationService;
            _webHelper = webHelper;
            _settingService = settingService;
        }
        #endregion

        #region Methods
        public IList<string> GetWidgetZones()
        {
            return new List<string> { PublicWidgetZones.HomepageBeforeCategories };
        }

        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/WidgetsDateTimeFormat/Configure";
        }

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "WidgetsDateTimeFormat";
        }
        #endregion

        public override void Install()
        {
            var settings = new DateTimeFormatSettings
            {
                FormatString = "yyyy-MM-dd HH:mm:ss"
            };
            _settingService.SaveSetting(settings);

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DateTimeFormat.FormatString", "Format");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DateTimeFormat.FormatString.Hint", "Enter Custom Date Time Format.");

            base.Install();
        }

        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<DateTimeFormatSettings>();

            //locales
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DateTimeFormat.FormatString");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DateTimeFormat.FormatString.Hint");

            base.Uninstall();
        }

    public bool HideInWidgetList => false;

}
}
