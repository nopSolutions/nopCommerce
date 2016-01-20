using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Blogs;

namespace Nop.Services.Blogs
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class BlogExtensions
    {
        /// <summary>
        /// Returns all posts published between the two dates.
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="dateFrom">Date from</param>
        /// <param name="dateTo">Date to</param>
        /// <returns>Filtered posts</returns>
        public static IList<BlogPost> GetPostsByDate(this IList<BlogPost> source,
            DateTime dateFrom, DateTime dateTo)
        {
            return source.Where(p => dateFrom.Date <= (p.StartDateUtc ?? p.CreatedOnUtc) && 
            (p.StartDateUtc ?? p.CreatedOnUtc).Date <= dateTo).ToList();
        }
    }
}
