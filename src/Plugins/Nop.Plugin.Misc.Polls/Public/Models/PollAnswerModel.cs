using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.Polls.Public.Models;

/// <summary>
/// Represents a poll answer model
/// </summary>
public record PollAnswerModel : BaseNopEntityModel
{
    #region Properties

    public string Name { get; set; }

    public int NumberOfVotes { get; set; }

    public double PercentOfTotalVotes { get; set; }

    #endregion
}