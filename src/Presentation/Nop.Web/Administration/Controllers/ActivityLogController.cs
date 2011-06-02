using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Models;
using Nop.Admin.Models.Logging;
using Nop.Core.Domain.Logging;
using Nop.Services.Helpers;
using Nop.Services.Logging;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
    public class ActivityLogController : Controller
    {
        #region Fields

        private readonly ICustomerActivityService _customerActivityService;
        private readonly IDateTimeHelper _dateTimeHelper;
        #endregion Fields

        #region Constructors

        public ActivityLogController(ICustomerActivityService customerActivityService, IDateTimeHelper dateTimeHelper)
		{
            this._customerActivityService = customerActivityService;
            this._dateTimeHelper = dateTimeHelper;
		}

		#endregion Constructors 

        #region Activity log types
        public ActionResult ListTypes()
        {
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
        public ActionResult SaveTypes(FormCollection formCollection, ActivityLogSearchModel model)
        {
            var keys = formCollection.AllKeys.Where(c => c.StartsWith("checkBox_")).Select(c => c.Substring(9));
            foreach (var key in keys)
            {
                int id;
                if (Int32.TryParse(key,out id))
                {
                    var activityType = _customerActivityService.GetActivityTypeById(id);
                    activityType.Enabled = formCollection["checkBox_"+key.ToString()].Equals("false") ? false : true;
                    _customerActivityService.UpdateActivityType(activityType);
                }

            }
            return RedirectToAction("ListTypes");
        }
        #endregion
        
        #region Activity log
        
        public ActionResult ListLogs()
        {
            var activityLogSearchModel = new ActivityLogSearchModel();
            activityLogSearchModel.ActivityLogType = _customerActivityService.GetAllActivityTypes().Select(x =>
                                                    {
                                                        return new SelectListItem()
                                                        {
                                                            Selected = false,
                                                            Value = x.Id.ToString(),
                                                            Text = x.Name
                                                        };
                                                    }).ToList();
            activityLogSearchModel.ActivityLogType.Add(new SelectListItem()
            {
                Value = "0",
                Selected = true,
                Text = "All"
            });
            activityLogSearchModel.ActivityLogType.ToList().Sort((a, b) => a.Text.CompareTo(b.Text));
            return View(activityLogSearchModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public JsonResult ListLogs(GridCommand command, ActivityLogSearchModel model)
        {
            DateTime? startDateValue = (model.CreatedOnFrom == null) ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.CreatedOnFrom.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.CreatedOnTo == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.CreatedOnTo.Value, _dateTimeHelper.CurrentTimeZone);

            var activityLogModel = _customerActivityService.GetAllActivities(startDateValue, endDateValue, model.CustomerEmail, null, model.ActivityLogTypeId, command.Page - 1, command.PageSize);
            var gridModel = new GridModel<ActivityLogModel>
            {
                Data = activityLogModel.Select(x => x.ToModel()),
                Total = activityLogModel.TotalCount
            };
            return new JsonResult { Data = gridModel};;
        }
        
        public ActionResult DeleteLog(int id)
        {
            _customerActivityService.DeleteActivity(id);
            return RedirectToAction("ListLogs");
        }

        public ActionResult ClearAll()
        {
            _customerActivityService.ClearAllActivities();
            return RedirectToAction("ListLogs");
        }
        #endregion

    }
}
