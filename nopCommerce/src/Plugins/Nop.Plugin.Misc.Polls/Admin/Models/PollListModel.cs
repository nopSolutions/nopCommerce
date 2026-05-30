using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.Polls.Admin.Models;

/// <summary>
/// Represents a poll list model
/// </summary>
public record PollListModel : BasePagedListModel<PollModel>;