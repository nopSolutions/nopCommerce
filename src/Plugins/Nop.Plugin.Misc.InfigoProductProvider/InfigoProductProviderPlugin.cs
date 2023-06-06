using System.Threading.Tasks;
using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Plugins;

namespace Nop.Plugin.Misc.InfigoProductProvider;

public class InfigoProductProviderPlugin : BasePlugin, IMiscPlugin
{
    private readonly IWebHelper _webHelper;

    public InfigoProductProviderPlugin(IWebHelper webHelper)
    {
        _webHelper = webHelper;
    }

    public override string GetConfigurationPageUrl()
    {
        return $"{_webHelper.GetStoreLocation()}Admin/InfigoProductProvider/Configure";
    }
}