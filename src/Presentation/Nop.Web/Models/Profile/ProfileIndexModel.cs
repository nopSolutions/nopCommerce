<<<<<<< HEAD
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Profile
{
    public partial record ProfileIndexModel : BaseNopModel
    {
        public int CustomerProfileId { get; set; }
        public string ProfileTitle { get; set; }
        public int PostsPage { get; set; }
        public bool PagingPosts { get; set; }
        public bool ForumsEnabled { get; set; }
    }
=======
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Profile
{
    public partial record ProfileIndexModel : BaseNopModel
    {
        public int CustomerProfileId { get; set; }
        public string ProfileTitle { get; set; }
        public int PostsPage { get; set; }
        public bool PagingPosts { get; set; }
        public bool ForumsEnabled { get; set; }
    }
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
}