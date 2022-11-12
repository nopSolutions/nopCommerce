<<<<<<< HEAD
﻿using Nop.Core.Domain.Forums;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Boards
{
    public partial record ForumTopicRowModel : BaseNopModel
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string SeName { get; set; }
        public int LastPostId { get; set; }

        public int NumPosts { get; set; }
        public int Views { get; set; }
        public int Votes { get; set; }
        public int NumReplies { get; set; }
        public ForumTopicType ForumTopicType { get; set; }

        public int CustomerId { get; set; }
        public bool AllowViewingProfiles { get; set; }
        public string CustomerName { get; set; }

        //posts
        public int TotalPostPages { get; set; }
    }
=======
﻿using Nop.Core.Domain.Forums;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Boards
{
    public partial record ForumTopicRowModel : BaseNopModel
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string SeName { get; set; }
        public int LastPostId { get; set; }

        public int NumPosts { get; set; }
        public int Views { get; set; }
        public int Votes { get; set; }
        public int NumReplies { get; set; }
        public ForumTopicType ForumTopicType { get; set; }

        public int CustomerId { get; set; }
        public bool AllowViewingProfiles { get; set; }
        public string CustomerName { get; set; }

        //posts
        public int TotalPostPages { get; set; }
    }
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
}