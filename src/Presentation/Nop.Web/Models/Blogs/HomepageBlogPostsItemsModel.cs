using Nop.Web.Framework.Models;
namespace Nop.Web.Models.Blogs{
    public partial record HomepageBlogPostItemsModel : BaseNopModel{
        public HomepageBlogPostItemsModel(){
            BlogPostItems = new List<BlogPostModel>();
        }

        public int WorkingLanguageId { get; set; }
        public IList<BlogPostModel> BlogPostItems { get; set; }
    }

}