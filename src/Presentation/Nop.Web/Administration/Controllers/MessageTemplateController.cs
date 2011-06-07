using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Models;
using Nop.Admin.Models.Messages;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Messages;
using Nop.Services.Catalog;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security.Permissions;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.UI;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public class MessageTemplateController : BaseNopController
    {
        #region Fields

        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;

        private readonly EmailAccountSettings _emailAccountSettings;
        #endregion Fields

        #region Constructors

        public MessageTemplateController(IMessageTemplateService messageTemplateService, 
            IEmailAccountService emailAccountService, ILanguageService languageService, 
            ILocalizedEntityService localizedEntityService, EmailAccountSettings emailAccountSettings)
        {
            this._messageTemplateService = messageTemplateService;
            this._emailAccountService = emailAccountService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._emailAccountSettings = emailAccountSettings;
        }

        #endregion Constructors
        
        #region Utilities

        [NonAction]
        public void UpdateLocales(MessageTemplate mt, MessageTemplateModel model)
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
        
        #endregion
        
        #region List

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            var messageTemplates = _messageTemplateService.GetAllMessageTemplates();
            var gridModel = new GridModel<MessageTemplateModel>
            {
                Data = messageTemplates.Select(x => x.ToModel()),
                Total = messageTemplates.Count
            };
            return View(gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command)
        {
            var messageTemplates = _messageTemplateService.GetAllMessageTemplates();
            var gridModel = new GridModel<MessageTemplateModel>
            {
                Data = messageTemplates.Select(x => x.ToModel()),
                Total = messageTemplates.Count
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        #endregion

        #region Create / Edit / Delete
        
        public ActionResult Edit(int id)
        {
            var messageTemplate = _messageTemplateService.GetMessageTemplateById(id);
            if (messageTemplate == null)
                throw new ArgumentException("No message template found with the specified id", "id");
            var model = messageTemplate.ToModel();
            //available email accounts
            foreach (var ea in _emailAccountService.GetAllEmailAccounts())
                model.AvailableEmailAccounts.Add(ea.ToModel());
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.BccEmailAddresses = messageTemplate.GetLocalized(x => x.BccEmailAddresses, languageId, false, false);
                locale.Subject = messageTemplate.GetLocalized(x => x.Subject, languageId, false, false);
                locale.Body = messageTemplate.GetLocalized(x => x.Body, languageId, false, false);

                var emailAccountId = messageTemplate.GetLocalized(x => x.EmailAccountId, languageId, false, false);
                locale.EmailAccountId = emailAccountId > 0 ? emailAccountId : _emailAccountSettings.DefaultEmailAccountId;
            });

            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult Edit(MessageTemplateModel model, bool continueEditing)
        {
            var messageTemplate = _messageTemplateService.GetMessageTemplateById(model.Id);
            if (messageTemplate == null)
                throw new ArgumentException("No message template found with the specified id");

            if (ModelState.IsValid)
            {
                messageTemplate = model.ToEntity(messageTemplate);
                _messageTemplateService.UpdateMessageTemplate(messageTemplate);
                //locales
                UpdateLocales(messageTemplate, model);

                return continueEditing ? RedirectToAction("Edit", messageTemplate.Id) : RedirectToAction("List");
            }


            //If we got this far, something failed, redisplay form
            //available email accounts
            foreach (var ea in _emailAccountService.GetAllEmailAccounts())
                model.AvailableEmailAccounts.Add(ea.ToModel());
            return View(model);
        }
        
        #endregion
    }
}
