using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.Babak.Domain;
using Nop.Plugin.Misc.Babak.Models;
using Nop.Plugin.Misc.Babak.Services;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.Babak.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
public class BabakController : BasePluginController
{
    private readonly IBabakItemService _babakItemService;
    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;

    public BabakController(IBabakItemService babakItemService,
        ILocalizationService localizationService,
        INotificationService notificationService)
    {
        _babakItemService = babakItemService;
        _localizationService = localizationService;
        _notificationService = notificationService;
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public IActionResult Configure()
    {
        var searchModel = new BabakItemSearchModel();
        searchModel.SetGridPageSize();

        return View("~/Plugins/Misc.Babak/Views/Configuration/Configure.cshtml", searchModel);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> List(BabakItemSearchModel searchModel)
    {
        var items = await _babakItemService.GetAllAsync(pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        var model = await new BabakItemListModel().PrepareToGridAsync(searchModel, items, () =>
        {
            return items.Select(item => new BabakItemModel
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                IsActive = item.IsActive
            });
        });

        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public IActionResult Create()
    {
        var model = new BabakItemModel
        {
            IsActive = true
        };

        return View("~/Plugins/Misc.Babak/Views/Configuration/Create.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> Create(BabakItemModel model)
    {
        if (!ModelState.IsValid)
            return View("~/Plugins/Misc.Babak/Views/Configuration/Create.cshtml", model);

        var item = new BabakItem
        {
            Name = model.Name,
            Description = model.Description,
            IsActive = model.IsActive
        };

        await _babakItemService.InsertAsync(item);
        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Nop.Plugin.Misc.Babak.Created"));

        return RedirectToAction("Configure");
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _babakItemService.GetByIdAsync(id);
        if (item == null)
            return RedirectToAction("Configure");

        var model = new BabakItemModel
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            IsActive = item.IsActive
        };

        return View("~/Plugins/Misc.Babak/Views/Configuration/Edit.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> Edit(BabakItemModel model)
    {
        var item = await _babakItemService.GetByIdAsync(model.Id);
        if (item == null)
            return RedirectToAction("Configure");

        if (!ModelState.IsValid)
            return View("~/Plugins/Misc.Babak/Views/Configuration/Edit.cshtml", model);

        item.Name = model.Name;
        item.Description = model.Description;
        item.IsActive = model.IsActive;

        await _babakItemService.UpdateAsync(item);
        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Nop.Plugin.Misc.Babak.Updated"));

        return RedirectToAction("Configure");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _babakItemService.GetByIdAsync(id);
        if (item == null)
            return RedirectToAction("Configure");

        await _babakItemService.DeleteAsync(item);
        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Nop.Plugin.Misc.Babak.Deleted"));

        return RedirectToAction("Configure");
    }
}
