using Nop.Plugin.Misc.Forums.Domain;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.Forums.Public.Models;

public record ForumTopicRowModel : BaseNopEntityModel
{
    #region Properties

    public string Subject { get; set; }
    public string SeName { get; set; }
    public int LastPostId { get; set; }
    public int NumPosts { get; set; }
    public int Views { get; set; }
    public int Votes { get; set; }
    public int NumReplies { get; set; }
    public ForumTopicType ForumTopicType { get; set; }
    public int CustomerId { get; set; }
    public bool AllowViewingProfiles { get; set; }
    public string CustomerName { get; set; }
    public int TotalPostPages { get; set; }

    #endregion
}