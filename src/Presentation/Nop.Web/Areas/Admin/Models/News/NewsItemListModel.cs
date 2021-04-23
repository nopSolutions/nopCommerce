using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.News
{
    /// <summary>
    /// Represents a news item list model
    /// </summary>
    public partial record NewsItemListModel : BasePagedListModel<NewsItemModel>
    {
    }
}