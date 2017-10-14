using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Core;
using Nop.Services.Customers;
using Nop.Services.ExportImport;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Areas.Admin.Controllers
{
	public partial class NewsLetterSubscriptionController : BaseAdminController
	{
	    #region Fields

        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
		private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreService _storeService;
        private readonly ICustomerService _customerService;
        private readonly IExportManager _exportManager;
        private readonly IImportManager _importManager;

        #endregion

	    #region Ctor

        public NewsLetterSubscriptionController(INewsLetterSubscriptionService newsLetterSubscriptionService,
			IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IStoreService storeService,
            ICustomerService customerService,
            IExportManager exportManager,
            IImportManager importManager)
		{
			this._newsLetterSubscriptionService = newsLetterSubscriptionService;
			this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._storeService = storeService;
            this._customerService = customerService;
            this._exportManager = exportManager;
            this._importManager = importManager;
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

            var model = new NewsLetterSubscriptionListModel();

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //active
            model.ActiveList.Add(new SelectListItem
            {
                Value = "0",
                Text = _localizationService.GetResource("Admin.Promotions.NewsLetterSubscriptions.List.SearchActive.All")
            });
            model.ActiveList.Add(new SelectListItem
            {
                Value = "1",
                Text = _localizationService.GetResource("Admin.Promotions.NewsLetterSubscriptions.List.SearchActive.ActiveOnly")
            });
            model.ActiveList.Add(new SelectListItem
            {
                Value = "2",
                Text = _localizationService.GetResource("Admin.Promotions.NewsLetterSubscriptions.List.SearchActive.NotActiveOnly")
            });

            //customer roles
            model.AvailableCustomerRoles.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var cr in _customerService.GetAllCustomerRoles(true))
                model.AvailableCustomerRoles.Add(new SelectListItem { Text = cr.Name, Value = cr.Id.ToString() });

			return View(model);
		}

		[HttpPost]
		public virtual IActionResult SubscriptionList(DataSourceRequest command, NewsLetterSubscriptionListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNewsletterSubscribers))
                return AccessDeniedKendoGridJson();

            bool? isActive = null;
            if (model.ActiveId == 1)
                isActive = true;
            else if (model.ActiveId == 2)
                isActive = false;

            var startDateValue = (model.StartDate == null) ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);
            var endDateValue = (model.EndDate == null) ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            var newsletterSubscriptions = _newsLetterSubscriptionService.GetAllNewsLetterSubscriptions(model.SearchEmail,
                startDateValue, endDateValue, model.StoreId, isActive, model.CustomerRoleId,
                command.Page - 1, command.PageSize);

            var gridModel = new DataSourceResult
            {
                Data = newsletterSubscriptions.Select(x =>
				{
					var m = x.ToModel();
				    var store = _storeService.GetStoreById(x.StoreId);
				    m.StoreName = store != null ? store.Name : "Unknown store";
					m.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc).ToLongTimeString();
					return m;
				}),
                Total = newsletterSubscriptions.TotalCount
            };

            return Json(gridModel);
		}

        [HttpPost]
        public virtual IActionResult SubscriptionUpdate(NewsLetterSubscriptionModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNewsletterSubscribers))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }

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

            var subscription = _newsLetterSubscriptionService.GetNewsLetterSubscriptionById(id);
            if (subscription == null)
                throw new ArgumentException("No subscription found with the specified id");
            _newsLetterSubscriptionService.DeleteNewsLetterSubscription(subscription);

            return new NullJsonResult();
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("exportcsv")]
		public virtual IActionResult ExportCsv(NewsLetterSubscriptionListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNewsletterSubscribers))
                return AccessDeniedView();

            bool? isActive = null;
            if (model.ActiveId == 1)
                isActive = true;
            else if (model.ActiveId == 2)
                isActive = false;

            var startDateValue = (model.StartDate == null) ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);
            var endDateValue = (model.EndDate == null) ? null
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