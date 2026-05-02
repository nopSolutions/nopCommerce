using Nop.Core.Domain.Orders;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.ShoppingCart;

/// <summary>
/// Represents a shopping cart item search model
/// </summary>
public partial record ShoppingCartItemSearchModel : BaseSearchModel
{
    #region Properties

    public int CustomerId { get; set; }

    public ShoppingCartType ShoppingCartType { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int ProductId { get; set; }

    public int BillingCountryId { get; set; }

    public int StoreId { get; set; }

    #endregion
}