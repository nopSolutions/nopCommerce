using FluentValidation.Attributes;
using Nop.Web.Areas.Admin.Validators.Directory;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Directory
{
    [Validator(typeof(MeasureWeightValidator))]
    public partial class MeasureWeightModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Configuration.Shipping.Measures.Weights.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Shipping.Measures.Weights.Fields.SystemKeyword")]
        public string SystemKeyword { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Shipping.Measures.Weights.Fields.Ratio")]
        public decimal Ratio { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Shipping.Measures.Weights.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Shipping.Measures.Weights.Fields.IsPrimaryWeight")]
        public bool IsPrimaryWeight { get; set; }
    }
}