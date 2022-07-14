using System.Collections.Generic;
using Nop.Core.Configuration;

namespace Nop.Core.Domain.Security
{
    /// <summary>
    /// robots.txt settings
    /// </summary>
    public partial class RobotsTxtSettings : ISettings
    {
        /// <summary>
        /// Disallow paths
        /// </summary>
        public List<string> DisallowPaths { get; set; } = new();

        /// <summary>
        /// Localizable disallow paths
        /// </summary>
        public List<string> LocalizableDisallowPaths { get; set; } = new();

        /// <summary>
        /// Disallow languages
        /// </summary>
        public List<int> DisallowLanguages { get; set; } = new();

        /// <summary>
        /// Additions rules
        /// </summary>
        public List<string> AdditionsRules { get; set; } = new();

        /// <summary>
        /// Is sitemap.xml allow
        /// </summary>
        public bool AllowSitemapXml { get; set; } = true;
    }
}
