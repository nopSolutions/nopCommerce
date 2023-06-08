using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Test.ProductProvider.Models;
using Nop.Services.Configuration;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Test.ProductProvider.Controllers;

[AuthorizeAdmin] //confirms access to the admin panel
[Area(AreaNames.Admin)] //specifies the area containing a controller or action
public class ProductProviderController : BasePluginController
{
    private readonly ISettingService _settingService;

    public ProductProviderController(ISettingService settingService)
    {
        _settingService = settingService;
    }

    public async Task<IActionResult> Configure()
    {
        var model = new ConfigurationModel();
        model.BaseUrl = await _settingService.GetSettingByKeyAsync<string>(model.BaseUrlKey);
        model.GetProductsIdsEndpoint = await _settingService.GetSettingByKeyAsync<string>(model.GetProductsIdsEndpointKey);
        model.GetProductByIdEndpoint = await _settingService.GetSettingByKeyAsync<string>(model.GetProductByIdEndpointKey);

        return View("~/Plugins/Test.ProductProvider/Views/Configure.cshtml", model);
    }
    
    [HttpPost]
    public async Task<IActionResult> Save(ConfigurationModel model)
    {
        await _settingService.SetSettingAsync(model.BaseUrlKey, model.BaseUrl);
        await _settingService.SetSettingAsync(model.GetProductsIdsEndpointKey, model.GetProductsIdsEndpoint);
        await _settingService.SetSettingAsync(model.GetProductByIdEndpointKey, model.GetProductByIdEndpoint);

        return await Configure();
    }
}