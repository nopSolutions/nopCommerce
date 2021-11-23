using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Logging;
using Nop.Web.Framework.Controllers;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class LogController : BaseAdminController
    {
        #region Fields

        protected ICustomerActivityService CustomerActivityService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected ILogger Logger { get; }
        protected ILogModelFactory LogModelFactory { get; }
        protected INotificationService NotificationService { get; }
        protected IPermissionService PermissionService { get; }

        #endregion

        #region Ctor

        public LogController(ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            ILogger logger,
            ILogModelFactory logModelFactory,
            INotificationService notificationService,
            IPermissionService permissionService)
        {
            CustomerActivityService = customerActivityService;
            LocalizationService = localizationService;
            Logger = logger;
            LogModelFactory = logModelFactory;
            NotificationService = notificationService;
            PermissionService = permissionService;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            //prepare model
            var model = await LogModelFactory.PrepareLogSearchModelAsync(new LogSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> LogList(LogSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSystemLog))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await LogModelFactory.PrepareLogListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("clearall")]
        public virtual async Task<IActionResult> ClearAll()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            await Logger.ClearLogAsync();

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteSystemLog", await LocalizationService.GetResourceAsync("ActivityLog.DeleteSystemLog"));

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.System.Log.Cleared"));

            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> View(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            //try to get a log with the specified id
            var log = await Logger.GetLogByIdAsync(id);
            if (log == null)
                return RedirectToAction("List");

            //prepare model
            var model = await LogModelFactory.PrepareLogModelAsync(null, log);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            //try to get a log with the specified id
            var log = await Logger.GetLogByIdAsync(id);
            if (log == null)
                return RedirectToAction("List");

            await Logger.DeleteLogAsync(log);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteSystemLog", await LocalizationService.GetResourceAsync("ActivityLog.DeleteSystemLog"), log);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.System.Log.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            if (selectedIds == null || selectedIds.Count == 0)
                return NoContent();

            await Logger.DeleteLogsAsync((await Logger.GetLogByIdsAsync(selectedIds.ToArray())).ToList());

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteSystemLog", await LocalizationService.GetResourceAsync("ActivityLog.DeleteSystemLog"));

            return Json(new { Result = true });
        }

        #endregion
    }
}