using System.Web.Mvc;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Catalog
{
    public partial class ProductSpecificationAttributeModel : BaseNopEntityModel
    {
        public int AttributeTypeId { get; set; }

        [AllowHtml]
        public string AttributeTypeName { get; set; }

        public int AttributeId { get; set; }

        [AllowHtml]
        public string AttributeName { get; set; }

        [AllowHtml]
        public string ValueRaw { get; set; }

        public bool AllowFiltering { get; set; }

        public bool ShowOnProductPage { get; set; }

        public int DisplayOrder { get; set; }

        public int SpecificationAttributeOptionId { get; set; } 
    }
}