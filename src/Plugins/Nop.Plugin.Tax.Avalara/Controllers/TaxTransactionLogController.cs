using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Html;
using Nop.Plugin.Tax.Avalara.Models.Log;
using Nop.Plugin.Tax.Avalara.Services;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.Tax.Avalara.Controllers
{
    public class TaxTransactionLogController : BaseAdminController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly TaxTransactionLogService _taxTransactionLogService;

        #endregion

        #region Ctor

        public TaxTransactionLogController(ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            TaxTransactionLogService taxTransactionLogService)
        {
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _taxTransactionLogService = taxTransactionLogService;
        }

        #endregion

        #region Methods

        [HttpPost]
        public virtual IActionResult LogList(TaxTransactionLogSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedDataTablesJson();

            //prepare filter parameters
            var createdFromValue = searchModel.CreatedFrom.HasValue
                ? (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedFrom.Value, _dateTimeHelper.CurrentTimeZone) : null;
            var createdToValue = searchModel.CreatedTo.HasValue
                ? (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedTo.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1) : null;

            //get tax transaction log
            var taxtransactionLog = _taxTransactionLogService.GetTaxTransactionLog(createdFromUtc: createdFromValue,
                createdToUtc: createdToValue, pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new TaxTransactionLogListModel().PrepareToGrid(searchModel, taxtransactionLog, () =>
            {
                return taxtransactionLog.Select(logItem => new TaxTransactionLogModel
                {
                    Id = logItem.Id,
                    StatusCode = logItem.StatusCode,
                    Url = logItem.Url,
                    CustomerId = logItem.CustomerId,
                    CreatedDate = _dateTimeHelper.ConvertToUserTime(logItem.CreatedDateUtc, DateTimeKind.Utc)
                });
            });

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            if (selectedIds != null)
                _taxTransactionLogService.DeleteTaxTransactionLog(selectedIds.ToArray());

            return Json(new { Result = true });
        }

        public virtual IActionResult View(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            //try to get log item with the passed identifier
            var logItem = _taxTransactionLogService.GetTaxTransactionLogById(id);
            if (logItem == null)
                return RedirectToAction("Configure", "AvalaraTax");

            var model = new TaxTransactionLogModel
            {
                Id = logItem.Id,
                StatusCode = logItem.StatusCode,
                Url = logItem.Url,
                RequestMessage = HtmlHelper.FormatText(logItem.RequestMessage, false, true, false, false, false, false),
                ResponseMessage = HtmlHelper.FormatText(logItem.ResponseMessage, false, true, false, false, false, false),
                CustomerId = logItem.CustomerId,
                CustomerEmail = _customerService.GetCustomerById(logItem.CustomerId)?.Email,
                CreatedDate = _dateTimeHelper.ConvertToUserTime(logItem.CreatedDateUtc, DateTimeKind.Utc)
            };

            return View("~/Plugins/Tax.Avalara/Views/Log/View.cshtml", model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            //try to get log item with the passed identifier
            var logItem = _taxTransactionLogService.GetTaxTransactionLogById(id);
            if (logItem != null)
            {
                _taxTransactionLogService.DeleteTaxTransactionLog(logItem);
                _notificationService.SuccessNotification(_localizationService.GetResource("Plugins.Tax.Avalara.Log.Deleted"));
            }

            return RedirectToAction("Configure", "AvalaraTax");
        }

        #endregion
    }
}