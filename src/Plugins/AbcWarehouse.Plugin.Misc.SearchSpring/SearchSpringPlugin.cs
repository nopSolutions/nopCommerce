using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Plugins;

public class SearchSpringPlugin : BasePlugin, IMiscPlugin
{
    private readonly IWebHelper _webHelper;

    public SearchSpringPlugin(
        IWebHelper webHelper
    )
    {
        _webHelper = webHelper;
    }

    public override string GetConfigurationPageUrl()
    {
        return $"{_webHelper.GetStoreLocation()}Admin/SearchSpring/Configure";
    }
}
