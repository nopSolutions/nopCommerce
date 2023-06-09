using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.InfigoProductProvider.Models;
using Nop.Plugin.Misc.InfigoProductProvider.Services;
using Nop.Services.Configuration;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.InfigoProductProvider.Controllers;

[Area(AreaNames.Admin)]
[AutoValidateAntiforgeryToken]
public class InfigoProductProviderController : BasePluginController
{
    private readonly ISettingService _settingService;
    private readonly IInfigoProductProviderService _infigoProductProviderService;

    public InfigoProductProviderController(ISettingService settingService,
        IInfigoProductProviderService infigoProductProviderService)
    {
        _settingService = settingService;
        _infigoProductProviderService = infigoProductProviderService;
    }

    [AuthorizeAdmin]
    public async Task<IActionResult> Configure()
    {
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
}