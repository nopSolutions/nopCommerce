using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace AbcWarehouse.Plugin.Misc.SLI
{
    public class SLIPlugin : BasePlugin, IMiscPlugin
    {
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;

        public SLIPlugin(
            ILocalizationService localizationService,
            ISettingService settingService)
        {
            _localizationService = localizationService;
            _settingService = settingService;
        }
    }
}
