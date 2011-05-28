using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Blogs
{
    public class BlogPostModel : BaseNopEntityModel
    {
        public BlogPostModel()
        {
            Tags = new List<string>();
        }

        public string SeName { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public bool AllowComments { get; set; }

        public int NumberOfComments { get; set; }

        public DateTime CreatedOn { get; set; }

        public IList<string> Tags { get; set; }
    }
}