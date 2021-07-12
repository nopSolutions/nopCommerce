using System;
using System.Collections.Generic;

namespace Nop.Services.Seo
{
    /// <summary>
    /// Represents sitemap URL entry
    /// </summary>
    public partial class SitemapUrl
    {
        #region Ctor

        /// <summary>
        /// Initializes a new instance of the sitemap URL entry
        /// </summary>
        /// <param name="location">URL of the page</param>
        /// <param name="alternateLocations">List of the page urls</param>
        /// <param name="frequency">Update frequency</param>
        /// <param name="updatedOn">Updated on</param>
        public SitemapUrl(string location, IList<string> alternateLocations, UpdateFrequency frequency, DateTime updatedOn)
        {
            Location = location;
            AlternateLocations = alternateLocations ?? new List<string>();
            UpdateFrequency = frequency;
            UpdatedOn = updatedOn;
        }

        /// <summary>
        /// Initializes a new instance of the sitemap URL entry based on the passed entry
        /// </summary>
        /// <param name="location">URL of the page</param>
        /// <param name="sitemapUrl">The another sitemap url</param>
        public SitemapUrl(string location, SitemapUrl sitemapUrl)
        {
            Location = location;
            AlternateLocations = sitemapUrl.AlternateLocations;
            UpdateFrequency = sitemapUrl.UpdateFrequency;
            UpdatedOn = sitemapUrl.UpdatedOn;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets URL of the page
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets localized URLs of the page
        /// </summary>
        public IList<string> AlternateLocations { get; set; }

        /// <summary>
        /// Gets or sets a value indicating how frequently the page is likely to change
        /// </summary>
        public UpdateFrequency UpdateFrequency { get; set; }

        /// <summary>
        /// Gets or sets the date of last modification of the page
        /// </summary>
        public DateTime UpdatedOn { get; set; }

        #endregion
    }
}