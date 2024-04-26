using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Discounts;

/// <summary>
/// Represents a product model to add to the discount
/// </summary>
public partial record AddProductToDiscountModel : BaseNopModel
{
    #region Ctor

    public AddProductToDiscountModel()
    {
        SelectedProductIds = new List<int>();
    }
    #endregion

    #region Properties

    public int DiscountId { get; set; }

    public IList<int> SelectedProductIds { get; set; }

    #endregion
}