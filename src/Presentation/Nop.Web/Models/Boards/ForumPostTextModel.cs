using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Boards;

public partial record ForumPostTextModel : BaseNopModel
{
    public string Text { get; set; }
}