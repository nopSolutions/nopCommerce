using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Widgets.Deals.Models;
using Nop.Plugin.Widgets.Deals.Services;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.DataTables;
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

    public async Task<IActionResult> List()
    {
        var model = new TireDealSearchModel() { AvailablePageSizes = "10, 20, 30" };

        return View("~/Plugins/Widgets.TireDeals/Views/List.cshtml", model);
    }
    
    [HttpPost]
    public async Task<IActionResult> GetTireDeals()
    {
        try
        {
            return Ok(new DataTablesModel { Data = await _tireDealService.GetAllAsync() });
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(TireDealCreateModel model)
    {
        await _tireDealService.InsertAsync(model);

        return await List();
    }

    public async Task<IActionResult> Update()
    {
        var model = new TireDealUpdateModel();

        return View("~/Plugins/Widgets.TireDeals/Views/Edit.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> Update(TireDealUpdateModel model)
    {
        await _tireDealService.UpdateAsync(model);

        return await List();
    }

    public async Task<IActionResult> Create()
    {
        var model = new TireDealCreateModel();
        
        return View("~/Plugins/Widgets.TireDeals/Views/Create.cshtml", model);
    }
}