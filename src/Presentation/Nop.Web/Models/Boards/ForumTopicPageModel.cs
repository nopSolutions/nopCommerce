using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Boards
{
    public partial class ForumTopicPageModel : BaseNopModel
    {
        public ForumTopicPageModel()
        {
            ForumPostModels = new List<ForumPostModel>();
        }

        public int Id { get; set; }
        public string Subject { get; set; }
        public string SeName { get; set; }

        public string WatchTopicText { get; set; }

        public bool IsCustomerAllowedToEditTopic { get; set; }
        public bool IsCustomerAllowedToDeleteTopic { get; set; }
        public bool IsCustomerAllowedToMoveTopic { get; set; }
        public bool IsCustomerAllowedToSubscribe { get; set; }

        public IList<ForumPostModel> ForumPostModels { get; set; }
        public int PostsPageIndex { get; set; }
        public int PostsPageSize { get; set; }
        public int PostsTotalRecords { get; set; }
    }
}