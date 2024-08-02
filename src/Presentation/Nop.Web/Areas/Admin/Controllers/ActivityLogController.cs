using Microsoft.AspNetCore.Mvc;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Logging;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class ActivityLogController : BaseAdminController
{
    #region Fields

    protected readonly IActivityLogModelFactory _activityLogModelFactory;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IPermissionService _permissionService;
    protected readonly INotificationService _notificationService;
    private static readonly char[] _separator = [','];

    #endregion

    #region Ctor

    public ActivityLogController(IActivityLogModelFactory activityLogModelFactory,
        ICustomerActivityService customerActivityService,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IPermissionService permissionService)
    {
        _activityLogModelFactory = activityLogModelFactory;
        _customerActivityService = customerActivityService;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _permissionService = permissionService;
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Customers.ACTIVITY_LOG_VIEW)]
    public virtual async Task<IActionResult> ActivityTypes()
    {
        //prepare model
        var model = await _activityLogModelFactory.PrepareActivityLogTypeSearchModelAsync(new ActivityLogTypeSearchModel());

        return View(model);
    }

    [HttpPost, ActionName("SaveTypes")]
    [CheckPermission(StandardPermission.Customers.ACTIVITY_LOG_MANAGE_TYPES)]
    public virtual async Task<IActionResult> SaveTypes(IFormCollection form)
    {
        //activity log
        await _customerActivityService.InsertActivityAsync("EditActivityLogTypes", await _localizationService.GetResourceAsync("ActivityLog.EditActivityLogTypes"));

        //get identifiers of selected activity types
        var selectedActivityTypesIds = form["checkbox_activity_types"]
            .SelectMany(value => value.Split(_separator, StringSplitOptions.RemoveEmptyEntries))
            .Select(idString => int.TryParse(idString, out var id) ? id : 0)
            .Distinct().ToList();

        //update activity types
        var activityTypes = await _customerActivityService.GetAllActivityTypesAsync();
        foreach (var activityType in activityTypes)
        {
            activityType.Enabled = selectedActivityTypesIds.Contains(activityType.Id);
            await _customerActivityService.UpdateActivityTypeAsync(activityType);
        }

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Customers.ActivityLogType.Updated"));

        return RedirectToAction("ActivityTypes");
    }

    [CheckPermission(StandardPermission.Customers.ACTIVITY_LOG_VIEW)]
    public virtual async Task<IActionResult> ActivityLogs()
    {
        //prepare model
        var model = await _activityLogModelFactory.PrepareActivityLogSearchModelAsync(new ActivityLogSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.ACTIVITY_LOG_VIEW)]
    public virtual async Task<IActionResult> ListLogs(ActivityLogSearchModel searchModel)
    {
        //prepare model
        var model = await _activityLogModelFactory.PrepareActivityLogListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.ACTIVITY_LOG_DELETE)]
    public virtual async Task<IActionResult> ActivityLogDelete(int id)
    {
        //try to get a log item with the specified id
        var logItem = await _customerActivityService.GetActivityByIdAsync(id)
            ?? throw new ArgumentException("No activity log found with the specified id", nameof(id));

        await _customerActivityService.DeleteActivityAsync(logItem);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteActivityLog",
            await _localizationService.GetResourceAsync("ActivityLog.DeleteActivityLog"), logItem);

        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.ACTIVITY_LOG_DELETE)]
    public virtual async Task<IActionResult> ClearAll()
    {
        await _customerActivityService.ClearAllActivitiesAsync();

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteActivityLog", await _localizationService.GetResourceAsync("ActivityLog.DeleteActivityLog"));

        return RedirectToAction("ActivityLogs");
    }

    #endregion
}