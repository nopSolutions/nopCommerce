using System;

namespace Nop.Core.Domain.News
{
    /// <summary>
    /// News item extensions
    /// </summary>
    public static class NewsExtensions
    {
        /// <summary>
        /// Get a value indicating whether a news item is available now (availability dates)
        /// </summary>
        /// <param name="newsItem">News item</param>
        /// <returns>Result</returns>
        public static bool IsAvailable(this NewsItem newsItem)
        {
            return IsAvailable(newsItem, DateTime.UtcNow);
        }

        /// <summary>
        /// Get a value indicating whether a news item is available now (availability dates)
        /// </summary>
        /// <param name="newsItem">News item</param>
        /// <param name="dateTime">Datetime to check</param>
        /// <returns>Result</returns>
        public static bool IsAvailable(this NewsItem newsItem, DateTime dateTime)
        {
            if (newsItem == null)
                throw new ArgumentNullException(nameof(newsItem));

            if (newsItem.StartDateUtc.HasValue && newsItem.StartDateUtc.Value >= dateTime)
            {
                return false;
            }

            if (newsItem.EndDateUtc.HasValue && newsItem.EndDateUtc.Value <= dateTime)
            {
                return false;
            }

            return true;
        }
    }
}
