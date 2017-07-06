using FluentValidation.Attributes;
using Nop.Web.Areas.Admin.Validators.Templates;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Templates
{
    [Validator(typeof(ManufacturerTemplateValidator))]
    public partial class ManufacturerTemplateModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.System.Templates.Manufacturer.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.System.Templates.Manufacturer.ViewPath")]
        public string ViewPath { get; set; }

        [NopResourceDisplayName("Admin.System.Templates.Manufacturer.DisplayOrder")]
        public int DisplayOrder { get; set; }
    }
}