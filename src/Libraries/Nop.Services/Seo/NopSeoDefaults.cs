using System.Collections.Generic;
using Nop.Core.Caching;

namespace Nop.Services.Seo
{
    /// <summary>
    /// Represents default values related to SEO services
    /// </summary>
    public static partial class NopSeoDefaults
    {
        /// <summary>
        /// Gets a max length of forum topic slug name
        /// </summary>
        /// <remarks>For long URLs we can get the following error: 
        /// "the specified path, file name, or both are too long. The fully qualified file name must be less than 260 characters, and the directory name must be less than 248 characters", 
        /// that's why we limit it to 100</remarks>
        public static int ForumTopicLength => 100;

        /// <summary>
        /// Gets a max length of search engine name
        /// </summary>
        /// <remarks>For long URLs we can get the following error: 
        /// "the specified path, file name, or both are too long. The fully qualified file name must be less than 260 characters, and the directory name must be less than 248 characters", 
        /// that's why we limit it to 200</remarks>
        public static int SearchEngineNameLength => 200;

        /// <summary>
        /// Gets a default list of slugs (seName) reserved for some other needs
        /// </summary>
        public static List<string> ReservedUrlRecordSlugs => new()
        {
            //routes that redirected customers to the specific actions
            "admin",
            "blog",
            "boards",
            "cart",
            "checkout",
            "clearcomparelist",
            "compareproducts",
            "contactus",
            "emailwishlist",
            "install",
            "login",
            "logout",
            "multi-factor-verification",
            "newproducts",
            "news",
            "onepagecheckout",
            "page-not-found",
            "passwordrecovery",
            "privatemessages",
            "profile",
            "recentlyviewedproducts",
            "register",
            "search",
            "sitemap",
            "storeclosed",
            "wishlist",
        };

        #region Sitemap

        /// <summary>
        /// Gets a date and time format for the sitemap
        /// </summary>
        public static string SitemapDateFormat => @"yyyy-MM-dd";

        /// <summary>
        /// Gets a max number of URLs in the sitemap file. At now each provided sitemap file must have no more than 50000 URLs
        /// </summary>
        public static int SitemapMaxUrlNumber => 50000;

        /// <summary>
        /// Gets the name of the sitemap directory
        /// </summary>
        public static string SitemapXmlDirectory => "sitemaps";

        /// <summary>
        /// Gets a pattern to build sitemap filename
        /// </summary>
        /// <remarks>
        /// {0} : store Id
        /// {1} : language Id
        /// {0} : sitemap index
        /// </remarks>
        public static string SitemapXmlFilePattern => "sitemap-{0}-{1}-{2}.xml";

        #endregion

        #region Caching defaults

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity ID
        /// {1} : entity name
        /// {2} : language ID
        /// </remarks>
        public static CacheKey UrlRecordCacheKey => new("Nop.urlrecord.{0}-{1}-{2}");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : slug
        /// </remarks>
        public static CacheKey UrlRecordBySlugCacheKey => new("Nop.urlrecord.byslug.{0}");

        #endregion
    }
}