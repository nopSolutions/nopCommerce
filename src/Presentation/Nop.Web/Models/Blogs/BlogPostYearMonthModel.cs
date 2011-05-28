using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Blogs
{
    public class BlogPostYearModel : BaseNopModel
    {
        public BlogPostYearModel()
        {
            Months = new List<BlogPostMonthModel>();
        }
        public int Year { get; set; }
        public IList<BlogPostMonthModel> Months { get; set; }
    }
    public class BlogPostMonthModel : BaseNopModel
    {
        public int Month { get; set; }

        public int BlogPostCount { get; set; }
    }
}