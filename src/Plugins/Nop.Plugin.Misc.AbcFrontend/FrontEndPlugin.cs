using Nop.Core.Domain;
using Nop.Services.Configuration;
using Nop.Services.Plugins;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AbcFrontend
{
    public class FrontEndPlugin : BasePlugin, IPlugin
    {
        private readonly ISettingService _settingService;
        private readonly StoreInformationSettings _storeInformationSettings;

        public FrontEndPlugin(ISettingService settingService, StoreInformationSettings storeInformationSettings)
        {
            _settingService = settingService;
            _storeInformationSettings = storeInformationSettings;
        }

        public async override Task InstallAsync()
        {
            _storeInformationSettings.DefaultStoreTheme = "Pavilion";
            await _settingService.SaveSettingAsync(_storeInformationSettings);

            await base.InstallAsync();
        }
    }

}