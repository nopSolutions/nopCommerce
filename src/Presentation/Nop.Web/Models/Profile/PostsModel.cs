<<<<<<< HEAD
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Profile
{
    public partial record PostsModel : BaseNopModel
    {
        public int ForumTopicId { get; set; }
        public string ForumTopicTitle { get; set; }
        public string ForumTopicSlug { get; set; }
        public string ForumPostText { get; set; }
        public string Posted { get; set; }
    }
=======
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Profile
{
    public partial record PostsModel : BaseNopModel
    {
        public int ForumTopicId { get; set; }
        public string ForumTopicTitle { get; set; }
        public string ForumTopicSlug { get; set; }
        public string ForumPostText { get; set; }
        public string Posted { get; set; }
    }
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
}