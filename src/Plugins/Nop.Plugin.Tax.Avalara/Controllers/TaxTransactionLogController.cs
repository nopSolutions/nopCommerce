using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Tax.Avalara.Models.Log;
using Nop.Plugin.Tax.Avalara.Services;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Tax.Avalara.Controllers;

public class TaxTransactionLogController : BaseAdminController
{
    #region Fields

    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IHtmlFormatter _htmlFormatter;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly TaxTransactionLogService _taxTransactionLogService;

    #endregion

    #region Ctor

    public TaxTransactionLogController(ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IHtmlFormatter htmlFormatter,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IPermissionService permissionService,
        TaxTransactionLogService taxTransactionLogService)
    {
        _customerService = customerService;
        _dateTimeHelper = dateTimeHelper;
        _htmlFormatter = htmlFormatter;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _permissionService = permissionService;
        _taxTransactionLogService = taxTransactionLogService;
    }

    #endregion

    #region Methods

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> LogList(TaxTransactionLogSearchModel searchModel)
    {
        //prepare filter parameters
        var createdFromValue = searchModel.CreatedFrom.HasValue
            ? (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedFrom.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync())
            : null;
        var createdToValue = searchModel.CreatedTo.HasValue
            ? (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedTo.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1)
            : null;

        //get tax transaction log
        var taxtransactionLog = await _taxTransactionLogService.GetTaxTransactionLogAsync(createdFromUtc: createdFromValue, createdToUtc: createdToValue,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare grid model
        var model = await new TaxTransactionLogListModel().PrepareToGridAsync(searchModel, taxtransactionLog, () =>
        {
            return taxtransactionLog.SelectAwait(async logItem => new TaxTransactionLogModel
            {
                Id = logItem.Id,
                StatusCode = logItem.StatusCode,
                Url = logItem.Url,
                CustomerId = logItem.CustomerId,
                CreatedDate = await _dateTimeHelper.ConvertToUserTimeAsync(logItem.CreatedDateUtc, DateTimeKind.Utc)
            });
        });

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
    {
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        await _taxTransactionLogService.DeleteTaxTransactionLogAsync(selectedIds.ToArray());

        return Json(new { Result = true });
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> View(int id)
    {
        //try to get log item with the passed identifier
        var logItem = await _taxTransactionLogService.GetTaxTransactionLogByIdAsync(id);
        if (logItem == null)
            return RedirectToAction("Configure", "Avalara");

        var model = new TaxTransactionLogModel
        {
            Id = logItem.Id,
            StatusCode = logItem.StatusCode,
            Url = logItem.Url,
            RequestMessage = _htmlFormatter.FormatText(logItem.RequestMessage, false, true, false, false, false, false),
            ResponseMessage = _htmlFormatter.FormatText(logItem.ResponseMessage, false, true, false, false, false, false),
            CustomerId = logItem.CustomerId,
            CustomerEmail = (await _customerService.GetCustomerByIdAsync(logItem.CustomerId))?.Email,
            CreatedDate = await _dateTimeHelper.ConvertToUserTimeAsync(logItem.CreatedDateUtc, DateTimeKind.Utc)
        };

        return View("~/Plugins/Tax.Avalara/Views/Log/View.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> Delete(int id)
    {
        //try to get log item with the passed identifier
        var logItem = await _taxTransactionLogService.GetTaxTransactionLogByIdAsync(id);
        if (logItem != null)
        {
            await _taxTransactionLogService.DeleteTaxTransactionLogAsync(logItem);
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.Log.Deleted"));
        }

        return RedirectToAction("Configure", "Avalara");
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> ClearAll()
    {
        await _taxTransactionLogService.ClearLogAsync();

        return Json(new { Result = true });
    }

    #endregion
}