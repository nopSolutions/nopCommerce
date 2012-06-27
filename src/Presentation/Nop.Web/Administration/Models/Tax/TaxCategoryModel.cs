using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators.Tax;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Tax
{
    [Validator(typeof(TaxCategoryValidator))]
    public partial class TaxCategoryModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Configuration.Tax.Categories.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Tax.Categories.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }
    }
}