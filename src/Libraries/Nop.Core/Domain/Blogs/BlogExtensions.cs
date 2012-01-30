using System;
using System.Collections.Generic;

namespace Nop.Core.Domain.Blogs
{
    public static class BlogExtensions
    {
        public static string[] ParseTags(this BlogPost blogPost)
        {
            if (blogPost == null)
                throw new ArgumentNullException("blogPost");

            var parsedTags = new List<string>();
            if (!String.IsNullOrEmpty(blogPost.Tags))
            {
                string[] tags2 = blogPost.Tags.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string tag2 in tags2)
                {
                    parsedTags.Add(tag2.Trim());
                }
            }
            return parsedTags.ToArray();
        }
    }
}
