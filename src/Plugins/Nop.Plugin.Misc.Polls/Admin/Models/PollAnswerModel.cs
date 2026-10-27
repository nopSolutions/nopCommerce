using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.Polls.Admin.Models;

/// <summary>
/// Represents a poll answer model
/// </summary>
public record PollAnswerModel : BaseNopEntityModel
{
    #region Properties

    public int PollId { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Polls.Answers.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Polls.Answers.Fields.NumberOfVotes")]
    public int NumberOfVotes { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Polls.Answers.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    #endregion
}