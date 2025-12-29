using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Http;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using Nop.Web.Models.PrivateMessages;

namespace Nop.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class PrivateMessagesController : BasePublicController
{
    #region Fields

    protected readonly CustomerSettings _customerSettings;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerService _customerService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IPrivateMessagesModelFactory _privateMessagesModelFactory;
    protected readonly IStoreContext _storeContext;
    protected readonly IWorkContext _workContext;
    protected readonly IWorkflowMessageService _workflowMessageService;

    #endregion

    #region Ctor

    public PrivateMessagesController(CustomerSettings customerSettings,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        IPrivateMessagesModelFactory privateMessagesModelFactory,
        IStoreContext storeContext,
        IWorkContext workContext,
        IWorkflowMessageService workflowMessageService)
    {
        _customerSettings = customerSettings;
        _customerActivityService = customerActivityService;
        _customerService = customerService;
        _genericAttributeService = genericAttributeService;
        _localizationService = localizationService;
        _privateMessagesModelFactory = privateMessagesModelFactory;
        _storeContext = storeContext;
        _workContext = workContext;
        _workflowMessageService = workflowMessageService;
    }

    #endregion

    #region Methods

    public virtual async Task<IActionResult> Index(int? pageNumber, string tab)
    {
        if (!_customerSettings.AllowPrivateMessages)
        {
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);
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
                    var pm = await _customerService.GetPrivateMessageByIdAsync(privateMessageId);
                    if (pm != null)
                    {
                        var customer = await _workContext.GetCurrentCustomerAsync();

                        if (pm.ToCustomerId == customer.Id)
                        {
                            pm.IsDeletedByRecipient = true;
                            await _customerService.UpdatePrivateMessageAsync(pm);
                        }
                    }
                }
            }
        }
        return RedirectToRoute(NopRouteNames.Standard.PRIVATE_MESSAGES);
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
                    var pm = await _customerService.GetPrivateMessageByIdAsync(privateMessageId);
                    if (pm != null)
                    {
                        var customer = await _workContext.GetCurrentCustomerAsync();

                        if (pm.ToCustomerId == customer.Id)
                        {
                            pm.IsRead = false;
                            await _customerService.UpdatePrivateMessageAsync(pm);
                        }
                    }
                }
            }
        }
        return RedirectToRoute(NopRouteNames.Standard.PRIVATE_MESSAGES);
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
                    var pm = await _customerService.GetPrivateMessageByIdAsync(privateMessageId);
                    if (pm != null)
                    {
                        var customer = await _workContext.GetCurrentCustomerAsync();

                        if (pm.FromCustomerId == customer.Id)
                        {
                            pm.IsDeletedByAuthor = true;
                            await _customerService.UpdatePrivateMessageAsync(pm);
                        }
                    }
                }
            }
        }
        return RedirectToRoute(NopRouteNames.Standard.PRIVATE_MESSAGES, new { tab = "sent" });
    }

    public virtual async Task<IActionResult> SendPM(int toCustomerId, int? replyToMessageId)
    {
        if (!_customerSettings.AllowPrivateMessages)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        if (await _customerService.IsGuestAsync(await _workContext.GetCurrentCustomerAsync()))
            return Challenge();

        var customerTo = await _customerService.GetCustomerByIdAsync(toCustomerId);
        if (customerTo == null || await _customerService.IsGuestAsync(customerTo))
            return RedirectToRoute(NopRouteNames.Standard.PRIVATE_MESSAGES);

        PrivateMessage replyToPM = null;
        if (replyToMessageId.HasValue)
        {
            //reply to a previous PM
            replyToPM = await _customerService.GetPrivateMessageByIdAsync(replyToMessageId.Value);
        }

        var model = await _privateMessagesModelFactory.PrepareSendPrivateMessageModelAsync(customerTo, replyToPM);
        return View(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> SendPM(SendPrivateMessageModel model)
    {
        if (!_customerSettings.AllowPrivateMessages)
        {
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);
        }

        var customer = await _workContext.GetCurrentCustomerAsync();
        if (await _customerService.IsGuestAsync(customer))
        {
            return Challenge();
        }

        Customer toCustomer;
        var replyToPM = await _customerService.GetPrivateMessageByIdAsync(model.ReplyToMessageId);
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
                return RedirectToRoute(NopRouteNames.Standard.PRIVATE_MESSAGES);
            }
        }
        else
        {
            //first PM
            toCustomer = await _customerService.GetCustomerByIdAsync(model.ToCustomerId);
        }

        if (toCustomer == null || await _customerService.IsGuestAsync(toCustomer))
        {
            return RedirectToRoute(NopRouteNames.Standard.PRIVATE_MESSAGES);
        }

        if (ModelState.IsValid)
        {
            try
            {
                var subject = model.Subject;
                if (_customerSettings.PMSubjectMaxLength > 0 && subject.Length > _customerSettings.PMSubjectMaxLength)
                {
                    subject = subject[0.._customerSettings.PMSubjectMaxLength];
                }

                var text = model.Message;
                if (_customerSettings.PMTextMaxLength > 0 && text.Length > _customerSettings.PMTextMaxLength)
                {
                    text = text[0.._customerSettings.PMTextMaxLength];
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

                await _customerService.InsertPrivateMessageAsync(privateMessage);

                //UI notification
                await _genericAttributeService.SaveAttributeAsync(toCustomer, NopCustomerDefaults.NotifiedAboutNewPrivateMessagesAttribute, false, privateMessage.StoreId);

                //Email notification
                if (_customerSettings.NotifyAboutPrivateMessages)
                    await _workflowMessageService.SendPrivateMessageNotificationAsync(privateMessage, (await _workContext.GetWorkingLanguageAsync())?.Id ?? 0);

                //activity log
                await _customerActivityService.InsertActivityAsync("PublicStore.SendPM",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.SendPM"), toCustomer.Email), toCustomer);

                return RedirectToRoute(NopRouteNames.Standard.PRIVATE_MESSAGES, new { tab = "sent" });
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
        if (!_customerSettings.AllowPrivateMessages)
        {
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);
        }

        var customer = await _workContext.GetCurrentCustomerAsync();
        if (await _customerService.IsGuestAsync(customer))
        {
            return Challenge();
        }

        var pm = await _customerService.GetPrivateMessageByIdAsync(privateMessageId);
        if (pm != null)
        {
            if (pm.ToCustomerId != customer.Id && pm.FromCustomerId != customer.Id)
            {
                return RedirectToRoute(NopRouteNames.Standard.PRIVATE_MESSAGES);
            }

            if (!pm.IsRead && pm.ToCustomerId == customer.Id)
            {
                pm.IsRead = true;
                await _customerService.UpdatePrivateMessageAsync(pm);
            }
        }
        else
        {
            return RedirectToRoute(NopRouteNames.Standard.PRIVATE_MESSAGES);
        }

        var model = await _privateMessagesModelFactory.PreparePrivateMessageModelAsync(pm);
        return View(model);
    }

    public virtual async Task<IActionResult> DeletePM(int privateMessageId)
    {
        if (!_customerSettings.AllowPrivateMessages)
        {
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);
        }

        var customer = await _workContext.GetCurrentCustomerAsync();
        if (await _customerService.IsGuestAsync(customer))
        {
            return Challenge();
        }

        var pm = await _customerService.GetPrivateMessageByIdAsync(privateMessageId);
        if (pm != null)
        {
            if (pm.FromCustomerId == customer.Id)
            {
                pm.IsDeletedByAuthor = true;
                await _customerService.UpdatePrivateMessageAsync(pm);
            }

            if (pm.ToCustomerId == customer.Id)
            {
                pm.IsDeletedByRecipient = true;
                await _customerService.UpdatePrivateMessageAsync(pm);
            }
        }
        return RedirectToRoute(NopRouteNames.Standard.PRIVATE_MESSAGES);
    }

    #endregion
}