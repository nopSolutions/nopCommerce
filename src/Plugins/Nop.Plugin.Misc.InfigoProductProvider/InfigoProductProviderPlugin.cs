using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;

namespace Nop.Plugin.Misc.InfigoProductProvider;

public class InfigoProductProviderPlugin : BasePlugin, IMiscPlugin
{
    private readonly IWebHelper _webHelper;
    private readonly ISettingService _settingService;
    private readonly ILocalizationService _localizationService;
    private readonly IScheduleTaskService _scheduleTaskService;

    public InfigoProductProviderPlugin(IWebHelper webHelper, ISettingService settingService, ILocalizationService localizationService, IScheduleTaskService scheduleTaskService)
    {
        _webHelper = webHelper;
        _settingService = settingService;
        _localizationService = localizationService;
        _scheduleTaskService = scheduleTaskService;
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
        
        await _scheduleTaskService.InsertTaskAsync(new ScheduleTask
        {
            Name = "Get products from Api",
            Seconds = 30,
            Type = "Nop.Plugin.Misc.InfigoProductProvider.Background.GetProductsFromApiTask, Nop.Plugin.Misc.InfigoProductProvider",
            Enabled = true,
            LastEnabledUtc = lastEnabledUtc,
            StopOnError = false
        });
        
        await base.InstallAsync();
    }

    public override string GetConfigurationPageUrl()
    {
        return $"{_webHelper.GetStoreLocation()}Admin/InfigoProductProvider/Configure";
    }
}