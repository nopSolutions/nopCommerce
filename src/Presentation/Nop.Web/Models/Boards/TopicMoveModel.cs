using System.Collections.Generic;
#if NET451
using System.Web.Mvc;
#endif
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Models.Boards
{
    public partial class TopicMoveModel : BaseNopEntityModel
    {
        public TopicMoveModel()
        {
#if NET451
            ForumList = new List<SelectListItem>();
#endif
        }

        public int ForumSelected { get; set; }
        public string TopicSeName { get; set; }

#if NET451
        public IEnumerable<SelectListItem> ForumList { get; set; }
#endif
    }
}