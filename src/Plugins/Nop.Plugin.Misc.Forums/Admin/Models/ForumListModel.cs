using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.Forums.Admin.Models;

/// <summary>
/// Represents a forum list model
/// </summary>
public record ForumListModel : BasePagedListModel<ForumModel>;