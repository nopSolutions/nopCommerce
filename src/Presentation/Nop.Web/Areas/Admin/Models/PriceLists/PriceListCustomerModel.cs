using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.PriceLists;

/// <summary>
/// Represents a price list customer model
/// </summary>
public partial record PriceListCustomerModel : BaseNopEntityModel
{
    #region Properties

    public int PriceListId { get; set; }

    public int CustomerId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.PriceList.Customers.Fields.Customer")]
    public string CustomerEmail { get; set; }

    #endregion
}
