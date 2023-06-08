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

    public async Task<IActionResult> Get()
    {
        var model = new ConfigurationModel();
        var endpoint = await _settingService.GetSettingByKeyAsync<string>(model.EndPointUrlKey);
        model.EndPointUrl = endpoint;
        
        return View("~/Plugins/Test.ProductProvider/Views/Configure.cshtml", model);
    }
    
    [HttpPost]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        await _settingService.SetSettingAsync(model.EndPointUrlKey, model.EndPointUrl);

        var endpoint = await _settingService.GetSettingByKeyAsync<string>(model.EndPointUrlKey);
        var a = 1; 
        
        return await Get();
    }
}