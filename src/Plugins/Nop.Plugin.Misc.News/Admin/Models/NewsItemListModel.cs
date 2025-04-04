using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.News.Admin.Models;

/// <summary>
/// Represents a news item list model
/// </summary>
public record NewsItemListModel : BasePagedListModel<NewsItemModel>
{
}