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
using Nop.Web.Models.PrivateMessages;

namespace Nop.Web.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class PrivateMessagesController : BasePublicController
    {
        #region Fields

        protected readonly ForumSettings _forumSettings;
        protected readonly ICustomerActivityService _customerActivityService;
        protected readonly ICustomerService _customerService;
        protected readonly IForumService _forumService;
        protected readonly ILocalizationService _localizationService;
        protected readonly IPrivateMessagesModelFactory _privateMessagesModelFactory;
        protected readonly IStoreContext _storeContext;
        protected readonly IWorkContext _workContext;

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

        public virtual async Task<IActionResult> Index(int? pageNumber, string tab)
        {
            if (!_forumSettings.AllowPrivateMessages)
            {
                return RedirectToRoute("Homepage");
            }

            if (await _customerService.IsGuestAsync(await _workContext.GetCurrentCustomerAsync()))
            {
                return Challenge();
            }

            var model = await _privateMessagesModelFactory.PreparePrivateMessageIndexModelAsync(pageNumber, tab);
            return View(model);
        }

        [HttpPost, FormValueRequired("delete-inbox"), ActionName("InboxUpdate")]
        public virtual async Task<IActionResult> DeleteInboxPM(IFormCollection formCollection)
        {
            foreach (var key in formCollection.Keys)
            {
                var value = formCollection[key];

                if (value.Equals("on") && key.StartsWith("pm", StringComparison.InvariantCultureIgnoreCase))
                {
                    var id = key.Replace("pm", "").Trim();
                    if (int.TryParse(id, out var privateMessageId))
                    {
                        var pm = await _forumService.GetPrivateMessageByIdAsync(privateMessageId);
                        if (pm != null)
                        {
                            var customer = await _workContext.GetCurrentCustomerAsync();

                            if (pm.ToCustomerId == customer.Id)
                            {
                                pm.IsDeletedByRecipient = true;
                                await _forumService.UpdatePrivateMessageAsync(pm);
                            }
                        }
                    }
                }
            }
            return RedirectToRoute("PrivateMessages");
        }

        [HttpPost, FormValueRequired("mark-unread"), ActionName("InboxUpdate")]
        public virtual async Task<IActionResult> MarkUnread(IFormCollection formCollection)
        {
            foreach (var key in formCollection.Keys)
            {
                var value = formCollection[key];

                if (value.Equals("on") && key.StartsWith("pm", StringComparison.InvariantCultureIgnoreCase))
                {
                    var id = key.Replace("pm", "").Trim();
                    if (int.TryParse(id, out var privateMessageId))
                    {
                        var pm = await _forumService.GetPrivateMessageByIdAsync(privateMessageId);
                        if (pm != null)
                        {
                            var customer = await _workContext.GetCurrentCustomerAsync();

                            if (pm.ToCustomerId == customer.Id)
                            {
                                pm.IsRead = false;
                                await _forumService.UpdatePrivateMessageAsync(pm);
                            }
                        }
                    }
                }
            }
            return RedirectToRoute("PrivateMessages");
        }

        //updates sent items (deletes PrivateMessages)
        [HttpPost, FormValueRequired("delete-sent"), ActionName("SentUpdate")]
        public virtual async Task<IActionResult> DeleteSentPM(IFormCollection formCollection)
        {
            foreach (var key in formCollection.Keys)
            {
                var value = formCollection[key];

                if (value.Equals("on") && key.StartsWith("si", StringComparison.InvariantCultureIgnoreCase))
                {
                    var id = key.Replace("si", "").Trim();
                    if (int.TryParse(id, out var privateMessageId))
                    {
                        var pm = await _forumService.GetPrivateMessageByIdAsync(privateMessageId);
                        if (pm != null)
                        {
                            var customer = await _workContext.GetCurrentCustomerAsync();

                            if (pm.FromCustomerId == customer.Id)
                            {
                                pm.IsDeletedByAuthor = true;
                                await _forumService.UpdatePrivateMessageAsync(pm);
                            }
                        }
                    }
                }
            }
            return RedirectToRoute("PrivateMessages", new { tab = "sent" });
        }

        public virtual async Task<IActionResult> SendPM(int toCustomerId, int? replyToMessageId)
        {
            if (!_forumSettings.AllowPrivateMessages)
                return RedirectToRoute("Homepage");

            if (await _customerService.IsGuestAsync(await _workContext.GetCurrentCustomerAsync()))
                return Challenge();

            var customerTo = await _customerService.GetCustomerByIdAsync(toCustomerId);
            if (customerTo == null || await _customerService.IsGuestAsync(customerTo))
                return RedirectToRoute("PrivateMessages");

            PrivateMessage replyToPM = null;
            if (replyToMessageId.HasValue)
            {
                //reply to a previous PM
                replyToPM = await _forumService.GetPrivateMessageByIdAsync(replyToMessageId.Value);
            }

            var model = await _privateMessagesModelFactory.PrepareSendPrivateMessageModelAsync(customerTo, replyToPM);
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> SendPM(SendPrivateMessageModel model)
        {
            if (!_forumSettings.AllowPrivateMessages)
            {
                return RedirectToRoute("Homepage");
            }

            var customer = await _workContext.GetCurrentCustomerAsync();
            if (await _customerService.IsGuestAsync(customer))
            {
                return Challenge();
            }

            Customer toCustomer;
            var replyToPM = await _forumService.GetPrivateMessageByIdAsync(model.ReplyToMessageId);
            if (replyToPM != null)
            {
                //reply to a previous PM
                if (replyToPM.ToCustomerId == customer.Id || replyToPM.FromCustomerId == customer.Id)
                {
                    //Reply to already sent PM (by current customer) should not be sent to yourself
                    toCustomer = await _customerService.GetCustomerByIdAsync(replyToPM.FromCustomerId == customer.Id
                        ? replyToPM.ToCustomerId
                        : replyToPM.FromCustomerId);
                }
                else
                {
                    return RedirectToRoute("PrivateMessages");
                }
            }
            else
            {
                //first PM
                toCustomer = await _customerService.GetCustomerByIdAsync(model.ToCustomerId);
            }

            if (toCustomer == null || await _customerService.IsGuestAsync(toCustomer))
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
                        subject = subject[0.._forumSettings.PMSubjectMaxLength];
                    }

                    var text = model.Message;
                    if (_forumSettings.PMTextMaxLength > 0 && text.Length > _forumSettings.PMTextMaxLength)
                    {
                        text = text[0.._forumSettings.PMTextMaxLength];
                    }

                    var nowUtc = DateTime.UtcNow;
                    var store = await _storeContext.GetCurrentStoreAsync();

                    var privateMessage = new PrivateMessage
                    {
                        StoreId = store.Id,
                        ToCustomerId = toCustomer.Id,
                        FromCustomerId = customer.Id,
                        Subject = subject,
                        Text = text,
                        IsDeletedByAuthor = false,
                        IsDeletedByRecipient = false,
                        IsRead = false,
                        CreatedOnUtc = nowUtc
                    };

                    await _forumService.InsertPrivateMessageAsync(privateMessage);

                    //activity log
                    await _customerActivityService.InsertActivityAsync("PublicStore.SendPM",
                        string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.SendPM"), toCustomer.Email), toCustomer);

                    return RedirectToRoute("PrivateMessages", new { tab = "sent" });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            model = await _privateMessagesModelFactory.PrepareSendPrivateMessageModelAsync(toCustomer, replyToPM);
            return View(model);
        }

        public virtual async Task<IActionResult> ViewPM(int privateMessageId)
        {
            if (!_forumSettings.AllowPrivateMessages)
            {
                return RedirectToRoute("Homepage");
            }

            var customer = await _workContext.GetCurrentCustomerAsync();
            if (await _customerService.IsGuestAsync(customer))
            {
                return Challenge();
            }

            var pm = await _forumService.GetPrivateMessageByIdAsync(privateMessageId);
            if (pm != null)
            {
                if (pm.ToCustomerId != customer.Id && pm.FromCustomerId != customer.Id)
                {
                    return RedirectToRoute("PrivateMessages");
                }

                if (!pm.IsRead && pm.ToCustomerId == customer.Id)
                {
                    pm.IsRead = true;
                    await _forumService.UpdatePrivateMessageAsync(pm);
                }
            }
            else
            {
                return RedirectToRoute("PrivateMessages");
            }

            var model = await _privateMessagesModelFactory.PreparePrivateMessageModelAsync(pm);
            return View(model);
        }

        public virtual async Task<IActionResult> DeletePM(int privateMessageId)
        {
            if (!_forumSettings.AllowPrivateMessages)
            {
                return RedirectToRoute("Homepage");
            }

            var customer = await _workContext.GetCurrentCustomerAsync();
            if (await _customerService.IsGuestAsync(customer))
            {
                return Challenge();
            }

            var pm = await _forumService.GetPrivateMessageByIdAsync(privateMessageId);
            if (pm != null)
            {
                if (pm.FromCustomerId == customer.Id)
                {
                    pm.IsDeletedByAuthor = true;
                    await _forumService.UpdatePrivateMessageAsync(pm);
                }

                if (pm.ToCustomerId == customer.Id)
                {
                    pm.IsDeletedByRecipient = true;
                    await _forumService.UpdatePrivateMessageAsync(pm);
                }
            }
            return RedirectToRoute("PrivateMessages");
        }

        #endregion
    }
}