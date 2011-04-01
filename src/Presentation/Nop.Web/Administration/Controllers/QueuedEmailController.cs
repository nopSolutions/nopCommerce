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
	public class QueuedEmailController : BaseNopController
	{
		private readonly IQueuedEmailService _queuedEmailService;

		public QueuedEmailController(IQueuedEmailService queuedEmailService)
		{
			_queuedEmailService = queuedEmailService;
		}

		public ActionResult Index()
		{
			return View("List");
		}

		public ActionResult List()
		{
			var emails = _queuedEmailService.GetAllQueuedEmails(0, false, 10);
			var gridModel = new GridModel<QueuedEmailModel>
			{
				Data = emails.Select(x => x.ToModel()),
				Total = emails.Count()
			};
			return View(gridModel);
		}

		[HttpPost, GridAction(EnableCustomBinding = true)]
		public ActionResult List(GridCommand command)
		{
			var emails = _queuedEmailService.GetAllQueuedEmails(0, false, 0);
			var gridModel = new GridModel<QueuedEmailModel>
			{
				Data = emails.Select(x => x.ToModel()),
				Total = emails.Count()
			};
			return new JsonResult
			{
				Data = gridModel
			};
		}


		public ActionResult Edit(int id)
		{
			var email = _queuedEmailService.GetQueuedEmailById(id);
			if (email == null) throw new ArgumentException("No email found with the specified id", "id");
			return View(email.ToModel());
		}

		[HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
		public ActionResult Edit(QueuedEmailModel queuedEmailModel, bool continueEditing)
		{
			if (!ModelState.IsValid)
			{
				return View(queuedEmailModel);
			}
			var email = _queuedEmailService.GetQueuedEmailById(queuedEmailModel.Id);
			email = queuedEmailModel.ToEntity(email);
			_queuedEmailService.UpdateQueuedEmail(email);

			return continueEditing ? RedirectToAction("Edit", email.Id) : RedirectToAction("List");
		}


		[HttpPost, ActionName("Delete")]
		public ActionResult DeleteConfirmed(int id)
		{
			var email = _queuedEmailService.GetQueuedEmailById(id);
			_queuedEmailService.DeleteQueuedEmail(email);
			return RedirectToAction("List");
		}
	}
}
