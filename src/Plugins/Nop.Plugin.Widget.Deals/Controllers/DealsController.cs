using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Widget.Deals.Models;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Widget.Deals.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.Admin)]
public class DealsController : BasePluginController
{
    public async Task<IActionResult> Configure()
    {
        var model = new ConfigurationModel();
        // var settings = await _settingService.LoadSettingAsync<ProductProviderSettings>();
        //
        // if (settings != null)
        // {
        //     model = new ConfigurationModel()
        //     {
        //         BaseUrl = settings.BaseUrl,
        //         ApiKey = settings.ApiKey,
        //         ProductListEndpoint = settings.ProductListEndpoint,
        //         ProductDetailsEndpoint = settings.ProductDetailEndpoint,
        //         ApiKeyType = ProductProviderDefaults.ApiKeyType
        //     };  
        // }
        
        return View("~/Plugins/Widget.Deals/Views/Configure.cshtml", model); // (..., model)
    }
    
    // [HttpPost]
    // public async Task<IActionResult> Save(ConfigurationModel model)
    // {
    //     var settings = new ProductProviderSettings()
    //     {
    //         BaseUrl = model.BaseUrl,
    //         ApiKey = model.ApiKey,
    //         ProductListEndpoint = model.ProductListEndpoint,
    //         ProductDetailEndpoint = model.ProductDetailsEndpoint,
    //     };
    //
    //     await _settingService.SaveSettingAsync(settings);
    //
    //     return await Configure();
    // }
}