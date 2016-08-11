using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators.Directory;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Directory
{
    [Validator(typeof(MeasureDimensionValidator))]
    public partial class MeasureDimensionModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Configuration.Shipping.Measures.Dimensions.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Shipping.Measures.Dimensions.Fields.SystemKeyword")]
        [AllowHtml]
        public string SystemKeyword { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Shipping.Measures.Dimensions.Fields.Ratio")]
        public decimal Ratio { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Shipping.Measures.Dimensions.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Shipping.Measures.Dimensions.Fields.IsPrimaryDimension")]
        public bool IsPrimaryDimension { get; set; }
    }
}