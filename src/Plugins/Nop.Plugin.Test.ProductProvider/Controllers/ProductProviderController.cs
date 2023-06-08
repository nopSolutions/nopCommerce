using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Test.ProductProvider.Models;
using Nop.Plugin.Test.ProductProvider.Services;
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
    private readonly IProductService _productService;

    public ProductProviderController(ISettingService settingService, IProductService productService)
    {
        _settingService = settingService;
        _productService = productService;
    }

    public async Task<IActionResult> Configure()
    {
        var model = new ConfigurationModel();
        var settings = await _settingService.LoadSettingAsync<ProductProviderSettings>();

        if (settings != null)
        {
            model = new ConfigurationModel()
            {
                BaseUrl = settings.BaseUrl,
                AccessToken = settings.ApiKey,
                GetProductsIdsEndpoint = settings.GetProductsIdsEndpoint,
                GetProductByIdEndpoint = settings.GetProductByIdEndpoint
            };
        }

        return View("~/Plugins/Test.ProductProvider/Views/Configure.cshtml", model);
    }
    
    [HttpPost]
    public async Task<IActionResult> Save(ConfigurationModel model)
    {
        var settings = new ProductProviderSettings()
        {
            BaseUrl = model.BaseUrl,
            ApiKey = model.AccessToken,
            GetProductsIdsEndpoint = model.GetProductsIdsEndpoint,
            GetProductByIdEndpoint = model.GetProductByIdEndpoint
        };

        await _settingService.SaveSettingAsync(settings);

        return await Configure();
    }
}