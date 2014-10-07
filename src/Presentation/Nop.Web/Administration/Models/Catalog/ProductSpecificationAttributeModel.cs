using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Catalog
{
    public partial class ProductSpecificationAttributeModel : BaseNopEntityModel
    {
        public string AttributeTypeName { get; set; }

        public string AttributeName { get; set; }

        public string ValueRaw { get; set; }

        public bool AllowFiltering { get; set; }

        public bool ShowOnProductPage { get; set; }

        public int DisplayOrder { get; set; }
    }
}