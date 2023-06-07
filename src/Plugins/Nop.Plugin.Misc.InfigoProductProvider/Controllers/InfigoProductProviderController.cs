using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.InfigoProductProvider.Domain;
using Nop.Plugin.Misc.InfigoProductProvider.Models;
using Nop.Plugin.Misc.InfigoProductProvider.Services;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.InfigoProductProvider.Controllers;

[AutoValidateAntiforgeryToken]
public class InfigoProductProviderController : BasePluginController
{
    private readonly IInfigoProductProviderService _infigoProductProviderService;

    public InfigoProductProviderController(IInfigoProductProviderService infigoProductProviderService)
    {
        _infigoProductProviderService = infigoProductProviderService;
    }

    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public async Task<IActionResult> Configure()
    {
        var configurationEntity = await _infigoProductProviderService.GetApiConfigurationAsync();
        var model = new ConfigurationModel
        {
            ApiUserName = configurationEntity.ApiUserName,
            ApiBase = configurationEntity.ApiBase,
            ProductListUrl = configurationEntity.ProductListUrl,
            ProductDetailsUrl = configurationEntity.ProductDetailsUrl
        };

        return View("~/Plugins/Misc.InfigoProductProvider/Views/Configure.cshtml", model);
    }

    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [HttpPost, ActionName("Configure")]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        var configuration = new InfigoProductProviderConfiguration
        {
            ApiUserName = model.ApiUserName,
            ApiBase = model.ApiBase,
            ProductListUrl = model.ProductListUrl,
            ProductDetailsUrl = model.ProductDetailsUrl
        };

        await _infigoProductProviderService.Set(configuration);
        
        return await Configure();
    }
}