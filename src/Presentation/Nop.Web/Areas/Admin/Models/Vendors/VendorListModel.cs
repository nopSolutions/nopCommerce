using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Vendors
{
    public partial class VendorListModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Vendors.List.SearchName")]
        public string SearchName { get; set; }
    }
}