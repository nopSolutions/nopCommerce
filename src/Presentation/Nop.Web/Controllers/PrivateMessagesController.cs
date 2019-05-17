using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Services.Customers;
using Nop.Services.Forums;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;
using Nop.Web.Models.PrivateMessages;

namespace Nop.Web.Controllers
{
    [HttpsRequirement(SslRequirement.Yes)]
    public partial class PrivateMessagesController : BasePublicController
    {
        #region Fields

        private readonly ForumSettings _forumSettings;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerService _customerService;
        private readonly IForumService _forumService;
        private readonly ILocalizationService _localizationService;
        private readonly IPrivateMessagesModelFactory _privateMessagesModelFactory;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public PrivateMessagesController(ForumSettings forumSettings,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IForumService forumService,
            ILocalizationService localizationService,
            IPrivateMessagesModelFactory privateMessagesModelFactory,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _forumSettings = forumSettings;
            _customerActivityService = customerActivityService;
            _customerService = customerService;
            _forumService = forumService;
            _localizationService = localizationService;
            _privateMessagesModelFactory = privateMessagesModelFactory;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        #endregion
        
        #region Methods

        public virtual IActionResult Index(int? pageNumber, string tab)
        {
            if (!_forumSettings.AllowPrivateMessages)
            {
                return RedirectToRoute("Homepage");
            }

            if (_workContext.CurrentCustomer.IsGuest())
            {
                return Challenge();
            }

            var model = _privateMessagesModelFactory.PreparePrivateMessageIndexModel(pageNumber, tab);
            return View(model);
        }
        
        [HttpPost, FormValueRequired("delete-inbox"), ActionName("InboxUpdate")]
        [PublicAntiForgery]
        public virtual IActionResult DeleteInboxPM(IFormCollection formCollection)
        {
            foreach (var key in formCollection.Keys)
            {
                var value = formCollection[key];

                if (value.Equals("on") && key.StartsWith("pm", StringComparison.InvariantCultureIgnoreCase))
                {
                    var id = key.Replace("pm", "").Trim();
                    if (int.TryParse(id, out int privateMessageId))
                    {
                        var pm = _forumService.GetPrivateMessageById(privateMessageId);
                        if (pm != null)
                        {
                            if (pm.ToCustomerId == _workContext.CurrentCustomer.Id)
                            {
                                pm.IsDeletedByRecipient = true;
                                _forumService.UpdatePrivateMessage(pm);
                            }
                        }
                    }
                }
            }
            return RedirectToRoute("PrivateMessages");
        }

        [HttpPost, FormValueRequired("mark-unread"), ActionName("InboxUpdate")]
        [PublicAntiForgery]
        public virtual IActionResult MarkUnread(IFormCollection formCollection)
        {
            foreach (var key in formCollection.Keys)
            {
                var value = formCollection[key];

                if (value.Equals("on") && key.StartsWith("pm", StringComparison.InvariantCultureIgnoreCase))
                {
                    var id = key.Replace("pm", "").Trim();
                    if (int.TryParse(id, out int privateMessageId))
                    {
                        var pm = _forumService.GetPrivateMessageById(privateMessageId);
                        if (pm != null)
                        {
                            if (pm.ToCustomerId == _workContext.CurrentCustomer.Id)
                            {
                                pm.IsRead = false;
                                _forumService.UpdatePrivateMessage(pm);
                            }
                        }
                    }
                }
            }
            return RedirectToRoute("PrivateMessages");
        }

        //updates sent items (deletes PrivateMessages)
        [HttpPost, FormValueRequired("delete-sent"), ActionName("SentUpdate")]
        [PublicAntiForgery]
        public virtual IActionResult DeleteSentPM(IFormCollection formCollection)
        {
            foreach (var key in formCollection.Keys)
            {
                var value = formCollection[key];

                if (value.Equals("on") && key.StartsWith("si", StringComparison.InvariantCultureIgnoreCase))
                {
                    var id = key.Replace("si", "").Trim();
                    if (int.TryParse(id, out int privateMessageId))
                    {
                        var pm = _forumService.GetPrivateMessageById(privateMessageId);
                        if (pm != null)
                        {
                            if (pm.FromCustomerId == _workContext.CurrentCustomer.Id)
                            {
                                pm.IsDeletedByAuthor = true;
                                _forumService.UpdatePrivateMessage(pm);
                            }
                        }
                    }
                }
            }
            return RedirectToRoute("PrivateMessages", new {tab = "sent"});
        }

        public virtual IActionResult SendPM(int toCustomerId, int? replyToMessageId)
        {
            if (!_forumSettings.AllowPrivateMessages)
                return RedirectToRoute("Homepage");

            if (_workContext.CurrentCustomer.IsGuest())
                return Challenge();

            var customerTo = _customerService.GetCustomerById(toCustomerId);
            if (customerTo == null || customerTo.IsGuest())
                return RedirectToRoute("PrivateMessages");

            PrivateMessage replyToPM = null;
            if (replyToMessageId.HasValue)
            {
                //reply to a previous PM
                replyToPM = _forumService.GetPrivateMessageById(replyToMessageId.Value);
            }

            var model = _privateMessagesModelFactory.PrepareSendPrivateMessageModel(customerTo, replyToPM);
            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        public virtual IActionResult SendPM(SendPrivateMessageModel model)
        {
            if (!_forumSettings.AllowPrivateMessages)
            {
                return RedirectToRoute("Homepage");
            }

            if (_workContext.CurrentCustomer.IsGuest())
            {
                return Challenge();
            }

            Customer toCustomer;
            var replyToPM = _forumService.GetPrivateMessageById(model.ReplyToMessageId);
            if (replyToPM != null)
            {
                //reply to a previous PM
                if (replyToPM.ToCustomerId == _workContext.CurrentCustomer.Id || replyToPM.FromCustomerId == _workContext.CurrentCustomer.Id)
                {
                    //Reply to already sent PM (by current customer) should not be sent to yourself
                    toCustomer = replyToPM.FromCustomerId == _workContext.CurrentCustomer.Id
                        ? replyToPM.ToCustomer
                        : replyToPM.FromCustomer;
                }
                else
                {
                    return RedirectToRoute("PrivateMessages");
                }
            }
            else
            {
                //first PM
                toCustomer = _customerService.GetCustomerById(model.ToCustomerId);
            }

            if (toCustomer == null || toCustomer.IsGuest())
            {
                return RedirectToRoute("PrivateMessages");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var subject = model.Subject;
                    if (_forumSettings.PMSubjectMaxLength > 0 && subject.Length > _forumSettings.PMSubjectMaxLength)
                    {
                        subject = subject.Substring(0, _forumSettings.PMSubjectMaxLength);
                    }

                    var text = model.Message;
                    if (_forumSettings.PMTextMaxLength > 0 && text.Length > _forumSettings.PMTextMaxLength)
                    {
                        text = text.Substring(0, _forumSettings.PMTextMaxLength);
                    }

                    var nowUtc = DateTime.UtcNow;

                    var privateMessage = new PrivateMessage
                    {
                        StoreId = _storeContext.CurrentStore.Id,
                        ToCustomerId = toCustomer.Id,
                        FromCustomerId = _workContext.CurrentCustomer.Id,
                        Subject = subject,
                        Text = text,
                        IsDeletedByAuthor = false,
                        IsDeletedByRecipient = false,
                        IsRead = false,
                        CreatedOnUtc = nowUtc
                    };

                    _forumService.InsertPrivateMessage(privateMessage);

                    //activity log
                    _customerActivityService.InsertActivity("PublicStore.SendPM",
                        string.Format(_localizationService.GetResource("ActivityLog.PublicStore.SendPM"), toCustomer.Email), toCustomer);

                    return RedirectToRoute("PrivateMessages", new { tab = "sent" });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            model = _privateMessagesModelFactory.PrepareSendPrivateMessageModel(toCustomer, replyToPM);
            return View(model);
        }

        public virtual IActionResult ViewPM(int privateMessageId)
        {
            if (!_forumSettings.AllowPrivateMessages)
            {
                return RedirectToRoute("Homepage");
            }

            if (_workContext.CurrentCustomer.IsGuest())
            {
                return Challenge();
            }

            var pm = _forumService.GetPrivateMessageById(privateMessageId);
            if (pm != null)
            {
                if (pm.ToCustomerId != _workContext.CurrentCustomer.Id && pm.FromCustomerId != _workContext.CurrentCustomer.Id)
                {
                    return RedirectToRoute("PrivateMessages");
                }

                if (!pm.IsRead && pm.ToCustomerId == _workContext.CurrentCustomer.Id)
                {
                    pm.IsRead = true;
                    _forumService.UpdatePrivateMessage(pm);
                }
            }
            else
            {
                return RedirectToRoute("PrivateMessages");
            }

            var model = _privateMessagesModelFactory.PreparePrivateMessageModel(pm);
            return View(model);
        }

        public virtual IActionResult DeletePM(int privateMessageId)
        {
            if (!_forumSettings.AllowPrivateMessages)
            {
                return RedirectToRoute("Homepage");
            }

            if (_workContext.CurrentCustomer.IsGuest())
            {
                return Challenge();
            }

            var pm = _forumService.GetPrivateMessageById(privateMessageId);
            if (pm != null)
            {
                if (pm.FromCustomerId == _workContext.CurrentCustomer.Id)
                {
                    pm.IsDeletedByAuthor = true;
                    _forumService.UpdatePrivateMessage(pm);
                }

                if (pm.ToCustomerId == _workContext.CurrentCustomer.Id)
                {
                    pm.IsDeletedByRecipient = true;
                    _forumService.UpdatePrivateMessage(pm);
                }
            }
            return RedirectToRoute("PrivateMessages");
        }

        #endregion
    }
}