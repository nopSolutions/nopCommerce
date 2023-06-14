using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Plugin.Widgets.Deals.Components;
using Nop.Services.Cms;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.Deals;

public class DealsPlugin : BasePlugin, IWidgetPlugin
{
    public Task<IList<string>> GetWidgetZonesAsync()
    {
        return Task.FromResult<IList<string>>(new List<string> { PublicWidgetZones.HomepageBeforeNews });
    }

    public Type GetWidgetViewComponent(string widgetZone)
    {
        return typeof(DealsViewComponent);
    }

    public bool HideInWidgetList { get; }
}