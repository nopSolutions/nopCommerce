using System;
using System.Linq;
using System.Web.Mvc;

using Nop.Admin.Models;
using Nop.Core.Domain.Messages;
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

        [HttpPost, ActionName("Edit"), FormValueRequired("save")]
        public ActionResult Save(QueuedEmailModel queuedEmailModel)
		{
			if (!ModelState.IsValid)
			{
				return View(queuedEmailModel);
			}

            Update(queuedEmailModel);

            return RedirectToAction("List");
		}

        [HttpPost, ActionName("Edit"), FormValueRequired("save-continue")]
        public ActionResult SaveAndContinue(QueuedEmailModel queuedEmailModel)
        {
            if (!ModelState.IsValid)
            {
                return View(queuedEmailModel);
            }

            Update(queuedEmailModel);

            return RedirectToAction("Edit", queuedEmailModel.Id);
        }

        [HttpPost, ActionName("Edit"), FormValueRequired("requeue")]
        public ActionResult Requeue(QueuedEmailModel queuedEmailModel)
        {
            var queuedEmail = _queuedEmailService.GetQueuedEmailById(queuedEmailModel.Id);
            if (queuedEmail != null)
            {
                var requeuedEmail = new QueuedEmail()
                {
                    Priority = queuedEmail.Priority,
                    From = queuedEmail.From,
                    FromName = queuedEmail.FromName,
                    To = queuedEmail.To,
                    ToName = queuedEmail.ToName,
                    CC = queuedEmail.CC,
                    Bcc = queuedEmail.Bcc,
                    Subject = queuedEmail.Subject,
                    Body = queuedEmail.Body,
                    CreatedOnUtc = DateTime.UtcNow,
                    EmailAccountId = queuedEmail.EmailAccountId
                };
                _queuedEmailService.InsertQueuedEmail(requeuedEmail);
                return RedirectToAction("Edit", requeuedEmail.Id);
            }
            else
                return RedirectToAction("List");
        }

        private void Update(QueuedEmailModel queuedEmailModel)
        {
            var email = _queuedEmailService.GetQueuedEmailById(queuedEmailModel.Id);
            email = queuedEmailModel.ToEntity(email);
            _queuedEmailService.UpdateQueuedEmail(email);
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
