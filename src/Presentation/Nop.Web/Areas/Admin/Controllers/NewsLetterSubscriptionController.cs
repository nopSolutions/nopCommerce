using System.Text;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.ExportImport;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class NewsLetterSubscriptionController : BaseAdminController
{
    #region Fields

    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IExportManager _exportManager;
    protected readonly IImportManager _importManager;
    protected readonly ILocalizationService _localizationService;
    protected readonly INewsletterSubscriptionModelFactory _newsletterSubscriptionModelFactory;
    protected readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;

    #endregion

    #region Ctor

    public NewsLetterSubscriptionController(IDateTimeHelper dateTimeHelper,
        IExportManager exportManager,
        IImportManager importManager,
        ILocalizationService localizationService,
        INewsletterSubscriptionModelFactory newsletterSubscriptionModelFactory,
        INewsLetterSubscriptionService newsLetterSubscriptionService,
        INotificationService notificationService,
        IPermissionService permissionService)
    {
        _dateTimeHelper = dateTimeHelper;
        _exportManager = exportManager;
        _importManager = importManager;
        _localizationService = localizationService;
        _newsletterSubscriptionModelFactory = newsletterSubscriptionModelFactory;
        _newsLetterSubscriptionService = newsLetterSubscriptionService;
        _notificationService = notificationService;
        _permissionService = permissionService;
    }

    #endregion

    #region Methods

    public virtual IActionResult Index()
    {
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Promotions.SUBSCRIBERS_VIEW)]
    public virtual async Task<IActionResult> List()
    {
        //prepare model
        var model = await _newsletterSubscriptionModelFactory.PrepareNewsletterSubscriptionSearchModelAsync(new NewsletterSubscriptionSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.SUBSCRIBERS_VIEW)]
    public virtual async Task<IActionResult> SubscriptionList(NewsletterSubscriptionSearchModel searchModel)
    {
        //prepare model
        var model = await _newsletterSubscriptionModelFactory.PrepareNewsletterSubscriptionListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.SUBSCRIBERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> SubscriptionUpdate(NewsletterSubscriptionModel model)
    {
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        var subscription = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByIdAsync(model.Id);

        //fill entity from model
        subscription = model.ToEntity(subscription);
        await _newsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(subscription);

        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.SUBSCRIBERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> SubscriptionDelete(int id)
    {
        var subscription = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByIdAsync(id)
            ?? throw new ArgumentException("No subscription found with the specified id", nameof(id));

        await _newsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(subscription);

        return new NullJsonResult();
    }

    [HttpPost, ActionName("ExportCSV")]
    [FormValueRequired("exportcsv")]
    [CheckPermission(StandardPermission.Promotions.SUBSCRIBERS_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ExportCsv(NewsletterSubscriptionSearchModel model)
    {
        bool? isActive = null;
        if (model.ActiveId == 1)
            isActive = true;
        else if (model.ActiveId == 2)
            isActive = false;

        var startDateValue = model.StartDate == null ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var endDateValue = model.EndDate == null ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

        var subscriptions = await _newsLetterSubscriptionService.GetAllNewsLetterSubscriptionsAsync(model.SearchEmail,
            startDateValue, endDateValue, model.StoreId, isActive, model.CustomerRoleId);

        var result = await _exportManager.ExportNewsletterSubscribersToTxtAsync(subscriptions);

        var fileName = $"newsletter_emails_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}_{CommonHelper.GenerateRandomDigitCode(4)}.csv";

        return File(Encoding.UTF8.GetBytes(result), MimeTypes.TextCsv, fileName);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.SUBSCRIBERS_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ImportCsv(IFormFile importcsvfile)
    {
        try
        {
            if (importcsvfile != null && importcsvfile.Length > 0)
            {
                var count = await _importManager.ImportNewsletterSubscribersFromTxtAsync(importcsvfile.OpenReadStream());

                _notificationService.SuccessNotification(string.Format(await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscriptions.ImportEmailsSuccess"), count));

                return RedirectToAction("List");
            }

            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Common.UploadFile"));
            return RedirectToAction("List");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("List");
        }
    }

    #endregion
}