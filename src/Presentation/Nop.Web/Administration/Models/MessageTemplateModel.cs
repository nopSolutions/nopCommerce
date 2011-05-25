using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.UI;


namespace Nop.Admin.Models
{
    [Validator(typeof(MessageTemplateValidator))]
    public class MessageTemplateModel : BaseNopEntityModel, ILocalizedModel<MessageTemplateLocalizedModel>
    {
        public MessageTemplateModel()
        {
            Locales = new List<MessageTemplateLocalizedModel>();
            AvailableEmailAccounts = new List<EmailAccountModel>();
        }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.BccEmailAddresses")]
        [AllowHtml]
        public string BccEmailAddresses { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Subject")]
        [AllowHtml]
        public string Subject { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Body")]
        [AllowHtml]
        public string Body { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.IsActive")]
        [AllowHtml]
        public bool IsActive { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.EmailAccount")]
        public int EmailAccountId { get; set; }

        public IList<MessageTemplateLocalizedModel> Locales { get; set; }
        public IList<EmailAccountModel> AvailableEmailAccounts { get; set; }
    }

    public class MessageTemplateLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.BccEmailAddresses")]
        [AllowHtml]
        public string BccEmailAddresses { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Subject")]
        [AllowHtml]
        public string Subject { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Body")]
        [AllowHtml]
        public string Body { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.EmailAccount")]
        public int EmailAccountId { get; set; }
    }
}