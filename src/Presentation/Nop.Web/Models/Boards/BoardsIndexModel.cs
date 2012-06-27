using System;
using System.Collections.Generic;

namespace Nop.Web.Models.Boards
{
    public partial class BoardsIndexModel
    {
        public BoardsIndexModel()
        {
            this.ForumGroups = new List<ForumGroupModel>();
        }

        public DateTime CurrentTime { get; set; }
        
        public IList<ForumGroupModel> ForumGroups { get; set; }
    }
}