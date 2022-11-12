<<<<<<< HEAD
﻿using System;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Blogs
{
    public partial record BlogCommentModel : BaseNopEntityModel
    {
        public int CustomerId { get; set; }

        public string CustomerName { get; set; }

        public string CustomerAvatarUrl { get; set; }

        public string CommentText { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool AllowViewingProfiles { get; set; }
    }
=======
﻿using System;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Blogs
{
    public partial record BlogCommentModel : BaseNopEntityModel
    {
        public int CustomerId { get; set; }

        public string CustomerName { get; set; }

        public string CustomerAvatarUrl { get; set; }

        public string CommentText { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool AllowViewingProfiles { get; set; }
    }
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
}