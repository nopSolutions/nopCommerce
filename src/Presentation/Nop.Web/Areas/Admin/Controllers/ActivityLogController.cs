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

        private readonly IActivityLogModelFactory _activityLogModelFactory;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;

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

        public virtual async Task<IActionResult> ActivityTypes()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            //prepare model
            var model = await _activityLogModelFactory.PrepareActivityLogTypeSearchModelAsync(new ActivityLogTypeSearchModel());

            return View(model);
        }

        [HttpPost, ActionName("SaveTypes")]
        public virtual async Task<IActionResult> SaveTypes(IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            //activity log
            await _customerActivityService.InsertActivityAsync("EditActivityLogTypes", await _localizationService.GetResourceAsync("ActivityLog.EditActivityLogTypes"));

            //get identifiers of selected activity types
            var selectedActivityTypesIds = form["checkbox_activity_types"]
                .SelectMany(value => value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
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

        public virtual async Task<IActionResult> ActivityLogs()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            //prepare model
            var model = await _activityLogModelFactory.PrepareActivityLogSearchModelAsync(new ActivityLogSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ListLogs(ActivityLogSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageActivityLog))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _activityLogModelFactory.PrepareActivityLogListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ActivityLogDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

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
        public virtual async Task<IActionResult> ClearAll()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            await _customerActivityService.ClearAllActivitiesAsync();

            //activity log
            await _customerActivityService.InsertActivityAsync("DeleteActivityLog", await _localizationService.GetResourceAsync("ActivityLog.DeleteActivityLog"));

            return RedirectToAction("ActivityLogs");
        }

        #endregion
    }
}