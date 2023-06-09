using System;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;

namespace Nop.Plugin.Test.ProductProvider;

public class ProductProviderPlugin : BasePlugin, IMiscPlugin
{
    protected readonly IWebHelper _webHelper;
    private readonly ISettingService _settingService;
    private readonly IScheduleTaskService _scheduleTaskService;

    public ProductProviderPlugin(IWebHelper webHelper, ISettingService settingService, IScheduleTaskService scheduleTaskService)
    {
        _webHelper = webHelper;
        _settingService = settingService;
        _scheduleTaskService = scheduleTaskService;
    }

    public override string GetConfigurationPageUrl()
    {
        return $"{_webHelper.GetStoreLocation()}Admin/ProductProvider/Configure";
    }

    public override async Task InstallAsync()
    {
        var settings = new ProductProviderSettings()
        {
            BaseUrl = "https://c2318.qa.infigosoftware.rocks",
            ApiKey = "NDViOTIyNzUtZGUxYi00MTk4LWI4YmUtMTkzNmRmNWQ0ZTc1",
            ProductListEndpoint = "services/api/catalog/productlist",
            ProductDetailEndpoint = "services/api/catalog/ProductDetails",
            ApiKeyType = "Basic"
        };

        await _settingService.SaveSettingAsync(settings);

        //install synchronization task
        if (await _scheduleTaskService.GetTaskByTypeAsync(ProductProviderDefaults.SynchronizationTask) == null)
        {
            await _scheduleTaskService.InsertTaskAsync(new ScheduleTask
            {
                Enabled = true,
                LastEnabledUtc = DateTime.UtcNow,
                Seconds = ProductProviderDefaults.DefaultSynchronizationPeriod,
                Name = ProductProviderDefaults.SynchronizationTaskName,
                Type = ProductProviderDefaults.SynchronizationTask,
            });
        }
        
        await base.InstallAsync();
    }
}