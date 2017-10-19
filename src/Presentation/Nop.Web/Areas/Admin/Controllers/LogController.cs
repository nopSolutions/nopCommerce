using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Models.Logging;
using Nop.Core;
using Nop.Core.Domain.Logging;
using Nop.Services;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Core.Html;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class LogController : BaseAdminController
    {
        #region Fields

        private readonly ICustomerActivityService _customerActivityService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public LogController(ICustomerActivityService customerActivityService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            ILogger logger,
            IPermissionService permissionService,
            IWorkContext workContext)
        {
            this._customerActivityService = customerActivityService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._logger = logger;
            this._permissionService = permissionService;
            this._workContext = workContext;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            var model = new LogListModel
            {
                AvailableLogLevels = LogLevel.Debug.ToSelectList(false).ToList()
            };
            model.AvailableLogLevels.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult LogList(DataSourceRequest command, LogListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedKendoGridJson();

            var createdOnFromValue = model.CreatedOnFrom != null
                ? (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.CreatedOnFrom.Value, _dateTimeHelper.CurrentTimeZone) : null;

            var createdToFromValue = model.CreatedOnTo != null
                ? (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.CreatedOnTo.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1) : null;

            var logLevel = model.LogLevelId > 0 ? (LogLevel?)(model.LogLevelId) : null;

            var logItems = _logger.GetAllLogs(createdOnFromValue, createdToFromValue, model.Message, logLevel, command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = logItems.Select(logItem => new LogModel
                {
                    Id = logItem.Id,
                    LogLevel = logItem.LogLevel.GetLocalizedEnum(_localizationService, _workContext),
                    ShortMessage = HtmlHelper.FormatText(logItem.ShortMessage, false, true, false, false, false, false),
                    FullMessage = string.Empty, //little performance optimization: ensure that "FullMessage" is not returned
                    IpAddress = logItem.IpAddress,
                    CustomerId = logItem.CustomerId,
                    CustomerEmail = logItem.Customer?.Email,
                    PageUrl = logItem.PageUrl,
                    ReferrerUrl = logItem.ReferrerUrl,
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(logItem.CreatedOnUtc, DateTimeKind.Utc)
                }),
                Total = logItems.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("clearall")]
        public virtual IActionResult ClearAll()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            _logger.ClearLog();

            //activity log
            _customerActivityService.InsertActivity("DeleteSystemLog", _localizationService.GetResource("ActivityLog.DeleteSystemLog"));

            SuccessNotification(_localizationService.GetResource("Admin.System.Log.Cleared"));
            return RedirectToAction("List");
        }

        public virtual IActionResult View(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            var log = _logger.GetLogById(id);
            if (log == null)
                //No log found with the specified id
                return RedirectToAction("List");

            var model = new LogModel
            {
                Id = log.Id,
                LogLevel = log.LogLevel.GetLocalizedEnum(_localizationService, _workContext),
                ShortMessage = HtmlHelper.FormatText(log.ShortMessage, false, true, false, false, false, false),
                FullMessage = HtmlHelper.FormatText(log.FullMessage, false, true, false, false, false, false),
                IpAddress = log.IpAddress,
                CustomerId = log.CustomerId,
                CustomerEmail = log.Customer?.Email,
                PageUrl = log.PageUrl,
                ReferrerUrl = log.ReferrerUrl,
                CreatedOn = _dateTimeHelper.ConvertToUserTime(log.CreatedOnUtc, DateTimeKind.Utc)
            };

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            var log = _logger.GetLogById(id);
            if (log == null)
                //No log found with the specified id
                return RedirectToAction("List");

            _logger.DeleteLog(log);

            //activity log
            _customerActivityService.InsertActivity("DeleteSystemLog", _localizationService.GetResource("ActivityLog.DeleteSystemLog"));

            SuccessNotification(_localizationService.GetResource("Admin.System.Log.Deleted"));
            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual IActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            if (selectedIds != null)
                _logger.DeleteLogs(_logger.GetLogByIds(selectedIds.ToArray()).ToList());

            //activity log
            _customerActivityService.InsertActivity("DeleteSystemLog", _localizationService.GetResource("ActivityLog.DeleteSystemLog"));

            return Json(new {Result = true});
        }

        #endregion
    }
}