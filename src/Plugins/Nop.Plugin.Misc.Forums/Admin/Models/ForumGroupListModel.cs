using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.Forums.Admin.Models;

/// <summary>
/// Represents a forum group list model
/// </summary>
public record ForumGroupListModel : BasePagedListModel<ForumGroupModel>;