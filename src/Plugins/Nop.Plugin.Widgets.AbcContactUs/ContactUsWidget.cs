using Nop.Core;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Plugins;
using System.Collections.Generic;
using Task = System.Threading.Tasks.Task;

namespace Nop.Plugin.Widgets.AbcContactUs
{
    public class ContactUsWidget : BasePlugin, IWidgetPlugin
    {
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;

        public ContactUsWidget(
            ISettingService settingService,
            IWebHelper webHelper
        )
        {
            _settingService = settingService;
            _webHelper = webHelper;
        }

        public override async Task InstallAsync()
        {
            await _settingService.SaveSettingAsync(ContactUsWidgetSettings.Default());
            await base.InstallAsync();
        }

        public System.Threading.Tasks.Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> { "topic_page_after_body" });
        }

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "AbcContactUs";
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/AbcContactUs/Configure";
        }

        public bool HideInWidgetList => false;
    }
}
