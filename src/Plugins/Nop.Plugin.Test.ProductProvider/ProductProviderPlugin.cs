using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Plugins;

namespace Nop.Plugin.Test.ProductProvider;

public class ProductProviderPlugin : BasePlugin, IMiscPlugin
{
    protected readonly IWebHelper _webHelper;

    public ProductProviderPlugin(IWebHelper webHelper)
    {
        _webHelper = webHelper;
    }

    public override string GetConfigurationPageUrl()
    {
        return $"{_webHelper.GetStoreLocation()}Admin/ProductProvider/Configure";
    }
}