using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Models.Blogs
{
    public partial class BlogPostTagModel : BaseNopModel
    {
        public string Name { get; set; }

        public int BlogPostCount { get; set; }
    }
}