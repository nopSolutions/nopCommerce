using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Polls;

/// <summary>
/// Represents a poll answer list model
/// </summary>
public partial record PollAnswerListModel : BasePagedListModel<PollAnswerModel>
{
}