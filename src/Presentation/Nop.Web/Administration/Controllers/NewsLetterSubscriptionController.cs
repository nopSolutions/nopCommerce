using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Nop.Admin.Models.Messages;
using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Stores;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Controllers
{
	public partial class NewsLetterSubscriptionController : BaseAdminController
	{
		private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
		private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;

		public NewsLetterSubscriptionController(INewsLetterSubscriptionService newsLetterSubscriptionService,
			IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IStoreContext storeContext,
            IStoreService storeService)
		{
			this._newsLetterSubscriptionService = newsLetterSubscriptionService;
			this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._storeContext = storeContext;
            this._storeService = storeService;
		}

		public ActionResult Index()
		{
			return RedirectToAction("List");
		}

		public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNewsletterSubscribers))
                return AccessDeniedView();

            var model = new NewsLetterSubscriptionListModel();

            //stores
            model.AvailableStores.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString() });
            
			return View(model);
		}

		[HttpPost]
		public ActionResult SubscriptionList(DataSourceRequest command, NewsLetterSubscriptionListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNewsletterSubscribers))
                return AccessDeniedView();

            var newsletterSubscriptions = _newsLetterSubscriptionService.GetAllNewsLetterSubscriptions(model.SearchEmail, 
                model.StoreId, command.Page - 1, command.PageSize, true);

            var gridModel = new DataSourceResult
            {
                Data = newsletterSubscriptions.Select(x =>
				{
					var m = x.ToModel();
				    var store = _storeService.GetStoreById(x.StoreId);
				    m.StoreName = store != null ? store.Name : "Unknown store";
					m.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
					return m;
				}),
                Total = newsletterSubscriptions.TotalCount
            };

            return Json(gridModel);
		}

        [HttpPost]
        public ActionResult SubscriptionUpdate([Bind(Exclude = "CreatedOn")] NewsLetterSubscriptionModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNewsletterSubscribers))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult() { Errors = ModelState.SerializeErrors() });
            }

            var subscription = _newsLetterSubscriptionService.GetNewsLetterSubscriptionById(model.Id);
            subscription.Email = model.Email;
            subscription.Active = model.Active;
            _newsLetterSubscriptionService.UpdateNewsLetterSubscription(subscription);

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult SubscriptionDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNewsletterSubscribers))
                return AccessDeniedView();

            var subscription = _newsLetterSubscriptionService.GetNewsLetterSubscriptionById(id);
            if (subscription == null)
                throw new ArgumentException("No subscription found with the specified id");
            _newsLetterSubscriptionService.DeleteNewsLetterSubscription(subscription);

            return new NullJsonResult();
        }

		public ActionResult ExportCsv(NewsLetterSubscriptionListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNewsletterSubscribers))
                return AccessDeniedView();

			string fileName = String.Format("newsletter_emails_{0}_{1}.txt", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), CommonHelper.GenerateRandomDigitCode(4));

			var sb = new StringBuilder();
			var newsLetterSubscriptions = _newsLetterSubscriptionService.GetAllNewsLetterSubscriptions(model.SearchEmail,
                model.StoreId, 0, int.MaxValue, true);
			foreach (var subscription in newsLetterSubscriptions)
			{
				sb.Append(subscription.Email);
                sb.Append(",");
                sb.Append(subscription.Active);
                sb.Append(",");
                sb.Append(subscription.StoreId);
                sb.Append(Environment.NewLine);  //new line
			}
			string result = sb.ToString();

			return File(Encoding.UTF8.GetBytes(result), "text/csv", fileName);
		}

        [HttpPost]
        public ActionResult ImportCsv(FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNewsletterSubscribers))
                return AccessDeniedView();

            try
            {
                var file = Request.Files["importcsvfile"];
                if (file != null && file.ContentLength > 0)
                {
                    int count = 0;

                    using (var reader = new StreamReader(file.InputStream))
                    {
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            if (String.IsNullOrWhiteSpace(line))
                                continue;
                            string[] tmp = line.Split(',');

                            var email = "";
                            bool isActive = true;
                            int storeId = _storeContext.CurrentStore.Id;
                            //parse
                            if (tmp.Length == 1)
                            {
                                //"email" only
                                email = tmp[0].Trim();
                            }
                            else if (tmp.Length == 2)
                            {
                                //"email" and "active" fields specified
                                email = tmp[0].Trim();
                                isActive = Boolean.Parse(tmp[1].Trim());
                            }
                            else if (tmp.Length ==3)
                            {
                                //"email" and "active" and "storeId" fields specified
                                email = tmp[0].Trim();
                                isActive = Boolean.Parse(tmp[1].Trim());
                                storeId = Int32.Parse(tmp[2].Trim());
                            }
                            else
                                throw new NopException("Wrong file format");

                            //import
                            var subscription = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(email, storeId);
                            if (subscription != null)
                            {
                                subscription.Email = email;
                                subscription.Active = isActive;
                                _newsLetterSubscriptionService.UpdateNewsLetterSubscription(subscription);
                            }
                            else
                            {
                                subscription = new NewsLetterSubscription()
                                {
                                    Active = isActive,
                                    CreatedOnUtc = DateTime.UtcNow,
                                    Email = email,
                                    StoreId = storeId,
                                    NewsLetterSubscriptionGuid = Guid.NewGuid()
                                };
                                _newsLetterSubscriptionService.InsertNewsLetterSubscription(subscription);
                            }
                            count++;
                        }
                        SuccessNotification(String.Format(_localizationService.GetResource("Admin.Promotions.NewsLetterSubscriptions.ImportEmailsSuccess"), count));
                        return RedirectToAction("List");
                    }
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
	}
}
