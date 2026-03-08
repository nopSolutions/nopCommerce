using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.News.Admin.Models;

/// <summary>
/// Represents a news comment list model
/// </summary>
public record NewsCommentListModel : BasePagedListModel<NewsCommentModel>
{
}