using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;

namespace Nop.Plugin.Misc.InfigoProductProvider;

public class InfigoProductProviderPlugin : BasePlugin, IMiscPlugin
{
    private readonly IWebHelper _webHelper;
    private readonly ISettingService _settingService;
    private readonly ILocalizationService _localizationService;

    public InfigoProductProviderPlugin(IWebHelper webHelper, ISettingService settingService, ILocalizationService localizationService)
    {
        _webHelper = webHelper;
        _settingService = settingService;
        _localizationService = localizationService;
    }

    public override async Task InstallAsync()
    {
        var apiSettings = new InfigoProductProviderConfiguration
        {
            ApiUserName = "", ApiBase = "", ProductListUrl = "", ProductDetailsUrl = ""
        };

        await _settingService.SaveSettingAsync(apiSettings);

        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.Misc.InfigoProductProvider.ApiUserName"] = "Api user name",
            ["Plugins.Misc.InfigoProductProvider.ApiBase"] = "Api base",
            ["Plugins.Misc.InfigoProductProvider.ProductListUrl"] = "Product list url",
            ["Plugins.Misc.InfigoProductProvider.ProductDetailsUrl"] = "Product details url"
        });
        
        await base.InstallAsync();
    }

    public override string GetConfigurationPageUrl()
    {
        return $"{_webHelper.GetStoreLocation()}Admin/InfigoProductProvider/Configure";
    }
}