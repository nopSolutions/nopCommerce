
using System;
using System.IO;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Topics;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Topics;

namespace Nop.Services.Seo
{
    /// <summary>
    /// Represents a sitemap generator
    /// </summary>
    public partial interface ISitemapGenerator
    {
        /// <summary>
        /// This will build an xml sitemap for better index with search engines.
        /// See http://en.wikipedia.org/wiki/Sitemaps for more information.
        /// </summary>
        /// <returns>Sitemap.xml as string</returns>
        string Generate();

        /// <summary>
        /// This will build an xml sitemap for better index with search engines.
        /// See http://en.wikipedia.org/wiki/Sitemaps for more information.
        /// </summary>
        /// <param name="stream">Stream of sitemap.</param>
        void Generate(Stream stream);

    }
}
