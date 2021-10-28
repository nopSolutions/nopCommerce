using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class NewsLetterSubscriptionController : BaseAdminController
    {
        #region Fields

        protected IDateTimeHelper DateTimeHelper { get; }
        protected IExportManager ExportManager { get; }
        protected IImportManager ImportManager { get; }
        protected ILocalizationService LocalizationService { get; }
        protected INewsletterSubscriptionModelFactory NewsletterSubscriptionModelFactory { get; }
        protected INewsLetterSubscriptionService NewsLetterSubscriptionService { get; }
        protected INotificationService NotificationService { get; }
        protected IPermissionService PermissionService { get; }

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
            DateTimeHelper = dateTimeHelper;
            ExportManager = exportManager;
            ImportManager = importManager;
            LocalizationService = localizationService;
            NewsletterSubscriptionModelFactory = newsletterSubscriptionModelFactory;
            NewsLetterSubscriptionService = newsLetterSubscriptionService;
            NotificationService = notificationService;
            PermissionService = permissionService;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageNewsletterSubscribers))
                return AccessDeniedView();

            //prepare model
            var model = await NewsletterSubscriptionModelFactory.PrepareNewsletterSubscriptionSearchModelAsync(new NewsletterSubscriptionSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> SubscriptionList(NewsletterSubscriptionSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageNewsletterSubscribers))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await NewsletterSubscriptionModelFactory.PrepareNewsletterSubscriptionListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> SubscriptionUpdate(NewsletterSubscriptionModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageNewsletterSubscribers))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            var subscription = await NewsLetterSubscriptionService.GetNewsLetterSubscriptionByIdAsync(model.Id);

            //fill entity from model
            subscription = model.ToEntity(subscription);
            await NewsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(subscription);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> SubscriptionDelete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageNewsletterSubscribers))
                return AccessDeniedView();

            var subscription = await NewsLetterSubscriptionService.GetNewsLetterSubscriptionByIdAsync(id)
                ?? throw new ArgumentException("No subscription found with the specified id", nameof(id));

            await NewsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(subscription);

            return new NullJsonResult();
        }

        [HttpPost, ActionName("ExportCSV")]
        [FormValueRequired("exportcsv")]
        public virtual async Task<IActionResult> ExportCsv(NewsletterSubscriptionSearchModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageNewsletterSubscribers))
                return AccessDeniedView();

            bool? isActive = null;
            if (model.ActiveId == 1)
                isActive = true;
            else if (model.ActiveId == 2)
                isActive = false;

            var startDateValue = model.StartDate == null ? null
                : (DateTime?)DateTimeHelper.ConvertToUtcTime(model.StartDate.Value, await DateTimeHelper.GetCurrentTimeZoneAsync());
            var endDateValue = model.EndDate == null ? null
                : (DateTime?)DateTimeHelper.ConvertToUtcTime(model.EndDate.Value, await DateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

            var subscriptions = await NewsLetterSubscriptionService.GetAllNewsLetterSubscriptionsAsync(model.SearchEmail,
                startDateValue, endDateValue, model.StoreId, isActive, model.CustomerRoleId);

            var result = await ExportManager.ExportNewsletterSubscribersToTxtAsync(subscriptions);

            var fileName = $"newsletter_emails_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}_{CommonHelper.GenerateRandomDigitCode(4)}.csv";

            return File(Encoding.UTF8.GetBytes(result), MimeTypes.TextCsv, fileName);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ImportCsv(IFormFile importcsvfile)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageNewsletterSubscribers))
                return AccessDeniedView();

            try
            {
                if (importcsvfile != null && importcsvfile.Length > 0)
                {
                    var count = await ImportManager.ImportNewsletterSubscribersFromTxtAsync(importcsvfile.OpenReadStream());

                    NotificationService.SuccessNotification(string.Format(await LocalizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscriptions.ImportEmailsSuccess"), count));

                    return RedirectToAction("List");
                }

                NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Admin.Common.UploadFile"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        #endregion
    }
}