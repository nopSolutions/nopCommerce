namespace Nop.Web.Models.Boards
{
    public class ForumBreadcrumbModel
    {
        public ForumGroupModel ForumGroupModel { get; set; }

        public ForumModel ForumModel { get; set; }

        public ForumTopicPostsModel ForumTopicPostsModel { get; set; }

        public string Separator { get; set; }

        public string StoreLocation { get; set; }
    }
}