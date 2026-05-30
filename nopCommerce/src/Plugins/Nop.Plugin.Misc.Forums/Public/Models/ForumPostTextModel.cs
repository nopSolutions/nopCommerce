using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.Forums.Public.Models;

public record ForumPostTextModel : BaseNopModel
{
    #region Properties

    public string Text { get; set; }

    #endregion
}