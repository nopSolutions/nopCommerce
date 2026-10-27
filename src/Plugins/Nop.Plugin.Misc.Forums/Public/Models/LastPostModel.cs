using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.Forums.Public.Models;

public record LastPostModel : BaseNopEntityModel
{
    #region Properties

    public int ForumTopicId { get; set; }
    public string ForumTopicSeName { get; set; }
    public string ForumTopicSubject { get; set; }
    public int CustomerId { get; set; }
    public bool AllowViewingProfiles { get; set; }
    public string CustomerName { get; set; }
    public string PostCreatedOnStr { get; set; }
    public bool ShowTopic { get; set; }

    #endregion
}