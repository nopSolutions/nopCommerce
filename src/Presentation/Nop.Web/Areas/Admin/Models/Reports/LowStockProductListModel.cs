using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Reports
{
    /// <summary>
    /// Represents a low stock product list model
    /// </summary>
    public partial record LowStockProductListModel : BasePagedListModel<LowStockProductModel>
    {
    }
}