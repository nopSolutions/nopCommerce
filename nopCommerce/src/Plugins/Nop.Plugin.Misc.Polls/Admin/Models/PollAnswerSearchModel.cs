using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.Polls.Admin.Models;

/// <summary>
/// Represents a poll answer search model
/// </summary>
public record PollAnswerSearchModel : BaseSearchModel
{
    #region Ctor

    public PollAnswerSearchModel()
    {
        AddPollAnswer = new PollAnswerModel();
    }

    #endregion

    #region Properties

    public int PollId { get; set; }

    public PollAnswerModel AddPollAnswer { get; set; }

    #endregion
}