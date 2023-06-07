using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.InfigoProductProvider.Factories;
using Nop.Plugin.Misc.InfigoProductProvider.Models;
using Nop.Services.Configuration;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.InfigoProductProvider.Controllers;

[AutoValidateAntiforgeryToken]
public class InfigoProductProviderController : BasePluginController
{
    private readonly ISettingService _settingService;
    private readonly IConfigurationModelFactory _configurationModelFactory;
    public InfigoProductProviderController(ISettingService settingService, IConfigurationModelFactory configurationModelFactory)
    {
        _settingService = settingService;
        _configurationModelFactory = configurationModelFactory;
    }

    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public async Task<IActionResult> Configure()
    {
        var apiSettings = await _settingService.LoadSettingAsync<InfigoProductProviderConfiguration>();
        var model = _configurationModelFactory.PrepareConfigurationModel(null, apiSettings);

        return View(InfigoProductProviderDefaults.ConfigurationPath, model);
    }

    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
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