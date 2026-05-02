using Microsoft.AspNetCore.Mvc;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Logging;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using ILogger = Nop.Services.Logging.ILogger;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class LogController : BaseAdminController
{
    #region Fields

    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILogger _logger;
    protected readonly ILogModelFactory _logModelFactory;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;

    #endregion

    #region Ctor

    public LogController(ICustomerActivityService customerActivityService,
        ILocalizationService localizationService,
        ILogger logger,
        ILogModelFactory logModelFactory,
        INotificationService notificationService,
        IPermissionService permissionService)
    {
        _customerActivityService = customerActivityService;
        _localizationService = localizationService;
        _logger = logger;
        _logModelFactory = logModelFactory;
        _notificationService = notificationService;
        _permissionService = permissionService;
    }

    #endregion

    #region Methods

    public virtual IActionResult Index()
    {
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.System.MANAGE_SYSTEM_LOG)]
    public virtual async Task<IActionResult> List()
    {
        //prepare model
        var model = await _logModelFactory.PrepareLogSearchModelAsync(new LogSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_SYSTEM_LOG)]
    public virtual async Task<IActionResult> LogList(LogSearchModel searchModel)
    {
        //prepare model
        var model = await _logModelFactory.PrepareLogListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost, ActionName("List")]
    [FormValueRequired("clearall")]
    [CheckPermission(StandardPermission.System.MANAGE_SYSTEM_LOG)]
    public virtual async Task<IActionResult> ClearAll()
    {
        await _logger.ClearLogAsync();

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteSystemLog", await _localizationService.GetResourceAsync("ActivityLog.DeleteSystemLog"));

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.System.Log.Cleared"));

        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.System.MANAGE_SYSTEM_LOG)]
    public virtual async Task<IActionResult> View(int id)
    {
        //try to get a log with the specified id
        var log = await _logger.GetLogByIdAsync(id);
        if (log == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _logModelFactory.PrepareLogModelAsync(null, log);

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_SYSTEM_LOG)]
    public virtual async Task<IActionResult> Delete(int id)
    {
        //try to get a log with the specified id
        var log = await _logger.GetLogByIdAsync(id);
        if (log == null)
            return RedirectToAction("List");

        await _logger.DeleteLogAsync(log);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteSystemLog", await _localizationService.GetResourceAsync("ActivityLog.DeleteSystemLog"), log);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.System.Log.Deleted"));

        return RedirectToAction("List");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_SYSTEM_LOG)]
    public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
    {
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        await _logger.DeleteLogsAsync((await _logger.GetLogByIdsAsync([.. selectedIds])).ToList());

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteSystemLog", await _localizationService.GetResourceAsync("ActivityLog.DeleteSystemLog"));

        return Json(new { Result = true });
    }

    #endregion
}