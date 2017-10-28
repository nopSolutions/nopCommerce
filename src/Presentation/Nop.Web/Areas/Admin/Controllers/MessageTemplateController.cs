using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Core.Domain.Messages;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class MessageTemplateController : BaseAdminController
    {
        #region Fields

        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly IPermissionService _permissionService;
        private readonly IStoreService _storeService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly ICustomerActivityService _customerActivityService;

        #endregion Fields

        #region Ctor

        public MessageTemplateController(IMessageTemplateService messageTemplateService, 
            IEmailAccountService emailAccountService,
            ILanguageService languageService, 
            ILocalizedEntityService localizedEntityService,
            ILocalizationService localizationService, 
            IMessageTokenProvider messageTokenProvider, 
            IPermissionService permissionService,
            IStoreService storeService,
            IStoreMappingService storeMappingService,
            IWorkflowMessageService workflowMessageService,
            ICustomerActivityService customerActivityService)
        {
            this._messageTemplateService = messageTemplateService;
            this._emailAccountService = emailAccountService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._localizationService = localizationService;
            this._messageTokenProvider = messageTokenProvider;
            this._permissionService = permissionService;
            this._storeService = storeService;
            this._storeMappingService = storeMappingService;
            this._workflowMessageService = workflowMessageService;
            this._customerActivityService = customerActivityService;
        }

        #endregion
        
        #region Utilities

        protected virtual void UpdateLocales(MessageTemplate mt, MessageTemplateModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(mt,
                    x => x.BccEmailAddresses,
                    localized.BccEmailAddresses,
                    localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(mt,
                    x => x.Subject,
                    localized.Subject,
                    localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(mt,
                    x => x.Body,
                    localized.Body,
                    localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(mt,
                    x => x.EmailAccountId,
                    localized.EmailAccountId,
                    localized.LanguageId);
            }
        }
        
        protected virtual void PrepareStoresMappingModel(MessageTemplateModel model, MessageTemplate messageTemplate, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!excludeProperties && messageTemplate != null)
                model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(messageTemplate).ToList();

            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                model.AvailableStores.Add(new SelectListItem
                {
                    Text = store.Name,
                    Value = store.Id.ToString(),
                    Selected = model.SelectedStoreIds.Contains(store.Id)
                });
            }
        }

        protected virtual void SaveStoreMappings(MessageTemplate messageTemplate, MessageTemplateModel model)
        {
            messageTemplate.LimitedToStores = model.SelectedStoreIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(messageTemplate);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(messageTemplate, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
                }
            }
        }

        #endregion
        
        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedView();

            var model = new MessageTemplateListModel();
            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
            
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(DataSourceRequest command, MessageTemplateListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedKendoGridJson();

            var messageTemplates = _messageTemplateService.GetAllMessageTemplates(model.SearchStoreId);
            var gridModel = new DataSourceResult
            {
                Data = messageTemplates.Select(x =>
                {
                    var templateModel = x.ToModel();
                    PrepareStoresMappingModel(templateModel, x, false);
                    var stores = _storeService
                            .GetAllStores()
                            .Where(s => !x.LimitedToStores || templateModel.SelectedStoreIds.Contains(s.Id))
                            .ToList();
                    for (var i = 0; i < stores.Count; i++)
                    {
                        templateModel.ListOfStores += stores[i].Name;
                        if (i != stores.Count - 1)
                            templateModel.ListOfStores += ", ";
                    }
                    return templateModel;
                }),
                Total = messageTemplates.Count
            };

            return Json(gridModel);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedView();

            var messageTemplate = _messageTemplateService.GetMessageTemplateById(id);
            if (messageTemplate == null)
                //no message template found with the specified id
                return RedirectToAction("List");
            
            var model = messageTemplate.ToModel();
            model.SendImmediately = !model.DelayBeforeSend.HasValue;
            model.HasAttachedDownload = model.AttachedDownloadId > 0;
            var allowedTokens = string.Join(", ", _messageTokenProvider.GetListOfAllowedTokens(messageTemplate.GetTokenGroups()));
            model.AllowedTokens = $"{allowedTokens}{Environment.NewLine}{Environment.NewLine}{_localizationService.GetResource("Admin.ContentManagement.MessageTemplates.Tokens.ConditionalStatement")}{Environment.NewLine}";

            //available email accounts
            foreach (var ea in _emailAccountService.GetAllEmailAccounts())
                model.AvailableEmailAccounts.Add(new SelectListItem { Text = ea.DisplayName, Value = ea.Id.ToString() });

            //store
            PrepareStoresMappingModel(model, messageTemplate, false);

            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.BccEmailAddresses = messageTemplate.GetLocalized(x => x.BccEmailAddresses, languageId, false, false);
                locale.Subject = messageTemplate.GetLocalized(x => x.Subject, languageId, false, false);
                locale.Body = messageTemplate.GetLocalized(x => x.Body, languageId, false, false);
                locale.EmailAccountId = messageTemplate.GetLocalized(x => x.EmailAccountId, languageId, false, false);

                //available email accounts (we add "Standard" value for localizable field)
                locale.AvailableEmailAccounts.Add(new SelectListItem
                {
                    Text = _localizationService.GetResource("Admin.ContentManagement.MessageTemplates.Fields.EmailAccount.Standard"),
                    Value = "0"
                });

                foreach (var ea in _emailAccountService.GetAllEmailAccounts())
                    locale.AvailableEmailAccounts.Add(new SelectListItem
                    {
                        Text = ea.DisplayName,
                        Value = ea.Id.ToString(),
                        Selected =  ea.Id == locale.EmailAccountId
                    });
            });

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual IActionResult Edit(MessageTemplateModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedView();

            var messageTemplate = _messageTemplateService.GetMessageTemplateById(model.Id);
            if (messageTemplate == null)
                //no message template found with the specified id
                return RedirectToAction("List");
            
            if (ModelState.IsValid)
            {
                messageTemplate = model.ToEntity(messageTemplate);

                //attached file
                if (!model.HasAttachedDownload)
                    messageTemplate.AttachedDownloadId = 0;
                if (model.SendImmediately)
                    messageTemplate.DelayBeforeSend = null;
                _messageTemplateService.UpdateMessageTemplate(messageTemplate);

                //activity log
                _customerActivityService.InsertActivity("EditMessageTemplate", _localizationService.GetResource("ActivityLog.EditMessageTemplate"), messageTemplate.Id);

                //stores
                SaveStoreMappings(messageTemplate, model);

                //locales
                UpdateLocales(messageTemplate, model);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.MessageTemplates.Updated"));
                
                if (continueEditing)
                {
                    return RedirectToAction("Edit",  new {id = messageTemplate.Id});
                }
                return RedirectToAction("List");
            }
            
            //if we got this far, something failed, redisplay form
            model.HasAttachedDownload = model.AttachedDownloadId > 0;
            var allowedTokens = string.Join(", ", _messageTokenProvider.GetListOfAllowedTokens(messageTemplate.GetTokenGroups()));
            model.AllowedTokens = $"{allowedTokens}{Environment.NewLine}{Environment.NewLine}{_localizationService.GetResource("Admin.ContentManagement.MessageTemplates.Tokens.ConditionalStatement")}{Environment.NewLine}";

            //available email accounts
            foreach (var ea in _emailAccountService.GetAllEmailAccounts())
                model.AvailableEmailAccounts.Add(new SelectListItem { Text = ea.DisplayName, Value = ea.Id.ToString() });

            //locales (update email account dropdownlists)
            foreach (var locale in model.Locales)
            {
                //available email accounts (we add "Standard" value for localizable field)
                locale.AvailableEmailAccounts.Add(new SelectListItem
                {
                    Text = _localizationService.GetResource("Admin.ContentManagement.MessageTemplates.Fields.EmailAccount.Standard"),
                    Value = "0"
                });
                foreach (var ea in _emailAccountService.GetAllEmailAccounts())
                    locale.AvailableEmailAccounts.Add(new SelectListItem
                    {
                        Text = ea.DisplayName,
                        Value = ea.Id.ToString(),
                        Selected = ea.Id == locale.EmailAccountId
                    });
            }

            //store
            PrepareStoresMappingModel(model, messageTemplate, true);
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedView();

            var messageTemplate = _messageTemplateService.GetMessageTemplateById(id);
            if (messageTemplate == null)
                //no message template found with the specified id
                return RedirectToAction("List");

            _messageTemplateService.DeleteMessageTemplate(messageTemplate);

            //activity log
            _customerActivityService.InsertActivity("DeleteMessageTemplate", _localizationService.GetResource("ActivityLog.DeleteMessageTemplate"), messageTemplate.Id);

            SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.MessageTemplates.Deleted"));
            return RedirectToAction("List");
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("message-template-copy")]
        public virtual IActionResult CopyTemplate(MessageTemplateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedView();

            var messageTemplate = _messageTemplateService.GetMessageTemplateById(model.Id);
            if (messageTemplate == null)
                //No message template found with the specified id
                return RedirectToAction("List");

            try
            {
                var newMessageTemplate = _messageTemplateService.CopyMessageTemplate(messageTemplate);
                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.MessageTemplates.Copied"));
                return RedirectToAction("Edit", new { id = newMessageTemplate.Id });
            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = model.Id });
            }
        }

        public virtual IActionResult TestTemplate(int id, int languageId = 0)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedView();

            var messageTemplate = _messageTemplateService.GetMessageTemplateById(id);
            if (messageTemplate == null)
                //No message template found with the specified id
                return RedirectToAction("List");

            var model = new TestMessageTemplateModel
            {
                Id = messageTemplate.Id,
                LanguageId = languageId
            };

            //filter tokens to the current template
            var subject = messageTemplate.GetLocalized(mt => mt.Subject, languageId);
            var body = messageTemplate.GetLocalized(mt => mt.Body, languageId);
            model.Tokens = _messageTokenProvider.GetListOfAllowedTokens().Where(x => subject.Contains(x) || body.Contains(x)).ToList();

            return View(model);
        }

        [HttpPost, ActionName("TestTemplate")]
        [FormValueRequired("send-test")]
        public virtual IActionResult TestTemplate(TestMessageTemplateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedView();

            var messageTemplate = _messageTemplateService.GetMessageTemplateById(model.Id);
            if (messageTemplate == null)
                //No message template found with the specified id
                return RedirectToAction("List");

            var tokens = new List<Token>();
            var form = model.Form;
            foreach (var formKey in form.Keys)
                if (formKey.StartsWith("token_", StringComparison.InvariantCultureIgnoreCase))
                {
                    var tokenKey = formKey.Substring("token_".Length).Replace("%", "");
                    var stringValue = form[formKey];

                    //try get non-string value
                    object tokenValue;
                    if (bool.TryParse(stringValue, out bool boolValue))
                        tokenValue = boolValue;
                    else if (int.TryParse(stringValue, out int intValue))
                        tokenValue = intValue;
                    else if (decimal.TryParse(stringValue, out decimal decimalValue))
                        tokenValue = decimalValue;
                    else
                        tokenValue = stringValue;

                    tokens.Add(new Token(tokenKey, tokenValue));
                }

            _workflowMessageService.SendTestEmail(messageTemplate.Id, model.SendTo, tokens, model.LanguageId);

            if (ModelState.IsValid)
            {
                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.MessageTemplates.Test.Success"));
            }

            return RedirectToAction("Edit", new {id = messageTemplate.Id});
        }

        #endregion
    }
}