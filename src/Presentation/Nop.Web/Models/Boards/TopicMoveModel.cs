using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Boards
{
    public class TopicMoveModel : BaseNopEntityModel
    {
        public TopicMoveModel()
        {
            ForumList = new List<SelectListItem>();
        }

        public int ForumSelected { get; set; }

        public IEnumerable<SelectListItem> ForumList { get; set; }

        public ForumBreadcrumbModel ForumBreadCrumbModel { get; set; }
    }
}