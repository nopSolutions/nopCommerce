using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nop.Plugin.Misc.InfigoProductProvider.Models;
using Nop.Plugin.Misc.InfigoProductProvider.Services;
using Nop.Services.Configuration;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.InfigoProductProvider.Controllers;

[Area(AreaNames.Admin)]
[AutoValidateAntiforgeryToken]
public class InfigoProductProviderController : BasePluginController
{
    private readonly ISettingService _settingService;
    private readonly ILogger<InfigoProductProviderController> _logger;
    private readonly IInfigoProductProviderService _infigoProductProviderService;

    public InfigoProductProviderController(ISettingService settingService, ILogger<InfigoProductProviderController> logger, IInfigoProductProviderService infigoProductProviderService)
    {
        _settingService = settingService;
        _logger = logger;
        _infigoProductProviderService = infigoProductProviderService;
    }

    [AuthorizeAdmin]
    public async Task<IActionResult> Configure()
    {
        _logger.LogInformation("Entering configure page Get Action");
        
        var apiSettings = await _settingService.LoadSettingAsync<InfigoProductProviderConfiguration>();
        var model = new ConfigurationModel
        {
            ApiUserName = apiSettings.ApiUserName,
            ApiBase = apiSettings.ApiBase,
            ProductListUrl = apiSettings.ProductListUrl,
            ProductDetailsUrl = apiSettings.ProductDetailsUrl
        };

        return View(InfigoProductProviderDefaults.ConfigurationPath, model);
    }

    [AuthorizeAdmin]
    [HttpPost, ActionName("Configure")]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        _logger.LogInformation("Entering configure page Post Action");
        
        var apiSettings = new InfigoProductProviderConfiguration
        {
            ApiUserName = model.ApiUserName,
            ApiBase = model.ApiBase,
            ProductListUrl = model.ProductListUrl,
            ProductDetailsUrl = model.ProductDetailsUrl
        };

        await _settingService.SaveSettingAsync(apiSettings);

        return await Configure();
    }

    [AuthorizeAdmin]
    public IActionResult SearchByExternalId()
    {
        return View(InfigoProductProviderDefaults.SearchByExternalIdPath, new ExternalIdSearchModel());
    }

    [AuthorizeAdmin]
    [HttpPost, ActionName("SearchByExternalId")]
    public async Task<IActionResult> SearchByExternalId(ExternalIdSearchModel model)
    {
        var product = await _infigoProductProviderService.GetProductByExternalId(model.ExternalId);
        var searchModel = new ProductSearchModel { SearchProductName = product.Name };
        return RedirectToAction("ProductList", "Product", searchModel);
    }
}