using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a stock quantity history list model
/// </summary>
public partial record StockQuantityHistoryListModel : BasePagedListModel<StockQuantityHistoryModel>
{
}