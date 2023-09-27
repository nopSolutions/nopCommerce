using Nop.Core.Configuration;

namespace Nop.Core.Domain.Common
{
    /// <summary>
    /// Sitemap settings
    /// </summary>
    public partial class SitemapSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether sitemap is enabled
        /// </summary>
        public bool SitemapEnabled { get; set; }

        /// <summary>
        /// Gets or sets the page size for sitemap
        /// </summary>
        public int SitemapPageSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include blog posts to sitemap
        /// </summary>
        public bool SitemapIncludeBlogPosts { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include categories to sitemap
        /// </summary>
        public bool SitemapIncludeCategories { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include manufacturers to sitemap
        /// </summary>
        public bool SitemapIncludeManufacturers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include news to sitemap
        /// </summary>
        public bool SitemapIncludeNews { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include products to sitemap
        /// </summary>
        public bool SitemapIncludeProducts { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include product tags to sitemap
        /// </summary>
        public bool SitemapIncludeProductTags { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include topics to sitemap
        /// </summary>
        public bool SitemapIncludeTopics { get; set; }
    }
}
