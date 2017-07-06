using FluentValidation.Attributes;
using Nop.Web.Areas.Admin.Validators.Templates;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Templates
{
    [Validator(typeof(CategoryTemplateValidator))]
    public partial class CategoryTemplateModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.System.Templates.Category.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.System.Templates.Category.ViewPath")]
        public string ViewPath { get; set; }

        [NopResourceDisplayName("Admin.System.Templates.Category.DisplayOrder")]
        public int DisplayOrder { get; set; }
    }
}