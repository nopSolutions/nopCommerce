using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Validators.Messages;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Messages
{
    [Validator(typeof(MessageTemplateValidator))]
    public partial class MessageTemplateModel : BaseNopEntityModel, ILocalizedModel<MessageTemplateLocalizedModel>
    {
        public MessageTemplateModel()
        {
            Locales = new List<MessageTemplateLocalizedModel>();
            AvailableEmailAccounts = new List<SelectListItem>();

            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.AllowedTokens")]
        public string AllowedTokens { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.BccEmailAddresses")]
        public string BccEmailAddresses { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Subject")]
        public string Subject { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Body")]
        public string Body { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.IsActive")]
        public bool IsActive { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.SendImmediately")]
        public bool SendImmediately { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.DelayBeforeSend")]
        [UIHint("Int32Nullable")]
        public int? DelayBeforeSend { get; set; }
        public int DelayPeriodId { get; set; }

        public bool HasAttachedDownload { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.AttachedDownload")]
        [UIHint("Download")]
        public int AttachedDownloadId { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.EmailAccount")]
        public int EmailAccountId { get; set; }
        public IList<SelectListItem> AvailableEmailAccounts { get; set; }

        //store mapping
        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.LimitedToStores")]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        //comma-separated list of stores used on the list page
        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.LimitedToStores")]
        public string ListOfStores { get; set; }

        public IList<MessageTemplateLocalizedModel> Locales { get; set; }
    }

    public partial class MessageTemplateLocalizedModel : ILocalizedModelLocal
    {
        public MessageTemplateLocalizedModel()
        {
            AvailableEmailAccounts = new List<SelectListItem>();
        }

        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.BccEmailAddresses")]
        public string BccEmailAddresses { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Subject")]
        public string Subject { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Body")]
        public string Body { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.EmailAccount")]
        public int EmailAccountId { get; set; }
        public IList<SelectListItem> AvailableEmailAccounts { get; set; }
    }
}