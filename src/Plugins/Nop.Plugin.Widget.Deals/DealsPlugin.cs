using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Services.Cms;
using Nop.Services.Plugins;

namespace Nop.Plugin.Widget.Deals;

public class DealsPlugin : BasePlugin, IWidgetPlugin
{
    protected readonly IWebHelper _webHelper;

    public DealsPlugin(IWebHelper webHelper)
    {
        _webHelper = webHelper;
    }
    
    public string GetConfigurationPageUrl()
    {
        return $"{_webHelper.GetStoreLocation()}Admin/Deals/Configure";
    }

    public bool HideInWidgetList { get; }
    public async Task<IList<string>> GetWidgetZonesAsync()
    {
        return new List<string>();
    }

    public Type GetWidgetViewComponent(string widgetZone)
    {
        throw new NotImplementedException();
    }
}