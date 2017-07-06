using FluentValidation.Attributes;
using Nop.Web.Areas.Admin.Validators.Localization;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Localization
{
    [Validator(typeof(LanguageResourceValidator))]
    public partial class LanguageResourceModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Configuration.Languages.Resources.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Languages.Resources.Fields.Value")]
        public string Value { get; set; }

        public int LanguageId { get; set; }
    }
}