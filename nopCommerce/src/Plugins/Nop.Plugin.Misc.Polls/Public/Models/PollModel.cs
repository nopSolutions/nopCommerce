using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.Polls.Public.Models;

/// <summary>
/// Represents a poll model
/// </summary>
public record PollModel : BaseNopEntityModel
{
    #region Ctor

    public PollModel()
    {
        Answers = new List<PollAnswerModel>();
    }

    #endregion

    #region Properties

    public string Name { get; set; }

    public bool AlreadyVoted { get; set; }

    public int TotalVotes { get; set; }

    public IList<PollAnswerModel> Answers { get; set; }

    #endregion
}
