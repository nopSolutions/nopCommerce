using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.Polls.Admin.Models;

/// <summary>
/// Represents a poll answer list model
/// </summary>
public record PollAnswerListModel : BasePagedListModel<PollAnswerModel>;