using System;
using System.Linq;
using System.Web.Mvc;

using Nop.Admin.Models;
using Nop.Core.Domain.Messages;
using Nop.Services.Helpers;
using Nop.Services.Messages;
using Nop.Web.Framework.Controllers;

using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
	[AdminAuthorize]
	public class QueuedEmailController : BaseNopController
	{
		private readonly IQueuedEmailService _queuedEmailService;
        private readonly IDateTimeHelper _dateTimeHelper;

		public QueuedEmailController(IQueuedEmailService queuedEmailService, IDateTimeHelper dateTimeHelper)
		{
			_queuedEmailService = queuedEmailService;
            _dateTimeHelper = dateTimeHelper;
		}

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

		public ActionResult List()
		{
            var queuedEmails = _queuedEmailService.SearchEmails(null, null, null, null, 0, false, 10, 0, 10);

            var model = new QueuedEmailListModel();
            model.QueuedEmails = new GridModel<QueuedEmailModel>
            {
                Data = queuedEmails.Select(x => x.ToModel()),
                Total = queuedEmails.TotalCount
            };

            return View(model);
		}

        [HttpPost, ActionName("List")]
        [FormValueRequired("search-emails")]
        public ActionResult Search(QueuedEmailListModel model)
        {            
            var queuedEmails = _queuedEmailService.SearchEmails(model.SearchFromEmail, model.SearchToEmail,
                model.SearchStartDate, model.SearchEndDate, 0, model.SearchLoadNotSent, model.SearchMaxSentTries, 0, 10);

            model.QueuedEmails = new GridModel<QueuedEmailModel>
            {
                Data = queuedEmails.Select(x => x.ToModel()),
                Total = queuedEmails.TotalCount
            };

            return View(model);
        }

		[HttpPost, GridAction(EnableCustomBinding = true)]
		public ActionResult QueuedEmailList(GridCommand command, QueuedEmailListModel model)
		{
            DateTime? startDateValue = (model.SearchStartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.SearchStartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.SearchEndDate == null) ? null 
                            :(DateTime?)_dateTimeHelper.ConvertToUtcTime(model.SearchEndDate.Value, _dateTimeHelper.CurrentTimeZone);

            var newModel = new GridModel();

            var queuedEmails = _queuedEmailService.SearchEmails(model.SearchFromEmail, model.SearchToEmail, startDateValue, endDateValue,
               0, model.SearchLoadNotSent, model.SearchMaxSentTries, 0, 10);
			var gridModel = new GridModel<QueuedEmailModel>
			{
				Data = queuedEmails.Select(x => x.ToModel()),
				Total = queuedEmails.TotalCount
			};
			return new JsonResult
			{
				Data = gridModel
			};
		}

		public ActionResult Edit(int id)
		{
			var email = _queuedEmailService.GetQueuedEmailById(id);
			if (email == null) 
                throw new ArgumentException("No email found with the specified id", "id");
			return View(email.ToModel());
		}

        [HttpPost, ActionName("Edit"), FormValueRequired("save")]
        public ActionResult Save(QueuedEmailModel model)
		{
            if (ModelState.IsValid)
            {
                Update(model);
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
		}

        [HttpPost, ActionName("Edit"), FormValueRequired("save-continue")]
        public ActionResult SaveAndContinue(QueuedEmailModel model)
        {
            if (ModelState.IsValid)
            {
                Update(model);
                return RedirectToAction("Edit", model.Id);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
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
