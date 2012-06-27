using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Catalog
{
    public partial class ProductSpecificationAttributeModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Catalog.Products.SpecificationAttributes.Fields.SpecificationAttribute")]
        [AllowHtml]
        public string SpecificationAttributeName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.SpecificationAttributes.Fields.SpecificationAttributeOption")]
        [AllowHtml]
        public string SpecificationAttributeOptionName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.SpecificationAttributes.Fields.AllowFiltering")]
        public bool AllowFiltering { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.SpecificationAttributes.Fields.ShowOnProductPage")]
        public bool ShowOnProductPage { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.SpecificationAttributes.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }
    }
}