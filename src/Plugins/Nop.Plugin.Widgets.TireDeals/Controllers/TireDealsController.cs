using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Widgets.Deals.Mapping.Factories;
using Nop.Plugin.Widgets.Deals.Models;
using Nop.Plugin.Widgets.Deals.Services;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Widgets.Deals.Controllers;

[AuthorizeAdmin] //confirms access to the admin panel
[Area(AreaNames.Admin)] //specifies the area containing a controller or action
public class TireDealsController : BasePluginController
{
    private readonly ITireDealService _tireDealService;
    private readonly ITireDealModelFactory _tireDealModelFactory;
    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;

    public TireDealsController(ITireDealService tireDealService, ITireDealModelFactory tireDealModelFactory, ILocalizationService localizationService, INotificationService notificationService)
    {
        _tireDealService = tireDealService;
        _tireDealModelFactory = tireDealModelFactory;
        _localizationService = localizationService;
        _notificationService = notificationService;
    }

    public async Task<IActionResult> List()
    {
        var model = new TireDealSearchModel() { AvailablePageSizes = "10, 20, 30" };

        return View("~/Plugins/Widgets.TireDeals/Views/List.cshtml", model);
    }
    
    [HttpPost]
    public virtual async Task<IActionResult> GetTireDeals(TireDealSearchModel searchModel)
    {
        //prepare model
        var model = await _tireDealModelFactory.PrepareTireDealListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(TireDealCreateModel model)
    {
        await _tireDealService.InsertAsync(model);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await List();
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await _tireDealService.GetByIdAsync(id);
        
        return View("~/Plugins/Widgets.TireDeals/Views/Edit.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(TireDealUpdateModel model)
    {
        await _tireDealService.UpdateAsync(model);

        return RedirectToAction("List");
    }

    public async Task<IActionResult> Create()
    {
        var model = new TireDealModel();
        
        return View("~/Plugins/Widgets.TireDeals/Views/Create.cshtml", model);
    }
}