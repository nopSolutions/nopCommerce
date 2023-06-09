using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Plugin.Test.ProductProvider.Models;
using Nop.Plugin.Test.ProductProvider.Services;
using Nop.Services.Configuration;
using Nop.Services.ScheduleTasks;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using WebMarkupMin.Core.Loggers;

namespace Nop.Plugin.Test.ProductProvider.Controllers;

[AuthorizeAdmin] //confirms access to the admin panel
[Area(AreaNames.Admin)] //specifies the area containing a controller or action
public class ProductProviderController : BasePluginController
{
    private readonly ISettingService _settingService;
    private readonly IProductService _productService;
    private readonly IScheduleTaskService _scheduleTaskService;

    public ProductProviderController(ISettingService settingService, IProductService productService, IScheduleTaskService scheduleTaskService)
    {
        _settingService = settingService;
        _productService = productService;
        _scheduleTaskService = scheduleTaskService;
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
                ApiKey = settings.ApiKey,
                ProductListEndpoint = settings.ProductListEndpoint,
                ProductDetailsEndpoint = settings.ProductDetailEndpoint,
                ApiKeyType = settings.ApiKeyType
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
            ApiKey = model.ApiKey,
            ProductListEndpoint = model.ProductListEndpoint,
            ProductDetailEndpoint = model.ProductDetailsEndpoint,
            ApiKeyType = model.ApiKeyType
        };

        await _settingService.SaveSettingAsync(settings);

        return await Configure();
    }
}