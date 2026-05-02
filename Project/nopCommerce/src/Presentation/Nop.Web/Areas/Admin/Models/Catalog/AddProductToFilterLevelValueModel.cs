using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a product model to add to the filter level value
/// </summary>
public partial record AddProductToFilterLevelValueModel : BaseNopModel
{
    #region Ctor

    public AddProductToFilterLevelValueModel()
    {
        SelectedProductIds = new List<int>();
    }
    #endregion

    #region Properties

    public int FilterLevelValueId { get; set; }

    public IList<int> SelectedProductIds { get; set; }

    #endregion
}
