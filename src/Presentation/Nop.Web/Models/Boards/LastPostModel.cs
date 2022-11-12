<<<<<<< HEAD
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Boards
{
    public partial record LastPostModel : BaseNopModel
    {
        public int Id { get; set; }
        public int ForumTopicId { get; set; }
        public string ForumTopicSeName { get; set; }
        public string ForumTopicSubject { get; set; }
        
        public int CustomerId { get; set; }
        public bool AllowViewingProfiles { get; set; }
        public string CustomerName { get; set; }

        public string PostCreatedOnStr { get; set; }
        
        public bool ShowTopic { get; set; }
    }
=======
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Boards
{
    public partial record LastPostModel : BaseNopModel
    {
        public int Id { get; set; }
        public int ForumTopicId { get; set; }
        public string ForumTopicSeName { get; set; }
        public string ForumTopicSubject { get; set; }
        
        public int CustomerId { get; set; }
        public bool AllowViewingProfiles { get; set; }
        public string CustomerName { get; set; }

        public string PostCreatedOnStr { get; set; }
        
        public bool ShowTopic { get; set; }
    }
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
}