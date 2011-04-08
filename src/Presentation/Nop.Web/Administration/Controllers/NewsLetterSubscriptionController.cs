using System;
using System.Linq;
using System.Web.Mvc;

using Nop.Admin.Models;
using Nop.Services.Messages;
using Nop.Web.Framework.Controllers;

using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
	[AdminAuthorize]
	public class NewsLetterSubscriptionController : BaseNopController
	{
		private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;

        public NewsLetterSubscriptionController(INewsLetterSubscriptionService newsLetterSubscriptionService)
		{
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
		}

		public ActionResult Index()
		{
			return View("List");
		}

		public ActionResult List()
		{
            var newsletterSubscriptions = _newsLetterSubscriptionService.GetAllNewsLetterSubscriptions(String.Empty);
			var gridModel = new GridModel<NewsLetterSubscriptionModel>
			{
				Data = newsletterSubscriptions.Select(x => x.ToModel()),
				Total = newsletterSubscriptions.Count()
			};
			return View(gridModel);
		}

		[HttpPost, GridAction(EnableCustomBinding = true)]
		public ActionResult List(GridCommand command)
		{
            var newsletterSubscriptions = _newsLetterSubscriptionService.GetAllNewsLetterSubscriptions(String.Empty);
            var gridModel = new GridModel<NewsLetterSubscriptionModel>
            {
                Data = newsletterSubscriptions.Select(x => x.ToModel()),
                Total = newsletterSubscriptions.Count()
            };
            return new JsonResult
			{
				Data = gridModel
			};
		}

	}
}
