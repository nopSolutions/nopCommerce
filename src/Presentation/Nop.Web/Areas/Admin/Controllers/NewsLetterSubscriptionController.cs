using System;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.ExportImport;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class NewsLetterSubscriptionController : BaseAdminController
    {
        #region Fields

        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IExportManager _exportManager;
        private readonly IImportManager _importManager;
        private readonly ILocalizationService _localizationService;
        private readonly INewsletterSubscriptionModelFactory _newsletterSubscriptionModelFactory;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public NewsLetterSubscriptionController(IDateTimeHelper dateTimeHelper,
            IExportManager exportManager,
            IImportManager importManager,
            ILocalizationService localizationService,
            INewsletterSubscriptionModelFactory newsletterSubscriptionModelFactory,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IPermissionService permissionService)
        {
            this._dateTimeHelper = dateTimeHelper;
            this._exportManager = exportManager;
            this._importManager = importManager;
            this._localizationService = localizationService;
            this._newsletterSubscriptionModelFactory = newsletterSubscriptionModelFactory;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._permissionService = permissionService;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNewsletterSubscribers))
                return AccessDeniedView();

            //prepare model
            var model = _newsletterSubscriptionModelFactory.PrepareNewsletterSubscriptionSearchModel(new NewsletterSubscriptionSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult SubscriptionList(NewsletterSubscriptionSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNewsletterSubscribers))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _newsletterSubscriptionModelFactory.PrepareNewsletterSubscriptionListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult SubscriptionUpdate(NewsletterSubscriptionModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNewsletterSubscribers))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });

            var subscription = _newsLetterSubscriptionService.GetNewsLetterSubscriptionById(model.Id);
            subscription.Email = model.Email;
            subscription.Active = model.Active;
            _newsLetterSubscriptionService.UpdateNewsLetterSubscription(subscription);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult SubscriptionDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNewsletterSubscribers))
                return AccessDeniedView();

            var subscription = _newsLetterSubscriptionService.GetNewsLetterSubscriptionById(id)
                ?? throw new ArgumentException("No subscription found with the specified id", nameof(id));

            _newsLetterSubscriptionService.DeleteNewsLetterSubscription(subscription);

            return new NullJsonResult();
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("exportcsv")]
        public virtual IActionResult ExportCsv(NewsletterSubscriptionSearchModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNewsletterSubscribers))
                return AccessDeniedView();

            bool? isActive = null;
            if (model.ActiveId == 1)
                isActive = true;
            else if (model.ActiveId == 2)
                isActive = false;

            var startDateValue = model.StartDate == null ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);
            var endDateValue = model.EndDate == null ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            var subscriptions = _newsLetterSubscriptionService.GetAllNewsLetterSubscriptions(model.SearchEmail,
                startDateValue, endDateValue, model.StoreId, isActive, model.CustomerRoleId);

            var result = _exportManager.ExportNewsletterSubscribersToTxt(subscriptions);

            var fileName = $"newsletter_emails_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}_{CommonHelper.GenerateRandomDigitCode(4)}.txt";

            return File(Encoding.UTF8.GetBytes(result), MimeTypes.TextCsv, fileName);
        }

        [HttpPost]
        public virtual IActionResult ImportCsv(IFormFile importcsvfile)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNewsletterSubscribers))
                return AccessDeniedView();

            try
            {
                if (importcsvfile != null && importcsvfile.Length > 0)
                {
                    var count = _importManager.ImportNewsletterSubscribersFromTxt(importcsvfile.OpenReadStream());

                    SuccessNotification(string.Format(_localizationService.GetResource("Admin.Promotions.NewsLetterSubscriptions.ImportEmailsSuccess"), count));

                    return RedirectToAction("List");
                }

                ErrorNotification(_localizationService.GetResource("Admin.Common.UploadFile"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        #endregion
    }
}