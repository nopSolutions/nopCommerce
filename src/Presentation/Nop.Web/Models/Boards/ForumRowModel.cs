<<<<<<< HEAD
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Boards
{
    public partial record ForumRowModel : BaseNopModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SeName { get; set; }
        public string Description { get; set; }
        public int NumTopics { get; set; }
        public int NumPosts { get; set; }
        public int LastPostId { get; set; }
    }
=======
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Boards
{
    public partial record ForumRowModel : BaseNopModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SeName { get; set; }
        public string Description { get; set; }
        public int NumTopics { get; set; }
        public int NumPosts { get; set; }
        public int LastPostId { get; set; }
    }
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
}