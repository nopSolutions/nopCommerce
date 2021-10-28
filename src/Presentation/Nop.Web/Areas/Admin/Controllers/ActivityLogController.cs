using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Logging;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class ActivityLogController : BaseAdminController
    {
        #region Fields

        protected IActivityLogModelFactory ActivityLogModelFactory { get; }
        protected ICustomerActivityService CustomerActivityService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected IPermissionService PermissionService { get; }
        protected INotificationService NotificationService { get; }

        #endregion

        #region Ctor

        public ActivityLogController(IActivityLogModelFactory activityLogModelFactory,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService)
        {
            ActivityLogModelFactory = activityLogModelFactory;
            CustomerActivityService = customerActivityService;
            LocalizationService = localizationService;
            NotificationService = notificationService;
            PermissionService = permissionService;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> ActivityTypes()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            //prepare model
            var model = await ActivityLogModelFactory.PrepareActivityLogTypeSearchModelAsync(new ActivityLogTypeSearchModel());

            return View(model);
        }

        [HttpPost, ActionName("SaveTypes")]
        public virtual async Task<IActionResult> SaveTypes(IFormCollection form)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            //activity log
            await CustomerActivityService.InsertActivityAsync("EditActivityLogTypes", await LocalizationService.GetResourceAsync("ActivityLog.EditActivityLogTypes"));

            //get identifiers of selected activity types
            var selectedActivityTypesIds = form["checkbox_activity_types"]
                .SelectMany(value => value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(idString => int.TryParse(idString, out var id) ? id : 0)
                .Distinct().ToList();

            //update activity types
            var activityTypes = await CustomerActivityService.GetAllActivityTypesAsync();
            foreach (var activityType in activityTypes)
            {
                activityType.Enabled = selectedActivityTypesIds.Contains(activityType.Id);
                await CustomerActivityService.UpdateActivityTypeAsync(activityType);
            }

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Customers.ActivityLogType.Updated"));

            return RedirectToAction("ActivityTypes");
        }

        public virtual async Task<IActionResult> ActivityLogs()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            //prepare model
            var model = await ActivityLogModelFactory.PrepareActivityLogSearchModelAsync(new ActivityLogSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ListLogs(ActivityLogSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageActivityLog))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await ActivityLogModelFactory.PrepareActivityLogListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ActivityLogDelete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            //try to get a log item with the specified id
            var logItem = await CustomerActivityService.GetActivityByIdAsync(id)
                ?? throw new ArgumentException("No activity log found with the specified id", nameof(id));

            await CustomerActivityService.DeleteActivityAsync(logItem);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteActivityLog",
                await LocalizationService.GetResourceAsync("ActivityLog.DeleteActivityLog"), logItem);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> ClearAll()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            await CustomerActivityService.ClearAllActivitiesAsync();

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteActivityLog", await LocalizationService.GetResourceAsync("ActivityLog.DeleteActivityLog"));

            return RedirectToAction("ActivityLogs");
        }

        #endregion
    }
}