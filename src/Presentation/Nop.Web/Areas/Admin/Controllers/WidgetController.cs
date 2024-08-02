using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Cms;
using Nop.Core.Events;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Cms;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class WidgetController : BaseAdminController
{
    #region Fields

    protected readonly IEventPublisher _eventPublisher;
    protected readonly IPermissionService _permissionService;
    protected readonly ISettingService _settingService;
    protected readonly IWidgetModelFactory _widgetModelFactory;
    protected readonly IWidgetPluginManager _widgetPluginManager;
    protected readonly WidgetSettings _widgetSettings;

    #endregion

    #region Ctor

    public WidgetController(IEventPublisher eventPublisher,
        IPermissionService permissionService,
        ISettingService settingService,
        IWidgetModelFactory widgetModelFactory,
        IWidgetPluginManager widgetPluginManager,
        WidgetSettings widgetSettings)
    {
        _eventPublisher = eventPublisher;
        _permissionService = permissionService;
        _settingService = settingService;
        _widgetModelFactory = widgetModelFactory;
        _widgetPluginManager = widgetPluginManager;
        _widgetSettings = widgetSettings;
    }

    #endregion

    #region Methods

    public virtual IActionResult Index()
    {
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public virtual async Task<IActionResult> List()
    {
        //prepare model
        var model = await _widgetModelFactory.PrepareWidgetSearchModelAsync(new WidgetSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public virtual async Task<IActionResult> List(WidgetSearchModel searchModel)
    {
        //prepare model
        var model = await _widgetModelFactory.PrepareWidgetListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public virtual async Task<IActionResult> WidgetUpdate(WidgetModel model)
    {
        var widget = await _widgetPluginManager.LoadPluginBySystemNameAsync(model.SystemName);
        if (_widgetPluginManager.IsPluginActive(widget, _widgetSettings.ActiveWidgetSystemNames))
        {
            if (!model.IsActive)
            {
                //mark as disabled
                _widgetSettings.ActiveWidgetSystemNames.Remove(widget.PluginDescriptor.SystemName);
                await _settingService.SaveSettingAsync(_widgetSettings);
            }
        }
        else
        {
            if (model.IsActive)
            {
                //mark as active
                _widgetSettings.ActiveWidgetSystemNames.Add(widget.PluginDescriptor.SystemName);
                await _settingService.SaveSettingAsync(_widgetSettings);
            }
        }

        var pluginDescriptor = widget.PluginDescriptor;

        //display order
        pluginDescriptor.DisplayOrder = model.DisplayOrder;

        //update the description file
        pluginDescriptor.Save();

        //raise event
        await _eventPublisher.PublishAsync(new PluginUpdatedEvent(pluginDescriptor));

        return new NullJsonResult();
    }

    #endregion
}