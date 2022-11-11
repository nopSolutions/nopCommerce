using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Vendors
{
    /// <summary>
    /// Represents a vendor list model
    /// </summary>
    public partial record VendorListModel : BasePagedListModel<VendorModel>
    {
    }
}