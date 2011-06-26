using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Models.Messages;
using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Services.Helpers;
using Nop.Services.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;

using Telerik.Web.Mvc;
using System.Text;
using System.IO;
using Nop.Services.Localization;

namespace Nop.Admin.Controllers
{
	[AdminAuthorize]
	public class NewsLetterSubscriptionController : BaseNopController
	{
		private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
		private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;

		public NewsLetterSubscriptionController(INewsLetterSubscriptionService newsLetterSubscriptionService,
			IDateTimeHelper dateTimeHelper,ILocalizationService localizationService)
		{
			this._newsLetterSubscriptionService = newsLetterSubscriptionService;
			this._dateTimeHelper = dateTimeHelper;
		    this._localizationService = localizationService;
		}

		public ActionResult Index()
		{
			return RedirectToAction("List");
		}

		public ActionResult List()
		{
			var newsletterSubscriptions = _newsLetterSubscriptionService.GetAllNewsLetterSubscriptions(String.Empty, true);
			var model = new NewsLetterSubscriptionListModel();

			model.NewsLetterSubscriptions = new GridModel<NewsLetterSubscriptionModel>
			{
				Data = newsletterSubscriptions.Select(x => 
				{
					var m = x.ToModel();
					m.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
					return m;
				}),
				Total = newsletterSubscriptions.Count()
			};
			return View(model);
		}

		[HttpPost, GridAction(EnableCustomBinding = true)]
		public ActionResult SubscriptionList(GridCommand command)
		{
			string searchCustomerEmail = command.FilterDescriptors.GetValueFromAppliedFilters("searchCustomerEmail");

			var newsletterSubscriptions = _newsLetterSubscriptionService.GetAllNewsLetterSubscriptions(searchCustomerEmail, true);

			var model = new NewsLetterSubscriptionListModel();
			model.SearchEmail = searchCustomerEmail;

			model.NewsLetterSubscriptions = new GridModel<NewsLetterSubscriptionModel>
			{
				Data = newsletterSubscriptions.Select(x =>
				{
					var m = x.ToModel();
					m.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
					return m;
				}),
				Total = newsletterSubscriptions.Count()
			};

			return new JsonResult
			{
				Data = model
			};
		}

		[HttpPost, ActionName("List")]
		[FormValueRequired("search-newsLetterSubscriptions")]
		public ActionResult Search(NewsLetterSubscriptionListModel model)
		{
			ViewData["searchCustomerEmail"] = model.SearchEmail;

			var newsletterSubscriptions = _newsLetterSubscriptionService.GetAllNewsLetterSubscriptions(model.SearchEmail, true);

			model.NewsLetterSubscriptions = new GridModel<NewsLetterSubscriptionModel>
			{
				Data = newsletterSubscriptions.Select(x =>
				{
					var m = x.ToModel();
					m.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
					return m;
				}),
				Total = newsletterSubscriptions.Count()
			};

			return View(model);
		}

		public ActionResult ExportCsv(NewsLetterSubscriptionListModel model)
		{
			string fileName = String.Format("newsletter_emails_{0}_{1}.txt", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), CommonHelper.GenerateRandomDigitCode(4));

			var sb = new StringBuilder();
			var newsLetterSubscriptions = _newsLetterSubscriptionService.GetAllNewsLetterSubscriptions(model.SearchEmail, true);
			if (newsLetterSubscriptions.Count == 0)
			{
				throw new NopException("No emails to export");
			}
			for (int i = 0; i < newsLetterSubscriptions.Count; i++)
			{
				var subscription = newsLetterSubscriptions[i];
				sb.Append(subscription.Email);
                sb.Append("\t");
                sb.Append(subscription.Active);
                sb.Append("\r\n");  //new line
			}
			string result = sb.ToString();

			return File(Encoding.UTF8.GetBytes(result), "text/csv", fileName);
		}

		[HttpPost]
		public ActionResult ImportCsv(FormCollection form)
		{
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
							string[] tmp = line.Split('\t');

							if (tmp.Length == 2)
							{
								string email = tmp[0].Trim();
								bool isActive = Boolean.Parse(tmp[1]);

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
						}
						SuccessNotification(
							String.Format(
								_localizationService.GetResource(
                                    "Admin.Promotions.NewsLetterSubscriptions.ImportEmailsSuccess"), count))
									;
						return RedirectToAction("List");
					}
				}
				ErrorNotification("Please upload a file");
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
