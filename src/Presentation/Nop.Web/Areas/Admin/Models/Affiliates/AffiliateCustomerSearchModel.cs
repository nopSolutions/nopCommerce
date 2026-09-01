using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Affiliates;

/// <summary>
/// Represents an affiliate customer search model
/// </summary>
public partial record AffiliateCustomerSearchModel : BaseSearchModel
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

    public AddCustomerToAffiliateModel AddCustomerToAffiliateModel { get; set; } = new();

    #endregion
}