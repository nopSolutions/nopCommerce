#if NET451
using System;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Services.Customers;
using Nop.Services.Forums;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Security;
using Nop.Web.Models.PrivateMessages;

namespace Nop.Web.Controllers
{
    [NopHttpsRequirement(SslRequirement.Yes)]
    public partial class PrivateMessagesController : BasePublicController
    {
        #region Fields

        private readonly IPrivateMessagesModelFactory _privateMessagesModelFactory;
        private readonly IForumService _forumService;
        private readonly ICustomerService _customerService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ForumSettings _forumSettings;

        #endregion

        #region Constructors

        public PrivateMessagesController(IPrivateMessagesModelFactory privateMessagesModelFactory,
            IForumService forumService,
            ICustomerService customerService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IWorkContext workContext, 
            IStoreContext storeContext,
            ForumSettings forumSettings)
        {
            this._privateMessagesModelFactory = privateMessagesModelFactory;
            this._forumService = forumService;
            this._customerService = customerService;
            this._customerActivityService = customerActivityService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._forumSettings = forumSettings;
        }

        #endregion
        
        #region Methods

        public virtual ActionResult Index(int? page, string tab)
        {
            if (!_forumSettings.AllowPrivateMessages)
            {
                return RedirectToRoute("HomePage");
            }

            if (_workContext.CurrentCustomer.IsGuest())
            {
                return new HttpUnauthorizedResult();
            }

            var model = _privateMessagesModelFactory.PreparePrivateMessageIndexModel(page, tab);
            return View(model);
        }

        //inbox tab
        [ChildActionOnly]
        public virtual ActionResult Inbox(int page, string tab)
        {
            var model = _privateMessagesModelFactory.PrepareInboxModel(page, tab);
            return PartialView(model);
        }

        //sent items tab
        [ChildActionOnly]
        public virtual ActionResult SentItems(int page, string tab)
        {
            var model = _privateMessagesModelFactory.PrepareSentModel(page, tab);
            return PartialView(model);
        }

        [HttpPost, FormValueRequired("delete-inbox"), ActionName("InboxUpdate")]
        [PublicAntiForgery]
        public virtual ActionResult DeleteInboxPM(FormCollection formCollection)
        {
            foreach (var key in formCollection.AllKeys)
            {
                var value = formCollection[key];

                if (value.Equals("on") && key.StartsWith("pm", StringComparison.InvariantCultureIgnoreCase))
                {
                    var id = key.Replace("pm", "").Trim();
                    int privateMessageId;
                    if (Int32.TryParse(id, out privateMessageId))
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
        public virtual ActionResult MarkUnread(FormCollection formCollection)
        {
            foreach (var key in formCollection.AllKeys)
            {
                var value = formCollection[key];

                if (value.Equals("on") && key.StartsWith("pm", StringComparison.InvariantCultureIgnoreCase))
                {
                    var id = key.Replace("pm", "").Trim();
                    int privateMessageId;
                    if (Int32.TryParse(id, out privateMessageId))
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
        public virtual ActionResult DeleteSentPM(FormCollection formCollection)
        {
            foreach (var key in formCollection.AllKeys)
            {
                var value = formCollection[key];

                if (value.Equals("on") && key.StartsWith("si", StringComparison.InvariantCultureIgnoreCase))
                {
                    var id = key.Replace("si", "").Trim();
                    int privateMessageId;
                    if (Int32.TryParse(id, out privateMessageId))
                    {
                        PrivateMessage pm = _forumService.GetPrivateMessageById(privateMessageId);
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

        public virtual ActionResult SendPM(int toCustomerId, int? replyToMessageId)
        {
            if (!_forumSettings.AllowPrivateMessages)
                return RedirectToRoute("HomePage");

            if (_workContext.CurrentCustomer.IsGuest())
                return new HttpUnauthorizedResult();

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
        public virtual ActionResult SendPM(SendPrivateMessageModel model)
        {
            if (!_forumSettings.AllowPrivateMessages)
            {
                return RedirectToRoute("HomePage");
            }

            if (_workContext.CurrentCustomer.IsGuest())
            {
                return new HttpUnauthorizedResult();
            }

            Customer toCustomer = null;
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
                    string subject = model.Subject;
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
                    _customerActivityService.InsertActivity("PublicStore.SendPM", _localizationService.GetResource("ActivityLog.PublicStore.SendPM"), toCustomer.Email);

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

        public virtual ActionResult ViewPM(int privateMessageId)
        {
            if (!_forumSettings.AllowPrivateMessages)
            {
                return RedirectToRoute("HomePage");
            }

            if (_workContext.CurrentCustomer.IsGuest())
            {
                return new HttpUnauthorizedResult();
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

        public virtual ActionResult DeletePM(int privateMessageId)
        {
            if (!_forumSettings.AllowPrivateMessages)
            {
                return RedirectToRoute("HomePage");
            }

            if (_workContext.CurrentCustomer.IsGuest())
            {
                return new HttpUnauthorizedResult();
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
#endif