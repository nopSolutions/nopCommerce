using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Plugin.Widgets.Deals.Components;
using Nop.Services.Cms;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.Deals;

public class TireDealsPlugin : BasePlugin, IWidgetPlugin
{
    private readonly IWebHelper _webHelper;

    public TireDealsPlugin(IWebHelper webHelper)
    {
        _webHelper = webHelper;
    }

    public override string GetConfigurationPageUrl()
    {
        return $"{_webHelper.GetStoreLocation()}Admin/TireDeals/Configure";
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
}