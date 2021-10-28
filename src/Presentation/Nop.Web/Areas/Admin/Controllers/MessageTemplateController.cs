using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class MessageTemplateController : BaseAdminController
    {
        #region Fields

        protected ICustomerActivityService CustomerActivityService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected ILocalizedEntityService LocalizedEntityService { get; }
        protected IMessageTemplateModelFactory MessageTemplateModelFactory { get; }
        protected IMessageTemplateService MessageTemplateService { get; }
        protected INotificationService NotificationService { get; }
        protected IPermissionService PermissionService { get; }
        protected IStoreMappingService StoreMappingService { get; }
        protected IStoreService StoreService { get; }
        protected IWorkflowMessageService WorkflowMessageService { get; }

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
            CustomerActivityService = customerActivityService;
            LocalizationService = localizationService;
            LocalizedEntityService = localizedEntityService;
            MessageTemplateModelFactory = messageTemplateModelFactory;
            MessageTemplateService = messageTemplateService;
            NotificationService = notificationService;
            PermissionService = permissionService;
            StoreMappingService = storeMappingService;
            StoreService = storeService;
            WorkflowMessageService = workflowMessageService;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateLocalesAsync(MessageTemplate mt, MessageTemplateModel model)
        {
            foreach (var localized in model.Locales)
            {
                await LocalizedEntityService.SaveLocalizedValueAsync(mt,
                    x => x.BccEmailAddresses,
                    localized.BccEmailAddresses,
                    localized.LanguageId);

                await LocalizedEntityService.SaveLocalizedValueAsync(mt,
                    x => x.Subject,
                    localized.Subject,
                    localized.LanguageId);

                await LocalizedEntityService.SaveLocalizedValueAsync(mt,
                    x => x.Body,
                    localized.Body,
                    localized.LanguageId);

                await LocalizedEntityService.SaveLocalizedValueAsync(mt,
                    x => x.EmailAccountId,
                    localized.EmailAccountId,
                    localized.LanguageId);
            }
        }

        protected virtual async Task SaveStoreMappingsAsync(MessageTemplate messageTemplate, MessageTemplateModel model)
        {
            messageTemplate.LimitedToStores = model.SelectedStoreIds.Any();
            await MessageTemplateService.UpdateMessageTemplateAsync(messageTemplate);

            var existingStoreMappings = await StoreMappingService.GetStoreMappingsAsync(messageTemplate);
            var allStores = await StoreService.GetAllStoresAsync();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        await StoreMappingService.InsertStoreMappingAsync(messageTemplate, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        await StoreMappingService.DeleteStoreMappingAsync(storeMappingToDelete);
                }
            }
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedView();

            //prepare model
            var model = await MessageTemplateModelFactory.PrepareMessageTemplateSearchModelAsync(new MessageTemplateSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(MessageTemplateSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMessageTemplates))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await MessageTemplateModelFactory.PrepareMessageTemplateListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedView();

            //try to get a message template with the specified id
            var messageTemplate = await MessageTemplateService.GetMessageTemplateByIdAsync(id);
            if (messageTemplate == null)
                return RedirectToAction("List");

            //prepare model
            var model = await MessageTemplateModelFactory.PrepareMessageTemplateModelAsync(null, messageTemplate);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> Edit(MessageTemplateModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedView();

            //try to get a message template with the specified id
            var messageTemplate = await MessageTemplateService.GetMessageTemplateByIdAsync(model.Id);
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
                await MessageTemplateService.UpdateMessageTemplateAsync(messageTemplate);

                //activity log
                await CustomerActivityService.InsertActivityAsync("EditMessageTemplate",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditMessageTemplate"), messageTemplate.Id), messageTemplate);

                //stores
                await SaveStoreMappingsAsync(messageTemplate, model);

                //locales
                await UpdateLocalesAsync(messageTemplate, model);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.ContentManagement.MessageTemplates.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = messageTemplate.Id });
            }

            //prepare model
            model = await MessageTemplateModelFactory.PrepareMessageTemplateModelAsync(model, messageTemplate, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedView();

            //try to get a message template with the specified id
            var messageTemplate = await MessageTemplateService.GetMessageTemplateByIdAsync(id);
            if (messageTemplate == null)
                return RedirectToAction("List");

            await MessageTemplateService.DeleteMessageTemplateAsync(messageTemplate);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteMessageTemplate",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteMessageTemplate"), messageTemplate.Id), messageTemplate);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.ContentManagement.MessageTemplates.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("message-template-copy")]
        public virtual async Task<IActionResult> CopyTemplate(MessageTemplateModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedView();

            //try to get a message template with the specified id
            var messageTemplate = await MessageTemplateService.GetMessageTemplateByIdAsync(model.Id);
            if (messageTemplate == null)
                return RedirectToAction("List");

            try
            {
                var newMessageTemplate = await MessageTemplateService.CopyMessageTemplateAsync(messageTemplate);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.ContentManagement.MessageTemplates.Copied"));

                return RedirectToAction("Edit", new { id = newMessageTemplate.Id });
            }
            catch (Exception exc)
            {
                NotificationService.ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = model.Id });
            }
        }

        public virtual async Task<IActionResult> TestTemplate(int id, int languageId = 0)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedView();

            //try to get a message template with the specified id
            var messageTemplate = await MessageTemplateService.GetMessageTemplateByIdAsync(id);
            if (messageTemplate == null)
                return RedirectToAction("List");

            //prepare model
            var model = await MessageTemplateModelFactory
                .PrepareTestMessageTemplateModelAsync(new TestMessageTemplateModel(), messageTemplate, languageId);

            return View(model);
        }

        [HttpPost, ActionName("TestTemplate")]
        [FormValueRequired("send-test")]
        public virtual async Task<IActionResult> TestTemplate(TestMessageTemplateModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedView();

            //try to get a message template with the specified id
            var messageTemplate = await MessageTemplateService.GetMessageTemplateByIdAsync(model.Id);
            if (messageTemplate == null)
                return RedirectToAction("List");

            var tokens = new List<Token>();
            var form = model.Form;
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

            await WorkflowMessageService.SendTestEmailAsync(messageTemplate.Id, model.SendTo, tokens, model.LanguageId);

            if (ModelState.IsValid)
            {
                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.ContentManagement.MessageTemplates.Test.Success"));
            }

            return RedirectToAction("Edit", new { id = messageTemplate.Id });
        }

        #endregion
    }
}