using System.Threading.Tasks;
using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Plugins;

namespace Nop.Plugin.Test.ProductProvider;

public class ProductProviderPlugin : BasePlugin, IMiscPlugin
{
    protected readonly IWebHelper _webHelper;
    private readonly ISettingService _settingService;

    public ProductProviderPlugin(IWebHelper webHelper, ISettingService settingService)
    {
        _webHelper = webHelper;
        _settingService = settingService;
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
        await base.InstallAsync();
    }
}