using System;
using System.Threading.Tasks;

namespace Nop.Services.Seo
{
    /// <summary>
    /// Represents a sitemap generator
    /// </summary>
    public partial interface ISitemapGenerator
    {
        /// <summary>
        /// This will build an XML sitemap for better index with search engines.
        /// See http://en.wikipedia.org/wiki/Sitemaps for more information.
        /// </summary>
        /// <param name="id">Sitemap identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sitemap.xml as string
        /// </returns>
        Task<string> GenerateAsync(int? id);

        /// <summary>
        /// Return localized urls
        /// </summary>
        /// <param name="routeName">Route name</param>
        /// <param name="getRouteParamsAwait">Lambda for route params object</param>
        /// <param name="dateTimeUpdatedOn">A time when URL was updated last time</param>
        /// <param name="updateFreq">How often to update url</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task<SitemapUrl> GetLocalizedSitemapUrlAsync(string routeName,
            Func<int?, Task<object>> getRouteParamsAwait = null,
            DateTime? dateTimeUpdatedOn = null,
            UpdateFrequency updateFreq = UpdateFrequency.Weekly);
    }
}