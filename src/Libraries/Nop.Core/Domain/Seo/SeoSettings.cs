using Nop.Core.Configuration;

namespace Nop.Core.Domain.Seo
{
    /// <summary>
    /// SEO settings
    /// </summary>
    public partial class SeoSettings : ISettings
    {
        /// <summary>
        /// Page title separator
        /// </summary>
        public string PageTitleSeparator { get; set; }

        /// <summary>
        /// Page title SEO adjustment
        /// </summary>
        public PageTitleSeoAdjustment PageTitleSeoAdjustment { get; set; }

        /// <summary>
        /// A value indicating whether product META descriptions will be generated automatically (if not entered)
        /// </summary>
        public bool GenerateProductMetaDescription { get; set; }

        /// <summary>
        /// A value indicating whether we should convert non-western chars to western ones
        /// </summary>
        public bool ConvertNonWesternChars { get; set; }

        /// <summary>
        /// A value indicating whether unicode chars are allowed
        /// </summary>
        public bool AllowUnicodeCharsInUrls { get; set; }

        /// <summary>
        /// A value indicating whether canonical URL tags should be used
        /// </summary>
        public bool CanonicalUrlsEnabled { get; set; }

        /// <summary>
        /// A value indicating whether to use canonical URLs with query string parameters
        /// </summary>
        public bool QueryStringInCanonicalUrlsEnabled { get; set; }

        /// <summary>
        /// WWW requires (with or without WWW)
        /// </summary>
        public WwwRequirement WwwRequirement { get; set; }

        /// <summary>
        /// A value indicating whether Twitter META tags should be generated
        /// </summary>
        public bool TwitterMetaTags { get; set; }

        /// <summary>
        /// A value indicating whether Open Graph META tags should be generated
        /// </summary>
        public bool OpenGraphMetaTags { get; set; }

        /// <summary>
        /// Slugs (seName) reserved for some other needs
        /// </summary>
        public List<string> ReservedUrlRecordSlugs { get; set; }

        /// <summary>
        /// Custom tags in the <![CDATA[<head></head>]]> section
        /// </summary>
        public string CustomHeadTags { get; set; }

        /// <summary>
        /// A value indicating whether Microdata tags should be generated
        /// </summary>
        public bool MicrodataEnabled { get; set; }
    }
}