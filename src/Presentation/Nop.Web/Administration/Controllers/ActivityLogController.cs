using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Models.Logging;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public partial class ActivityLogController : BaseNopController
    {
        #region Fields

        private readonly ICustomerActivityService _customerActivityService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;

        #endregion Fields

        #region Constructors

        public ActivityLogController(ICustomerActivityService customerActivityService,
            IDateTimeHelper dateTimeHelper, ILocalizationService localizationService,
            IPermissionService permissionService)
		{
            this._customerActivityService = customerActivityService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
		}

		#endregion 

        #region Activity log types

        public ActionResult ListTypes()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            var activityLogTypeModel = _customerActivityService.GetAllActivityTypes().Select(x => x.ToModel());
            var gridModel = new GridModel<ActivityLogTypeModel>
            {
                Data = activityLogTypeModel,
                Total = activityLogTypeModel.Count()
            };
            return View(gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult ListTypes(GridCommand command)
        {
            var activityLogTypeModel = _customerActivityService.GetAllActivityTypes().Select(x => x.ToModel());
            var gridModel = new GridModel<ActivityLogTypeModel>
            {
                Data = activityLogTypeModel,
                Total = activityLogTypeModel.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        [HttpPost]
        public ActionResult SaveTypes(FormCollection formCollection)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            var keys = formCollection.AllKeys.Where(c => c.StartsWith("checkBox_")).Select(c => c.Substring(9));
            foreach (var key in keys)
            {
                int id;
                if (Int32.TryParse(key,out id))
                {
                    var activityType = _customerActivityService.GetActivityTypeById(id);
                    activityType.Enabled = formCollection["checkBox_"+key].Equals("false") ? false : true;
                    _customerActivityService.UpdateActivityType(activityType);
                }

            }
            SuccessNotification(_localizationService.GetResource("Admin.Configuration.ActivityLog.ActivityLogType.Updated"));
            return RedirectToAction("ListTypes");
        }

        #endregion
        
        #region Activity log
        
        public ActionResult ListLogs()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            var activityLogSearchModel = new ActivityLogSearchModel();
            activityLogSearchModel.ActivityLogType.Add(new SelectListItem()
            {
                Value = "0",
                Text = "All"
            });


            foreach (var at in _customerActivityService.GetAllActivityTypes()
                .OrderBy(x=>x.Name)
                .Select(x =>
                {
                    return new SelectListItem()
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name
                    };
                }))
                activityLogSearchModel.ActivityLogType.Add(at);
            return View(activityLogSearchModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public JsonResult ListLogs(GridCommand command, ActivityLogSearchModel model)
        {
            DateTime? startDateValue = (model.CreatedOnFrom == null) ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.CreatedOnFrom.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.CreatedOnTo == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.CreatedOnTo.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            var activityLog = _customerActivityService.GetAllActivities(startDateValue, endDateValue,null, model.ActivityLogTypeId, command.Page - 1, command.PageSize);
            var gridModel = new GridModel<ActivityLogModel>
            {
                Data = activityLog.Select(x =>
                {
                    var m = x.ToModel();
                    m.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
                    return m;
                    
                }),
                Total = activityLog.TotalCount
            };
            return new JsonResult { Data = gridModel};;
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult AcivityLogDelete(int id, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            var activityLog = _customerActivityService.GetActivityById(id);
            if (activityLog == null)
                throw new ArgumentException("No activity log found with the specified id");
            
            _customerActivityService.DeleteActivity(activityLog);

            //TODO pass and return current ActivityLogSearchModel
            return ListLogs(command, new ActivityLogSearchModel());
        }

        public ActionResult ClearAll()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            _customerActivityService.ClearAllActivities();
            return RedirectToAction("ListLogs");
        }

        #endregion

    }
}
