
namespace Nop.Web.Models.Profile
{
    public class PostsModel
    {
        public int ForumTopicId { get; set; }
        public string ForumTopicTitle { get; set; }
        public string ForumTopicSlug { get; set; }
        public string ForumPostText { get; set; }
        public string Posted { get; set; }
    }
}