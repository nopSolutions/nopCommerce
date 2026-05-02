using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Vendors;

/// <summary>
/// Represents a vendor search model
/// </summary>
public partial record VendorSearchModel : BaseSearchModel
{
    #region Properties

    [NopResourceDisplayName("Admin.Vendors.List.SearchName")]
    public string SearchName { get; set; }

    [NopResourceDisplayName("Admin.Vendors.List.SearchEmail")]
    public string SearchEmail { get; set; }

    #endregion
}