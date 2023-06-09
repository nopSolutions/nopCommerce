using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Boards
{
    public partial record TopicMoveModel : BaseNopEntityModel
    {
        public TopicMoveModel()
        {
            ForumList = new List<SelectListItem>();
        }

        public int ForumSelected { get; set; }
        public string TopicSeName { get; set; }

        public IEnumerable<SelectListItem> ForumList { get; set; }
    }
}