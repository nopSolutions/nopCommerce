using FluentValidation.Attributes;
using Nop.Web.Areas.Admin.Validators.Tax;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Tax
{
    [Validator(typeof(TaxCategoryValidator))]
    public partial class TaxCategoryModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Configuration.Tax.Categories.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Tax.Categories.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }
    }
}