using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Plugin.Widgets.Deals.Components;
using Nop.Services.Cms;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.Deals;

public class TireDealsPlugin : BasePlugin, IWidgetPlugin
{
    private readonly IWebHelper _webHelper;
    private readonly ILocalizationService _localizationService;

    public TireDealsPlugin(IWebHelper webHelper, ILocalizationService localizationService)
    {
        _webHelper = webHelper;
        _localizationService = localizationService;
    }

    public override string GetConfigurationPageUrl()
    {
        return $"{_webHelper.GetStoreLocation()}Admin/TireDeals/List";
    }
    
    public Task<IList<string>> GetWidgetZonesAsync()
    {
        return Task.FromResult<IList<string>>(new List<string> { PublicWidgetZones.HomepageBeforeNews });
    }

    public Type GetWidgetViewComponent(string widgetZone)
    {
        return typeof(TireDealsViewComponent);
    }

    public bool HideInWidgetList { get; }

    public override async Task InstallAsync()
    {
        //List
        await _localizationService.AddOrUpdateLocaleResourceAsync("Admin.Plugins.TireDeal.List.IsActive", "Is active");
        await _localizationService.AddOrUpdateLocaleResourceAsync("Admin.Plugins.TireDeal.List.Title", "Title");
        await _localizationService.AddOrUpdateLocaleResourceAsync("Admin.Plugins.TireDeal.List.LongDescription", "Long Description");
        await _localizationService.AddOrUpdateLocaleResourceAsync("Admin.Plugins.TireDeal.List.ShortDescription", "Short Description");
        
        await _localizationService.AddOrUpdateLocaleResourceAsync("Admin.Plugins.TireDeal.List.IsActive.Hint", "Search by active");
        await _localizationService.AddOrUpdateLocaleResourceAsync("Admin.Plugins.TireDeal.List.Title.Hint", "Search by Title");
        await _localizationService.AddOrUpdateLocaleResourceAsync("Admin.Plugins.TireDeal.List.LongDescription.Hint",  "Search by Long Description");
        await _localizationService.AddOrUpdateLocaleResourceAsync("Admin.Plugins.TireDeal.List.ShortDescription.Hint", "Search by Short Description");

        //Edit
        await _localizationService.AddOrUpdateLocaleResourceAsync("Admin.Plugins.TireDeal.Edit.IsActive", "Is active");
        await _localizationService.AddOrUpdateLocaleResourceAsync("Admin.Plugins.TireDeal.Edit.Title", "Title");
        await _localizationService.AddOrUpdateLocaleResourceAsync("Admin.Plugins.TireDeal.Edit.LongDescription", "Long Description");
        await _localizationService.AddOrUpdateLocaleResourceAsync("Admin.Plugins.TireDeal.Edit.ShortDescription", "Short Description");

        await _localizationService.AddOrUpdateLocaleResourceAsync("Admin.Plugins.TireDeal.Edit.IsActive.Hint", "Edit is active");
        await _localizationService.AddOrUpdateLocaleResourceAsync("Admin.Plugins.TireDeal.Edit.Title.Hint", "Edit Title");
        await _localizationService.AddOrUpdateLocaleResourceAsync("Admin.Plugins.TireDeal.Edit.LongDescription.Hint", "Edit Long Description");
        await _localizationService.AddOrUpdateLocaleResourceAsync("Admin.Plugins.TireDeal.Edit.ShortDescription.Hint", "Edit Short Description");

        await base.InstallAsync();
    }

    public override async Task UninstallAsync()
    {
        await _localizationService.DeleteLocaleResourcesAsync(new string[]
        {
            //List
            "Admin.Plugins.TireDeal.List.IsActive",
            "Admin.Plugins.TireDeal.List.Title", 
            "Admin.Plugins.TireDeal.List.LongDescription", 
            "Admin.Plugins.TireDeal.List.ShortDescription", 
            "Admin.Plugins.TireDeal.List.IsActive.Hint",
            "Admin.Plugins.TireDeal.List.Title.Hint",
            "Admin.Plugins.TireDeal.List.LongDescription.Hint", 
            "Admin.Plugins.TireDeal.List.ShortDescription.Hint",
            
            //Edit
            "Admin.Plugins.TireDeal.Edit.IsActive",
            "Admin.Plugins.TireDeal.Edit.Title", 
            "Admin.Plugins.TireDeal.Edit.LongDescription", 
            "Admin.Plugins.TireDeal.Edit.ShortDescription", 
            "Admin.Plugins.TireDeal.Edit.IsActive.Hint",
            "Admin.Plugins.TireDeal.Edit.Title.Hint",
            "Admin.Plugins.TireDeal.Edit.LongDescription.Hint", 
            "Admin.Plugins.TireDeal.Edit.ShortDescription.Hint",
        });

        await base.UninstallAsync();
    }
}