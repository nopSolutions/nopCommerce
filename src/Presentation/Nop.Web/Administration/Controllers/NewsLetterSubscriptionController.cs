using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Nop.Admin.Models.Messages;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Messages;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
	[AdminAuthorize]
	public partial class NewsLetterSubscriptionController : BaseNopController
	{
		private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
		private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly AdminAreaSettings _adminAreaSettings;

		public NewsLetterSubscriptionController(INewsLetterSubscriptionService newsLetterSubscriptionService,
			IDateTimeHelper dateTimeHelper,ILocalizationService localizationService,
            IPermissionService permissionService, AdminAreaSettings adminAreaSettings)
		{
			this._newsLetterSubscriptionService = newsLetterSubscriptionService;
			this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._adminAreaSettings = adminAreaSettings;
		}

		public ActionResult Index()
		{
			return RedirectToAction("List");
		}

		public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNewsletterSubscribers))
                return AccessDeniedView();

            var newsletterSubscriptions = _newsLetterSubscriptionService.GetAllNewsLetterSubscriptions(String.Empty, 0, _adminAreaSettings.GridPageSize, true);
			var model = new NewsLetterSubscriptionListModel();

			model.NewsLetterSubscriptions = new GridModel<NewsLetterSubscriptionModel>
			{
				Data = newsletterSubscriptions.Select(x => 
				{
					var m = x.ToModel();
					m.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
					return m;
				}),
				Total = newsletterSubscriptions.TotalCount
			};
			return View(model);
		}

		[HttpPost, GridAction(EnableCustomBinding = true)]
		public ActionResult SubscriptionList(GridCommand command, NewsLetterSubscriptionListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNewsletterSubscribers))
                return AccessDeniedView();

            var newsletterSubscriptions = _newsLetterSubscriptionService.GetAllNewsLetterSubscriptions(model.SearchEmail, 
                command.Page - 1, command.PageSize, true);

            var gridModel = new GridModel<NewsLetterSubscriptionModel>
            {
                Data = newsletterSubscriptions.Select(x =>
				{
					var m = x.ToModel();
					m.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
					return m;
				}),
                Total = newsletterSubscriptions.TotalCount
            };
            return new JsonResult
            {
                Data = gridModel
            };
		}

        [GridAction(EnableCustomBinding = true)]
        public ActionResult SubscriptionUpdate(NewsLetterSubscriptionModel model, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNewsletterSubscribers))
                return AccessDeniedView();
            
            if (!ModelState.IsValid)
            {
                //display the first model error
                var modelStateErrors = this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            var subscription = _newsLetterSubscriptionService.GetNewsLetterSubscriptionById(model.Id);
            subscription.Email = model.Email;
            subscription.Active = model.Active;
            _newsLetterSubscriptionService.UpdateNewsLetterSubscription(subscription);

            return SubscriptionList(command, new NewsLetterSubscriptionListModel());
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult SubscriptionDelete(int id, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNewsletterSubscribers))
                return AccessDeniedView();

            var subscription = _newsLetterSubscriptionService.GetNewsLetterSubscriptionById(id);
            if (subscription == null)
                throw new ArgumentException("No subscription found with the specified id");
            _newsLetterSubscriptionService.DeleteNewsLetterSubscription(subscription);

            return SubscriptionList(command, new NewsLetterSubscriptionListModel());
        }

		public ActionResult ExportCsv(NewsLetterSubscriptionListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNewsletterSubscribers))
                return AccessDeniedView();

			string fileName = String.Format("newsletter_emails_{0}_{1}.txt", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), CommonHelper.GenerateRandomDigitCode(4));

			var sb = new StringBuilder();
			var newsLetterSubscriptions = _newsLetterSubscriptionService.GetAllNewsLetterSubscriptions(model.SearchEmail, 0, int.MaxValue, true);
			foreach (var subscription in newsLetterSubscriptions)
			{
				sb.Append(subscription.Email);
                sb.Append(",");
                sb.Append(subscription.Active);
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
                            else
                                throw new NopException("Wrong file format");

                            //import
                            var subscription = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmail(email);
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
