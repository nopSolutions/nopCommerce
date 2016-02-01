using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators.Localization;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Localization
{
    [Validator(typeof(LanguageResourceValidator))]
    public partial class LanguageResourceModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Configuration.Languages.Resources.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Languages.Resources.Fields.Value")]
        [AllowHtml]
        public string Value { get; set; }

        public int LanguageId { get; set; }
    }
}