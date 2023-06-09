using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Blogs
{
    public partial record BlogPostModel : BaseNopEntityModel
    {
        public BlogPostModel()
        {
            Tags = new List<string>();
            Comments = new List<BlogCommentModel>();
            AddNewComment = new AddBlogCommentModel();
        }

        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }

        public string Title { get; set; }
        public string Body { get; set; }
        public string BodyOverview { get; set; }
        public bool AllowComments { get; set; }
        public bool PreventNotRegisteredUsersToLeaveComments { get; set; }
        public int NumberOfComments { get; set; }
        public DateTime CreatedOn { get; set; }

        public IList<string> Tags { get; set; }

        public IList<BlogCommentModel> Comments { get; set; }
        public AddBlogCommentModel AddNewComment { get; set; }
    }
}