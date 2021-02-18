using System.Collections.Generic;

namespace Nop.Services.Seo
{
    /// <summary>
    /// Represents an event that occurs when the sitemap is created
    /// </summary>
    public class SitemapCreatedEvent
    {
        #region Ctor

        public SitemapCreatedEvent(IList<SitemapUrl> sitemapUrls)
        {
            SitemapUrls = sitemapUrls ?? new List<SitemapUrl>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a list of sitemap URLs
        /// </summary>
        public IList<SitemapUrl> SitemapUrls { get; }

        #endregion
    }
}