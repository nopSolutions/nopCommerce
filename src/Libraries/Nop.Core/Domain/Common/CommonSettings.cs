
using Nop.Core.Configuration;

namespace Nop.Core.Domain.Common
{
    public class CommonSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whethyer to hide newsletter box
        /// </summary>
        public bool HideNewsletterBox { get; set; }

        public bool UseSystemEmailForContactUsForm { get; set; }


        public bool SitemapIncludeCategories { get; set; }
        public bool SitemapIncludeManufacturers { get; set; }
        public bool SitemapIncludeProducts { get; set; }
        public bool SitemapIncludeTopics { get; set; }
    }
}