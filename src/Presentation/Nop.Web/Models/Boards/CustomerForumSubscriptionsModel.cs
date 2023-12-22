using Nop.Web.Framework.Models;
using Nop.Web.Models.Common;

namespace Nop.Web.Models.Boards;

public partial record CustomerForumSubscriptionsModel : BaseNopModel
{
    public CustomerForumSubscriptionsModel()
    {
        ForumSubscriptions = new List<ForumSubscriptionModel>();
    }

    public IList<ForumSubscriptionModel> ForumSubscriptions { get; set; }
    public PagerModel PagerModel { get; set; }

    #region Nested classes

    public partial record ForumSubscriptionModel : BaseNopEntityModel
    {
        public int ForumId { get; set; }
        public int ForumTopicId { get; set; }
        public bool TopicSubscription { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
    }

    #endregion
}