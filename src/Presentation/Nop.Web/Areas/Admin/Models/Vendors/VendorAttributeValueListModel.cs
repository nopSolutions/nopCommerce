using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Vendors
{
    /// <summary>
    /// Represents a vendor attribute value list model
    /// </summary>
    public partial record VendorAttributeValueListModel : BasePagedListModel<VendorAttributeValueModel>
    {
    }
}