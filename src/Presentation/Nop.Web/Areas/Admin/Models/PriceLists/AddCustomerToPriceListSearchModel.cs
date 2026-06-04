using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.PriceLists;

/// <summary>
/// Represents a product search model to add to the price list
/// </summary>
public partial record AddCustomerToPriceListSearchModel : BaseSearchModel
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

    public AddCustomerToPriceListModel AddCustomerToPriceListModel { get; set; } = new();

    #endregion
}
