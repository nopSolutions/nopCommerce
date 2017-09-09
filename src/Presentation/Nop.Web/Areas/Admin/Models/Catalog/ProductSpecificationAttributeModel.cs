using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    public partial class ProductSpecificationAttributeModel : BaseNopEntityModel
    {
        public int AttributeTypeId { get; set; }
        
        public string AttributeTypeName { get; set; }

        public int AttributeId { get; set; }
        
        public string AttributeName { get; set; }
        
        public string ValueRaw { get; set; }

        public bool AllowFiltering { get; set; }

        public bool ShowOnProductPage { get; set; }

        public int DisplayOrder { get; set; }

        public int SpecificationAttributeOptionId { get; set; } 
    }
}