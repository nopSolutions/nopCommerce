using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Vendors;

/// <summary>
/// Represents a vendor customer search model
/// </summary>
public partial record VendorCustomerSearchModel : BaseSearchModel
{
    #region Properties

    [NopResourceDisplayName("Admin.Customers.Customers.List.SearchEmail")]
    public string SearchEmail { get; set; }

    [NopResourceDisplayName("Admin.Customers.Customers.List.SearchFirstName")]
    public string SearchFirstName { get; set; }

    [NopResourceDisplayName("Admin.Customers.Customers.List.SearchLastName")]
    public string SearchLastName { get; set; }

    [NopResourceDisplayName("Admin.Customers.Customers.List.SearchCompany")]
    public string SearchCompany { get; set; }

    public AddCustomerToVendorModel AddCustomerToVendorModel { get; set; } = new();

    #endregion
}
