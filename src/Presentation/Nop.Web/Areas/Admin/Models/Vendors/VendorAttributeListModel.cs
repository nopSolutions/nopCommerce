using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Vendors;

/// <summary>
/// Represents a vendor attribute list model
/// </summary>
public partial record VendorAttributeListModel : BasePagedListModel<VendorAttributeModel>
{
}