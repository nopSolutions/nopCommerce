using System.Collections.Generic;

namespace Nop.Web.Models.Profile
{
    public class ProfilePostsModel
    {
        public IList<PostsModel> Posts { get; set; }
        public PagerModel PagerModel { get; set; }
    }
}