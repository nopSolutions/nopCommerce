using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;
using Nop.Web.Framework;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Misc.InfigoProductProvider;

public class InfigoProductProviderPlugin : BasePlugin, IMiscPlugin, IAdminMenuPlugin
{
    private readonly IWebHelper _webHelper;
    private readonly ISettingService _settingService;
    private readonly ILocalizationService _localizationService;
    private readonly IScheduleTaskService _scheduleTaskService;
    private readonly ISpecificationAttributeService _specificationAttributeService;
    private readonly ILogger<InfigoProductProviderPlugin> _logger;

    public InfigoProductProviderPlugin(IWebHelper webHelper, ISettingService settingService,
        ILocalizationService localizationService, IScheduleTaskService scheduleTaskService,
        ISpecificationAttributeService specificationAttributeService, ILogger<InfigoProductProviderPlugin> logger)
    {
        _webHelper = webHelper;
        _settingService = settingService;
        _localizationService = localizationService;
        _scheduleTaskService = scheduleTaskService;
        _specificationAttributeService = specificationAttributeService;
        _logger = logger;
    }

    public override async Task InstallAsync()
    {
        _logger.LogInformation("Inserting Api settings");

        var apiSettings = new InfigoProductProviderConfiguration
        {
            ApiUserName = "", ApiBase = "", ProductListUrl = "", ProductDetailsUrl = ""
        };

        await _settingService.SaveSettingAsync(apiSettings);

        _logger.LogInformation("Inserting localization resources");

        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.Misc.InfigoProductProvider.ApiUserName"] = "Api user name",
            ["Plugins.Misc.InfigoProductProvider.ApiBase"] = "Api base",
            ["Plugins.Misc.InfigoProductProvider.ProductListUrl"] = "Product list url",
            ["Plugins.Misc.InfigoProductProvider.ProductDetailsUrl"] = "Product details url",
            ["Plugins.Misc.InfigoProductProvider.ExternalId"] = "External Id",
        });

        _logger.LogInformation("Inserting scheduled task.");

        await _scheduleTaskService.InsertTaskAsync(new ScheduleTask
        {
            Name = "Get products from Api",
            Seconds = 3600,
            Type =
                "Nop.Plugin.Misc.InfigoProductProvider.Background.GetProductsFromApiTask, Nop.Plugin.Misc.InfigoProductProvider",
            Enabled = true,
            LastEnabledUtc = DateTime.UtcNow,
            StopOnError = false
        });

        _logger.LogInformation("Inserting specification attribute for external Id");

        await _specificationAttributeService.InsertSpecificationAttributeAsync(new SpecificationAttribute
        {
            Name = InfigoProductProviderDefaults.SpecificationAttributeForExternalId
        });

        await base.InstallAsync();
    }

    public override async Task UninstallAsync()
    {
        var scheduleTask = await _scheduleTaskService.GetTaskByTypeAsync(
            "Nop.Plugin.Misc.InfigoProductProvider.Background.GetProductsFromApiTask, Nop.Plugin.Misc.InfigoProductProvider");
        if (scheduleTask != null)
        {
            _logger.LogInformation("Deleting scheduled task");

            await _scheduleTaskService.DeleteTaskAsync(scheduleTask);
        }

        var specificationAttribute =
            (await _specificationAttributeService.GetSpecificationAttributesAsync()).FirstOrDefault(sa =>
                sa.Name == InfigoProductProviderDefaults.SpecificationAttributeForExternalId);
        if (specificationAttribute != null)
        {
            _logger.LogInformation("Deleting specification attribute for external Id");

            await _specificationAttributeService.DeleteSpecificationAttributeAsync(specificationAttribute);
        }
    }

    public override string GetConfigurationPageUrl()
    {
        return $"{_webHelper.GetStoreLocation()}Admin/InfigoProductProvider/Configure";
    }

    public Task ManageSiteMapAsync(SiteMapNode rootNode)
    {
        var menuItem = new SiteMapNode
        {
            SystemName = InfigoProductProviderDefaults.SystemName,
            Title = "Search by external Id",
            ControllerName = "InfigoProductProvider",
            ActionName = "SearchByExternalId",
            IconClass = "far fa-dot-circle",
            Visible = true,
            RouteValues = new RouteValueDictionary() { { "area", AreaNames.Admin } },
        };
        var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Third party plugins");
        if (pluginNode != null)
            pluginNode.ChildNodes.Add(menuItem);
        else
            rootNode.ChildNodes.Add(menuItem);

        return Task.CompletedTask;
    }
}