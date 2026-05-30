using System.Text;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Services.ExportImport;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class NewsLetterSubscriptionController : BaseAdminController
{
    #region Fields

    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IExportManager _exportManager;
    protected readonly IImportManager _importManager;
    protected readonly ILocalizationService _localizationService;
    protected readonly INewsLetterSubscriptionModelFactory _newsletterSubscriptionModelFactory;
    protected readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
    protected readonly INewsLetterSubscriptionTypeService _newsLetterSubscriptionTypeService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;

    #endregion

    #region Ctor

    public NewsLetterSubscriptionController(IDateTimeHelper dateTimeHelper,
        IExportManager exportManager,
        IImportManager importManager,
        ILocalizationService localizationService,
        INewsLetterSubscriptionModelFactory newsletterSubscriptionModelFactory,
        INewsLetterSubscriptionService newsLetterSubscriptionService,
        INewsLetterSubscriptionTypeService newsLetterSubscriptionTypeService,
        INotificationService notificationService,
        IPermissionService permissionService)
    {
        _dateTimeHelper = dateTimeHelper;
        _exportManager = exportManager;
        _importManager = importManager;
        _localizationService = localizationService;
        _newsletterSubscriptionModelFactory = newsletterSubscriptionModelFactory;
        _newsLetterSubscriptionService = newsLetterSubscriptionService;
        _newsLetterSubscriptionTypeService = newsLetterSubscriptionTypeService;
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
        var model = await _newsletterSubscriptionModelFactory.PrepareNewsLetterSubscriptionSearchModelAsync(new NewsLetterSubscriptionSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.SUBSCRIBERS_VIEW)]
    public virtual async Task<IActionResult> List(NewsLetterSubscriptionSearchModel searchModel)
    {
        //prepare model
        var model = await _newsletterSubscriptionModelFactory.PrepareNewsLetterSubscriptionListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Promotions.SUBSCRIBERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create()
    {
        //prepare model
        var model = await _newsletterSubscriptionModelFactory.PrepareNewsLetterSubscriptionModelAsync(new NewsLetterSubscriptionModel(), null);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Promotions.SUBSCRIBERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create(NewsLetterSubscriptionModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var subscription = model.ToEntity<NewsLetterSubscription>();

            subscription.TypeId = model.SelectedNewsLetterSubscriptionTypeId;
            subscription.StoreId = model.SelectedNewsLetterSubscriptionStoreId;
            subscription.LanguageId = model.SelectedNewsLetterSubscriptionLanguageId;

            var subscriptionsByEmail = await _newsLetterSubscriptionService
                .GetNewsLetterSubscriptionsByEmailAsync(subscription.Email, storeId: subscription.StoreId);
            var availableSubscriptionTypes = await _newsLetterSubscriptionTypeService.GetAllNewsLetterSubscriptionTypesAsync(subscription.StoreId);

            var existingSubscription = subscriptionsByEmail.FirstOrDefault(s => s.TypeId == subscription.TypeId);
            if (existingSubscription == null && availableSubscriptionTypes.Any(x => x.Id == subscription.TypeId))
            {
                subscription.NewsLetterSubscriptionGuid = subscriptionsByEmail.FirstOrDefault()?.NewsLetterSubscriptionGuid ?? Guid.NewGuid();
                subscription.CreatedOnUtc = DateTime.UtcNow;
                await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(subscription);
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscription.Added"));

                return continueEditing ? RedirectToAction("Edit", new { id = subscription.Id }) : RedirectToAction("List");
            }
            else
            {
                _notificationService.WarningNotification(await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscription.Warning"));
            }
        }

        //prepare model
        model = await _newsletterSubscriptionModelFactory.PrepareNewsLetterSubscriptionModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Promotions.SUBSCRIBERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(int id)
    {
        var subscription = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByIdAsync(id);
        if (subscription == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _newsletterSubscriptionModelFactory.PrepareNewsLetterSubscriptionModelAsync(null, subscription);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Promotions.SUBSCRIBERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(NewsLetterSubscriptionModel model, bool continueEditing)
    {
        var subscription = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByIdAsync(model.Id);
        if (subscription == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            //fill entity from model
            subscription = model.ToEntity(subscription);

            subscription.TypeId = model.SelectedNewsLetterSubscriptionTypeId;
            subscription.LanguageId = model.SelectedNewsLetterSubscriptionLanguageId;
            subscription.StoreId = model.SelectedNewsLetterSubscriptionStoreId;

            var currentSubscription = (await _newsLetterSubscriptionService
                .GetNewsLetterSubscriptionsByEmailAsync(subscription.Email, storeId: subscription.StoreId, subscriptionTypeId: subscription.TypeId))
                .FirstOrDefault();

            var availableSubscriptionTypes = await _newsLetterSubscriptionTypeService.GetAllNewsLetterSubscriptionTypesAsync(subscription.StoreId);
            var isAvailableSubscriptionType = availableSubscriptionTypes.Any(x => x.Id == subscription.TypeId);

            if ((currentSubscription == null || (currentSubscription.Id == subscription.Id)) && isAvailableSubscriptionType)
            {
                await _newsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(subscription);
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscription.Updated"));

                return continueEditing ? RedirectToAction("Edit", new { id = subscription.Id }) : RedirectToAction("List");
            }
            else
            {
                _notificationService.WarningNotification(await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscription.Warning"));
            }
        }

        //prepare model
        model = await _newsletterSubscriptionModelFactory.PrepareNewsLetterSubscriptionModelAsync(model, subscription, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.SUBSCRIBERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
        var subscription = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByIdAsync(id);
        if (subscription == null)
            return RedirectToAction("List");

        try
        {
            await _newsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(subscription);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscription.Deleted"));

            return RedirectToAction("List");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("Edit", new { id = subscription.Id });
        }
    }

    #region Export/Import

    [HttpPost, ActionName("ExportCSV")]
    [FormValueRequired("exportcsv")]
    [CheckPermission(StandardPermission.Promotions.SUBSCRIBERS_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ExportCsv(NewsLetterSubscriptionSearchModel model)
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

        var subscriptions = (await _newsLetterSubscriptionService.GetAllNewsLetterSubscriptionsAsync(model.SearchEmail,
            startDateValue, endDateValue, model.StoreId, isActive, model.CustomerRoleId, model.SubscriptionTypeId)).ToList();

        var result = await _exportManager.ExportNewsLetterSubscribersToTxtAsync(subscriptions);

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
                var count = await _importManager.ImportNewsLetterSubscribersFromTxtAsync(importcsvfile.OpenReadStream());

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

    #endregion
}