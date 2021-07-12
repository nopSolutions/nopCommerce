using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Api.Catalog;

namespace Nop.Web.Models.Catalog
{
    public partial record ProductSpecificationApiModel : BaseNopModel
    {
        public ProductSpecificationApiModel()
        {
            ProductVendors = new List<VendorBriefInfoModel>();
            ProductSpecificationAttribute = new List<ProductSpecificationAttributeApiModel>();
        }

        public List<VendorBriefInfoModel> ProductVendors { get; set; }
        public IList<ProductSpecificationAttributeApiModel> ProductSpecificationAttribute { get; set; }

    }
}