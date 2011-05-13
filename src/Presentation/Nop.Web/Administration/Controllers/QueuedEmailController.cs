using System;
using System.Linq;
using System.Web.Mvc;

using Nop.Admin.Models;
using Nop.Core.Domain.Messages;
using Nop.Services.Helpers;
using Nop.Services.Messages;
using Nop.Web.Framework.Controllers;

using Telerik.Web.Mvc;
using System.Collections.Generic;

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
            var model = new QueuedEmailListModel();

            return View(model);
		}

		[GridAction(EnableCustomBinding = true)]
		public ActionResult QueuedEmailList(GridCommand command, QueuedEmailListModel model)
		{
            DateTime? startDateValue = (model.SearchStartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.SearchStartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.SearchEndDate == null) ? null 
                            :(DateTime?)_dateTimeHelper.ConvertToUtcTime(model.SearchEndDate.Value, _dateTimeHelper.CurrentTimeZone);

            var queuedEmails = _queuedEmailService.SearchEmails(model.SearchFromEmail, model.SearchToEmail, startDateValue, endDateValue,
               0, model.SearchLoadNotSent, model.SearchMaxSentTries, command.Page - 1, command.PageSize);
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

        [HttpPost, ActionName("List")]
        [FormValueRequired("go-to-email-by-number")]
        public ActionResult GoToEmailByNumber(QueuedEmailListModel model)
        {
            var queuedEmail = _queuedEmailService.GetQueuedEmailById(model.GoDirectlyToNumber);
            if (queuedEmail != null)
                return RedirectToAction("Edit", "QueuedEmail", new { id = queuedEmail.Id });
            else
                return List();
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

        //TODO: currently, only recored within current page are passed, 
        //  need to somehow pass all of the records
        [HttpPost, ActionName("List")]
        [FormValueRequired("delete-selected")]
        public ActionResult DeleteSelected(QueuedEmailListModel model, ICollection<int> checkedRecords)
        {
            if (checkedRecords != null)
            {
                foreach (var queuedEmailId in checkedRecords)
                {
                    var queuedEmail = _queuedEmailService.GetQueuedEmailById(queuedEmailId);
                    _queuedEmailService.DeleteQueuedEmail(queuedEmail);
                }
            }
            return View(model);
        }
	}
}
