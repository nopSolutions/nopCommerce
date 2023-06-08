using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Test.ProductProvider.Models;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Test.ProductProvider.Controllers;

[AuthorizeAdmin] //confirms access to the admin panel
[Area(AreaNames.Admin)] //specifies the area containing a controller or action
public class ProductProviderController : BasePluginController
{
    public async Task<IActionResult> Get()
    {
        var model = new ConfigurationModel();
        
        return View("~/Plugins/Test.ProductProvider/Views/Configure.cshtml", model);
    }
    
    [HttpPost]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        Console.Write("Configure called");
        
        return await Get();
    }
}