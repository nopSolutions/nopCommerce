using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.News;

/// <summary>
/// Represents a news comment list model
/// </summary>
public partial record NewsCommentListModel : BasePagedListModel<NewsCommentModel>
{
}