using System;
using System.Threading.Tasks;
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
using Nop.Web.Models.PrivateMessages;

namespace Nop.Web.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class PrivateMessagesController : BasePublicController
    {
        #region Fields

        protected ForumSettings ForumSettings { get; }
        protected ICustomerActivityService CustomerActivityService { get; }
        protected ICustomerService CustomerService { get; }
        protected IForumService ForumService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected IPrivateMessagesModelFactory PrivateMessagesModelFactory { get; }
        protected IStoreContext StoreContext { get; }
        protected IWorkContext WorkContext { get; }

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
            ForumSettings = forumSettings;
            CustomerActivityService = customerActivityService;
            CustomerService = customerService;
            ForumService = forumService;
            LocalizationService = localizationService;
            PrivateMessagesModelFactory = privateMessagesModelFactory;
            StoreContext = storeContext;
            WorkContext = workContext;
        }

        #endregion
        
        #region Methods

        public virtual async Task<IActionResult> Index(int? pageNumber, string tab)
        {
            if (!ForumSettings.AllowPrivateMessages)
            {
                return RedirectToRoute("Homepage");
            }

            if (await CustomerService.IsGuestAsync(await WorkContext.GetCurrentCustomerAsync()))
            {
                return Challenge();
            }

            var model = await PrivateMessagesModelFactory.PreparePrivateMessageIndexModelAsync(pageNumber, tab);
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
                        var pm = await ForumService.GetPrivateMessageByIdAsync(privateMessageId);
                        if (pm != null)
                        {
                            var customer = await WorkContext.GetCurrentCustomerAsync();

                            if (pm.ToCustomerId == customer.Id)
                            {
                                pm.IsDeletedByRecipient = true;
                                await ForumService.UpdatePrivateMessageAsync(pm);
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
                        var pm = await ForumService.GetPrivateMessageByIdAsync(privateMessageId);
                        if (pm != null)
                        {
                            var customer = await WorkContext.GetCurrentCustomerAsync();

                            if (pm.ToCustomerId == customer.Id)
                            {
                                pm.IsRead = false;
                                await ForumService.UpdatePrivateMessageAsync(pm);
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
                        var pm = await ForumService.GetPrivateMessageByIdAsync(privateMessageId);
                        if (pm != null)
                        {
                            var customer = await WorkContext.GetCurrentCustomerAsync();

                            if (pm.FromCustomerId == customer.Id)
                            {
                                pm.IsDeletedByAuthor = true;
                                await ForumService.UpdatePrivateMessageAsync(pm);
                            }
                        }
                    }
                }
            }
            return RedirectToRoute("PrivateMessages", new {tab = "sent"});
        }

        public virtual async Task<IActionResult> SendPM(int toCustomerId, int? replyToMessageId)
        {
            if (!ForumSettings.AllowPrivateMessages)
                return RedirectToRoute("Homepage");

            if (await CustomerService.IsGuestAsync(await WorkContext.GetCurrentCustomerAsync()))
                return Challenge();

            var customerTo = await CustomerService.GetCustomerByIdAsync(toCustomerId);
            if (customerTo == null || await CustomerService.IsGuestAsync(customerTo))
                return RedirectToRoute("PrivateMessages");

            PrivateMessage replyToPM = null;
            if (replyToMessageId.HasValue)
            {
                //reply to a previous PM
                replyToPM = await ForumService.GetPrivateMessageByIdAsync(replyToMessageId.Value);
            }

            var model = await PrivateMessagesModelFactory.PrepareSendPrivateMessageModelAsync(customerTo, replyToPM);
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> SendPM(SendPrivateMessageModel model)
        {
            if (!ForumSettings.AllowPrivateMessages)
            {
                return RedirectToRoute("Homepage");
            }

            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (await CustomerService.IsGuestAsync(customer))
            {
                return Challenge();
            }

            Customer toCustomer;
            var replyToPM = await ForumService.GetPrivateMessageByIdAsync(model.ReplyToMessageId);
            if (replyToPM != null)
            {
                //reply to a previous PM
                if (replyToPM.ToCustomerId == customer.Id || replyToPM.FromCustomerId == customer.Id)
                {
                    //Reply to already sent PM (by current customer) should not be sent to yourself
                    toCustomer = await CustomerService.GetCustomerByIdAsync(replyToPM.FromCustomerId == customer.Id
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
                toCustomer = await CustomerService.GetCustomerByIdAsync(model.ToCustomerId);
            }

            if (toCustomer == null || await CustomerService.IsGuestAsync(toCustomer))
            {
                return RedirectToRoute("PrivateMessages");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var subject = model.Subject;
                    if (ForumSettings.PMSubjectMaxLength > 0 && subject.Length > ForumSettings.PMSubjectMaxLength)
                    {
                        subject = subject[0..ForumSettings.PMSubjectMaxLength];
                    }

                    var text = model.Message;
                    if (ForumSettings.PMTextMaxLength > 0 && text.Length > ForumSettings.PMTextMaxLength)
                    {
                        text = text[0..ForumSettings.PMTextMaxLength];
                    }

                    var nowUtc = DateTime.UtcNow;
                    var store = await StoreContext.GetCurrentStoreAsync();

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

                    await ForumService.InsertPrivateMessageAsync(privateMessage);

                    //activity log
                    await CustomerActivityService.InsertActivityAsync("PublicStore.SendPM",
                        string.Format(await LocalizationService.GetResourceAsync("ActivityLog.PublicStore.SendPM"), toCustomer.Email), toCustomer);

                    return RedirectToRoute("PrivateMessages", new { tab = "sent" });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            model = await PrivateMessagesModelFactory.PrepareSendPrivateMessageModelAsync(toCustomer, replyToPM);
            return View(model);
        }

        public virtual async Task<IActionResult> ViewPM(int privateMessageId)
        {
            if (!ForumSettings.AllowPrivateMessages)
            {
                return RedirectToRoute("Homepage");
            }

            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (await CustomerService.IsGuestAsync(customer))
            {
                return Challenge();
            }

            var pm = await ForumService.GetPrivateMessageByIdAsync(privateMessageId);
            if (pm != null)
            {
                if (pm.ToCustomerId != customer.Id && pm.FromCustomerId != customer.Id)
                {
                    return RedirectToRoute("PrivateMessages");
                }

                if (!pm.IsRead && pm.ToCustomerId == customer.Id)
                {
                    pm.IsRead = true;
                    await ForumService.UpdatePrivateMessageAsync(pm);
                }
            }
            else
            {
                return RedirectToRoute("PrivateMessages");
            }

            var model = await PrivateMessagesModelFactory.PreparePrivateMessageModelAsync(pm);
            return View(model);
        }

        public virtual async Task<IActionResult> DeletePM(int privateMessageId)
        {
            if (!ForumSettings.AllowPrivateMessages)
            {
                return RedirectToRoute("Homepage");
            }

            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (await CustomerService.IsGuestAsync(customer))
            {
                return Challenge();
            }

            var pm = await ForumService.GetPrivateMessageByIdAsync(privateMessageId);
            if (pm != null)
            {
                if (pm.FromCustomerId == customer.Id)
                {
                    pm.IsDeletedByAuthor = true;
                    await ForumService.UpdatePrivateMessageAsync(pm);
                }

                if (pm.ToCustomerId == customer.Id)
                {
                    pm.IsDeletedByRecipient = true;
                    await ForumService.UpdatePrivateMessageAsync(pm);
                }
            }
            return RedirectToRoute("PrivateMessages");
        }

        #endregion
    }
}