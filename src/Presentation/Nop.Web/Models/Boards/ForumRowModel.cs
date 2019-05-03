using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Boards
{
    public partial class ForumRowModel : BaseNopModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SeName { get; set; }
        public string Description { get; set; }
        public int NumTopics { get; set; }
        public int NumPosts { get; set; }
        public int LastPostId { get; set; }
    }
}