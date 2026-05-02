using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Vendors;

/// <summary>
/// Represents a vendor associated customer model
/// </summary>
public partial record VendorAssociatedCustomerModel : BaseNopEntityModel
{
    #region Properties

    public string Email { get; set; }

    #endregion
}