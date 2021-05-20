using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a cross-sell product list model
    /// </summary>
    public partial record CrossSellProductListModel : BasePagedListModel<CrossSellProductModel>
    {
    }
}