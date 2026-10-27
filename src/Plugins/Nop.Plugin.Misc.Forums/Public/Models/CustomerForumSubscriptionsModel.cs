using Nop.Web.Framework.Models;
using Nop.Web.Models.Common;

namespace Nop.Plugin.Misc.Forums.Public.Models;

public record CustomerForumSubscriptionsModel : BaseNopModel
{
    #region Properties

    public List<ForumSubscriptionModel> ForumSubscriptions { get; set; } = new();
    public PagerModel PagerModel { get; set; }

    #endregion

    #region Nested classes

    public record ForumSubscriptionModel : BaseNopEntityModel
    {
        public int ForumId { get; set; }
        public int ForumTopicId { get; set; }
        public bool TopicSubscription { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
    }

    #endregion
}