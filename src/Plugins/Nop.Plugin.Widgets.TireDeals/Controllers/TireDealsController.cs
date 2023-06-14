using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Widgets.Deals.Domain;
using Nop.Plugin.Widgets.Deals.Models;
using Nop.Plugin.Widgets.Deals.Services;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Widgets.Deals.Controllers;

[AuthorizeAdmin] //confirms access to the admin panel
[Area(AreaNames.Admin)] //specifies the area containing a controller or action
public class TireDealsController : BasePluginController
{
    private readonly ITireDealService _tireDealService;

    public TireDealsController(ITireDealService tireDealService)
    {
        _tireDealService = tireDealService;
    }

    public async Task<IActionResult> Configure()
    {
        var model = new TireDealSearchModel();

        return View("~/Plugins/Widgets.TireDeals/Views/Configure.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(TireDealCreateModel model)
    {
        await _tireDealService.InsertAsync(model);

        return await Configure();
    }

    public async Task<IActionResult> Create()
    {
        var model = new TireDealCreateModel();
        
        return View("~/Plugins/Widgets.TireDeals/Views/Create.cshtml", model);
    }
}