using System;
using System.Linq;
using System.Web.Mvc;

using Nop.Admin.Models;
using Nop.Core.Domain.Messages;
using Nop.Services.Helpers;
using Nop.Services.Messages;
using Nop.Web.Framework;
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
            ViewData["SearchStartDate"] = model.SearchStartDate;
            ViewData["SearchEndDate"] = model.SearchEndDate;
            ViewData["SearchFromEmail"] = model.SearchFromEmail;
            ViewData["SearchToEmail"] = model.SearchToEmail;
            ViewData["SearchLoadNotSent"] = model.SearchLoadNotSent;
            ViewData["SearchMaxSentTries"] = model.SearchMaxSentTries;

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
		public ActionResult QueuedEmailList(GridCommand command)
		{
            //filtering
            string startDate = command.FilterDescriptors.GetValueFromAppliedFilters("SearchStartDate");
            string endDate = command.FilterDescriptors.GetValueFromAppliedFilters("SearchEndDate");
            string fromEmail = command.FilterDescriptors.GetValueFromAppliedFilters("From");
            string toEmail = command.FilterDescriptors.GetValueFromAppliedFilters("To");
            string loadNotSent = command.FilterDescriptors.GetValueFromAppliedFilters("SearchNotSentOnly");
            string maxSentTries = command.FilterDescriptors.GetValueFromAppliedFilters("SentTries");

            DateTime? startDateValue = null;
            if(String.IsNullOrEmpty(startDate))
                startDateValue = null;
            else
                startDateValue = _dateTimeHelper.ConvertToUtcTime(Convert.ToDateTime(startDate), _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = null;
            if(String.IsNullOrEmpty(endDate))
                endDateValue = null;
            else
                endDateValue = _dateTimeHelper.ConvertToUtcTime(Convert.ToDateTime(endDate), _dateTimeHelper.CurrentTimeZone);

            var model = new GridModel();

            var queuedEmails = _queuedEmailService.SearchEmails(fromEmail, toEmail, startDateValue, endDateValue,
               0, Convert.ToBoolean(loadNotSent),Convert.ToInt32(maxSentTries), 0, 10);
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
