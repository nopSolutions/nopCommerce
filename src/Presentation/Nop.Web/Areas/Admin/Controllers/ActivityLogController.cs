using System;
using System.Linq;
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

        public virtual IActionResult ActivityTypes()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            //prepare model
            var model = _activityLogModelFactory.PrepareActivityLogTypeSearchModel(new ActivityLogTypeSearchModel());

            return View(model);
        }

        [HttpPost, ActionName("SaveTypes")]
        public virtual IActionResult SaveTypes(IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            //activity log
            _customerActivityService.InsertActivity("EditActivityLogTypes", _localizationService.GetResource("ActivityLog.EditActivityLogTypes"));

            //get identifiers of selected activity types
            var selectedActivityTypesIds = form["checkbox_activity_types"]
                .SelectMany(value => value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(idString => int.TryParse(idString, out var id) ? id : 0)
                .Distinct().ToList();

            //update activity types
            var activityTypes = _customerActivityService.GetAllActivityTypes();
            foreach (var activityType in activityTypes)
            {
                activityType.Enabled = selectedActivityTypesIds.Contains(activityType.Id);
                _customerActivityService.UpdateActivityType(activityType);
            }

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Customers.ActivityLogType.Updated"));

            return RedirectToAction("ActivityTypes");
        }

        public virtual IActionResult ActivityLogs()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            //prepare model
            var model = _activityLogModelFactory.PrepareActivityLogSearchModel(new ActivityLogSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ListLogs(ActivityLogSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _activityLogModelFactory.PrepareActivityLogListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult ActivityLogDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            //try to get a log item with the specified id
            var logItem = _customerActivityService.GetActivityById(id)
                ?? throw new ArgumentException("No activity log found with the specified id", nameof(id));

            _customerActivityService.DeleteActivity(logItem);

            //activity log
            _customerActivityService.InsertActivity("DeleteActivityLog",
                _localizationService.GetResource("ActivityLog.DeleteActivityLog"), logItem);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult ClearAll()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            _customerActivityService.ClearAllActivities();

            //activity log
            _customerActivityService.InsertActivity("DeleteActivityLog", _localizationService.GetResource("ActivityLog.DeleteActivityLog"));

            return RedirectToAction("ActivityLogs");
        }

        #endregion
    }
}