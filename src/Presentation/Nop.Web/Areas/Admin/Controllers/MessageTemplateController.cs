using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Messages;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class MessageTemplateController : BaseAdminController
{
    #region Fields

    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedEntityService _localizedEntityService;
    protected readonly IMessageTemplateModelFactory _messageTemplateModelFactory;
    protected readonly IMessageTemplateService _messageTemplateService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IStoreService _storeService;
    protected readonly IWorkflowMessageService _workflowMessageService;

    #endregion Fields

    #region Ctor

    public MessageTemplateController(ICustomerActivityService customerActivityService,
        ILocalizationService localizationService,
        ILocalizedEntityService localizedEntityService,
        IMessageTemplateModelFactory messageTemplateModelFactory,
        IMessageTemplateService messageTemplateService,
        INotificationService notificationService,
        IPermissionService permissionService,
        IStoreMappingService storeMappingService,
        IStoreService storeService,
        IWorkflowMessageService workflowMessageService)
    {
        _customerActivityService = customerActivityService;
        _localizationService = localizationService;
        _localizedEntityService = localizedEntityService;
        _messageTemplateModelFactory = messageTemplateModelFactory;
        _messageTemplateService = messageTemplateService;
        _notificationService = notificationService;
        _permissionService = permissionService;
        _storeMappingService = storeMappingService;
        _storeService = storeService;
        _workflowMessageService = workflowMessageService;
    }

    #endregion

    #region Utilities

    protected virtual async Task UpdateLocalesAsync(MessageTemplate mt, MessageTemplateModel model)
    {
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(mt,
                x => x.BccEmailAddresses,
                localized.BccEmailAddresses,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(mt,
                x => x.Subject,
                localized.Subject,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(mt,
                x => x.Body,
                localized.Body,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(mt,
                x => x.EmailAccountId,
                localized.EmailAccountId,
                localized.LanguageId);
        }
    }

    protected virtual async Task SaveStoreMappingsAsync(MessageTemplate messageTemplate, MessageTemplateModel model)
    {
        messageTemplate.LimitedToStores = model.SelectedStoreIds.Any();
        await _messageTemplateService.UpdateMessageTemplateAsync(messageTemplate);

        var existingStoreMappings = await _storeMappingService.GetStoreMappingsAsync(messageTemplate);
        var allStores = await _storeService.GetAllStoresAsync();
        foreach (var store in allStores)
        {
            if (model.SelectedStoreIds.Contains(store.Id))
            {
                //new store
                if (!existingStoreMappings.Any(sm => sm.StoreId == store.Id))
                    await _storeMappingService.InsertStoreMappingAsync(messageTemplate, store.Id);
            }
            else
            {
                //remove store
                var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                if (storeMappingToDelete != null)
                    await _storeMappingService.DeleteStoreMappingAsync(storeMappingToDelete);
            }
        }
    }

    #endregion

    #region Methods

    public virtual IActionResult Index()
    {
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.ContentManagement.MESSAGE_TEMPLATES_VIEW)]
    public virtual async Task<IActionResult> List()
    {
        //prepare model
        var model = await _messageTemplateModelFactory.PrepareMessageTemplateSearchModelAsync(new MessageTemplateSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.MESSAGE_TEMPLATES_VIEW)]
    public virtual async Task<IActionResult> List(MessageTemplateSearchModel searchModel)
    {
        //prepare model
        var model = await _messageTemplateModelFactory.PrepareMessageTemplateListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.ContentManagement.MESSAGE_TEMPLATES_VIEW)]
    public virtual async Task<IActionResult> Edit(int id)
    {
        //try to get a message template with the specified id
        var messageTemplate = await _messageTemplateService.GetMessageTemplateByIdAsync(id);
        if (messageTemplate == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _messageTemplateModelFactory.PrepareMessageTemplateModelAsync(null, messageTemplate);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [FormValueRequired("save", "save-continue")]
    [CheckPermission(StandardPermission.ContentManagement.MESSAGE_TEMPLATES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(MessageTemplateModel model, bool continueEditing)
    {
        //try to get a message template with the specified id
        var messageTemplate = await _messageTemplateService.GetMessageTemplateByIdAsync(model.Id);
        if (messageTemplate == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            messageTemplate = model.ToEntity(messageTemplate);

            //attached file
            if (!model.HasAttachedDownload)
                messageTemplate.AttachedDownloadId = 0;
            if (model.SendImmediately)
                messageTemplate.DelayBeforeSend = null;
            await _messageTemplateService.UpdateMessageTemplateAsync(messageTemplate);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditMessageTemplate",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditMessageTemplate"), messageTemplate.Id), messageTemplate);

            //stores
            await SaveStoreMappingsAsync(messageTemplate, model);

            //locales
            await UpdateLocalesAsync(messageTemplate, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.MessageTemplates.Updated"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = messageTemplate.Id });
        }

        //prepare model
        model = await _messageTemplateModelFactory.PrepareMessageTemplateModelAsync(model, messageTemplate, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.MESSAGE_TEMPLATES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
        //try to get a message template with the specified id
        var messageTemplate = await _messageTemplateService.GetMessageTemplateByIdAsync(id);
        if (messageTemplate == null)
            return RedirectToAction("List");

        await _messageTemplateService.DeleteMessageTemplateAsync(messageTemplate);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteMessageTemplate",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteMessageTemplate"), messageTemplate.Id), messageTemplate);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.MessageTemplates.Deleted"));

        return RedirectToAction("List");
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("message-template-copy")]
    [CheckPermission(StandardPermission.ContentManagement.MESSAGE_TEMPLATES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CopyTemplate(MessageTemplateModel model)
    {
        //try to get a message template with the specified id
        var messageTemplate = await _messageTemplateService.GetMessageTemplateByIdAsync(model.Id);
        if (messageTemplate == null)
            return RedirectToAction("List");

        try
        {
            var newMessageTemplate = await _messageTemplateService.CopyMessageTemplateAsync(messageTemplate);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.MessageTemplates.Copied"));

            return RedirectToAction("Edit", new { id = newMessageTemplate.Id });
        }
        catch (Exception exc)
        {
            _notificationService.ErrorNotification(exc.Message);
            return RedirectToAction("Edit", new { id = model.Id });
        }
    }

    [CheckPermission(StandardPermission.ContentManagement.MESSAGE_TEMPLATES_VIEW)]
    public virtual async Task<IActionResult> TestTemplate(int id, int languageId = 0)
    {
        //try to get a message template with the specified id
        var messageTemplate = await _messageTemplateService.GetMessageTemplateByIdAsync(id);
        if (messageTemplate == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _messageTemplateModelFactory
            .PrepareTestMessageTemplateModelAsync(new TestMessageTemplateModel(), messageTemplate, languageId);

        return View(model);
    }

    [HttpPost, ActionName("TestTemplate")]
    [FormValueRequired("send-test")]
    [CheckPermission(StandardPermission.ContentManagement.MESSAGE_TEMPLATES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> TestTemplate(TestMessageTemplateModel model, IFormCollection form)
    {
        //try to get a message template with the specified id
        var messageTemplate = await _messageTemplateService.GetMessageTemplateByIdAsync(model.Id);
        if (messageTemplate == null)
            return RedirectToAction("List");

        var tokens = new List<Token>();
        foreach (var formKey in form.Keys)
            if (formKey.StartsWith("token_", StringComparison.InvariantCultureIgnoreCase))
            {
                var tokenKey = formKey["token_".Length..].Replace("%", string.Empty);
                var stringValue = form[formKey].ToString();

                //try get non-string value
                object tokenValue;
                if (bool.TryParse(stringValue, out var boolValue))
                    tokenValue = boolValue;
                else if (int.TryParse(stringValue, out var intValue))
                    tokenValue = intValue;
                else if (decimal.TryParse(stringValue, out var decimalValue))
                    tokenValue = decimalValue;
                else
                    tokenValue = stringValue;

                tokens.Add(new Token(tokenKey, tokenValue));
            }

        await _workflowMessageService.SendTestEmailAsync(messageTemplate.Id, model.SendTo, tokens, model.LanguageId);

        if (ModelState.IsValid)
        {
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.MessageTemplates.Test.Success"));
        }

        return RedirectToAction("Edit", new { id = messageTemplate.Id });
    }

    #endregion
}