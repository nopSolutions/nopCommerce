using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Messages;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class QueuedEmailController : BaseAdminController
    {
        #region Fields

        protected readonly IDateTimeHelper _dateTimeHelper;
        protected readonly ILocalizationService _localizationService;
        protected readonly INotificationService _notificationService;
        protected readonly IPermissionService _permissionService;
        protected readonly IQueuedEmailModelFactory _queuedEmailModelFactory;
        protected readonly IQueuedEmailService _queuedEmailService;

        #endregion

        #region Ctor

        public QueuedEmailController(IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IQueuedEmailModelFactory queuedEmailModelFactory,
            IQueuedEmailService queuedEmailService)
        {
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _queuedEmailModelFactory = queuedEmailModelFactory;
            _queuedEmailService = queuedEmailService;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMessageQueue))
                return AccessDeniedView();

            //prepare model
            var model = await _queuedEmailModelFactory.PrepareQueuedEmailSearchModelAsync(new QueuedEmailSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> QueuedEmailList(QueuedEmailSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMessageQueue))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _queuedEmailModelFactory.PrepareQueuedEmailListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("go-to-email-by-number")]
        public virtual async Task<IActionResult> GoToEmailByNumber(QueuedEmailSearchModel model)
        {
            //try to get a queued email with the specified id
            var queuedEmail = await _queuedEmailService.GetQueuedEmailByIdAsync(model.GoDirectlyToNumber);
            if (queuedEmail == null)
                return await List();

            return RedirectToAction("Edit", "QueuedEmail", new { id = queuedEmail.Id });
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMessageQueue))
                return AccessDeniedView();

            //try to get a queued email with the specified id
            var email = await _queuedEmailService.GetQueuedEmailByIdAsync(id);
            if (email == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _queuedEmailModelFactory.PrepareQueuedEmailModelAsync(null, email);

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> Edit(QueuedEmailModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMessageQueue))
                return AccessDeniedView();

            //try to get a queued email with the specified id
            var email = await _queuedEmailService.GetQueuedEmailByIdAsync(model.Id);
            if (email == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                email = model.ToEntity(email);
                email.DontSendBeforeDateUtc = model.SendImmediately || !model.DontSendBeforeDate.HasValue ?
                    null : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DontSendBeforeDate.Value);
                await _queuedEmailService.UpdateQueuedEmailAsync(email);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.System.QueuedEmails.Updated"));

                return continueEditing ? RedirectToAction("Edit", new { id = email.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = await _queuedEmailModelFactory.PrepareQueuedEmailModelAsync(model, email, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost, ActionName("Edit"), FormValueRequired("requeue")]
        public virtual async Task<IActionResult> Requeue(QueuedEmailModel queuedEmailModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMessageQueue))
                return AccessDeniedView();

            //try to get a queued email with the specified id
            var queuedEmail = await _queuedEmailService.GetQueuedEmailByIdAsync(queuedEmailModel.Id);
            if (queuedEmail == null)
                return RedirectToAction("List");

            var requeuedEmail = new QueuedEmail
            {
                PriorityId = queuedEmail.PriorityId,
                From = queuedEmail.From,
                FromName = queuedEmail.FromName,
                To = queuedEmail.To,
                ToName = queuedEmail.ToName,
                ReplyTo = queuedEmail.ReplyTo,
                ReplyToName = queuedEmail.ReplyToName,
                CC = queuedEmail.CC,
                Bcc = queuedEmail.Bcc,
                Subject = queuedEmail.Subject,
                Body = queuedEmail.Body,
                AttachmentFilePath = queuedEmail.AttachmentFilePath,
                AttachmentFileName = queuedEmail.AttachmentFileName,
                AttachedDownloadId = queuedEmail.AttachedDownloadId,
                CreatedOnUtc = DateTime.UtcNow,
                EmailAccountId = queuedEmail.EmailAccountId,
                DontSendBeforeDateUtc = queuedEmailModel.SendImmediately || !queuedEmailModel.DontSendBeforeDate.HasValue ?
                    null : (DateTime?)_dateTimeHelper.ConvertToUtcTime(queuedEmailModel.DontSendBeforeDate.Value)
            };
            await _queuedEmailService.InsertQueuedEmailAsync(requeuedEmail);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.System.QueuedEmails.Requeued"));

            return RedirectToAction("Edit", new { id = requeuedEmail.Id });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMessageQueue))
                return AccessDeniedView();

            //try to get a queued email with the specified id
            var email = await _queuedEmailService.GetQueuedEmailByIdAsync(id);
            if (email == null)
                return RedirectToAction("List");

            await _queuedEmailService.DeleteQueuedEmailAsync(email);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.System.QueuedEmails.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMessageQueue))
                return AccessDeniedView();

            if (selectedIds == null || selectedIds.Count == 0)
                return NoContent();

            await _queuedEmailService.DeleteQueuedEmailsAsync(await _queuedEmailService.GetQueuedEmailsByIdsAsync(selectedIds.ToArray()));

            return Json(new { Result = true });
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("delete-all")]
        public virtual async Task<IActionResult> DeleteAll()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMessageQueue))
                return AccessDeniedView();

            await _queuedEmailService.DeleteAllEmailsAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.System.QueuedEmails.DeletedAll"));

            return RedirectToAction("List");
        }

        #endregion
    }
}