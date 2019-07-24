using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog
{
    public partial class VendorNavigationModel : BaseNopModel
    {
        public VendorNavigationModel()
        {
            Vendors = new List<VendorBriefInfoModel>();
        }

        public IList<VendorBriefInfoModel> Vendors { get; set; }

        public int TotalVendors { get; set; }
    }

    public partial class VendorBriefInfoModel : BaseNopEntityModel
    {
        public string Name { get; set; }

        public string SeName { get; set; }
    }
}