using System;
using System.Collections.Generic;

namespace Nop.Core.Domain.Blogs
{
    /// <summary>
    /// Blog extensions
    /// </summary>
    public static class BlogExtensions
    {
        /// <summary>
        /// Parse tags
        /// </summary>
        /// <param name="blogPost">Blog post</param>
        /// <returns>Tags</returns>
        public static string[] ParseTags(this BlogPost blogPost)
        {
            if (blogPost == null)
                throw new ArgumentNullException(nameof(blogPost));

            var parsedTags = new List<string>();
            if (string.IsNullOrEmpty(blogPost.Tags)) 
                return parsedTags.ToArray();

            var tags2 = blogPost.Tags.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var tag2 in tags2)
            {
                var tmp = tag2.Trim();
                if (!string.IsNullOrEmpty(tmp))
                    parsedTags.Add(tmp);
            }

            return parsedTags.ToArray();
        }

        /// <summary>
        /// Get a value indicating whether a blog post is available now (availability dates)
        /// </summary>
        /// <param name="blogPost">Blog post</param>
        /// <returns>Result</returns>
        public static bool IsAvailable(this BlogPost blogPost)
        {
            return IsAvailable(blogPost, DateTime.UtcNow);
        }

        /// <summary>
        /// Get a value indicating whether a blog post is available now (availability dates)
        /// </summary>
        /// <param name="blogPost">Blog post</param>
        /// <param name="dateTime">Datetime to check</param>
        /// <returns>Result</returns>
        public static bool IsAvailable(this BlogPost blogPost, DateTime dateTime)
        {
            if (blogPost == null)
                throw new ArgumentNullException(nameof(blogPost));

            if (blogPost.StartDateUtc.HasValue && blogPost.StartDateUtc.Value >= dateTime)
            {
                return false;
            }

            if (blogPost.EndDateUtc.HasValue && blogPost.EndDateUtc.Value <= dateTime)
            {
                return false;
            }

            return true;
        }
    }
}
