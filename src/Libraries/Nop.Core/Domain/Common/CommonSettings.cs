
using Nop.Core.Configuration;

namespace Nop.Core.Domain.Common
{
    public class CommonSettings : ISettings
    {
        public bool UseSystemEmailForContactUsForm { get; set; }

        public bool UseStoredProceduresIfSupported { get; set; }

        public bool HideAdvertisementsOnAdminArea { get; set; }

        public bool SitemapEnabled { get; set; }
        public bool SitemapIncludeCategories { get; set; }
        public bool SitemapIncludeManufacturers { get; set; }
        public bool SitemapIncludeProducts { get; set; }
        public bool SitemapIncludeTopics { get; set; }
    }
}