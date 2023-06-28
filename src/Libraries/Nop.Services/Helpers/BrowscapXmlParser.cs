using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Nop.Core.Infrastructure;

namespace Nop.Services.Helpers
{
    /// <summary>
    /// Helper class for working with XML file of Browser Capabilities Project (http://browscap.org/)
    /// </summary>
    public partial class BrowscapXmlHelper
    {
        #region Fields

        protected readonly INopFileProvider _fileProvider;
        protected Regex _crawlerUserAgentsRegexp;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="userAgentStringsPath">User agent file path</param>
        /// <param name="crawlerOnlyUserAgentStringsPath">User agent with crawlers only file path</param>
        /// <param name="additionalCrawlersFilePath">Additional crawlers user agent file path</param>
        /// <param name="fileProvider">File provider</param>
        public BrowscapXmlHelper(string userAgentStringsPath, string crawlerOnlyUserAgentStringsPath, string additionalCrawlersFilePath, INopFileProvider fileProvider)
        {
            _fileProvider = fileProvider;

            Initialize(userAgentStringsPath, crawlerOnlyUserAgentStringsPath, additionalCrawlersFilePath);
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Indicates whether the browscap item is a crawler
        /// </summary>
        /// <param name="browscapItem">Browscap item</param>
        /// <returns>True - if the item is a crawler, false otherwise</returns>
        protected virtual bool IsBrowscapItemIsCrawler(XElement browscapItem)
        {
            var el = browscapItem.Elements("item").FirstOrDefault(e => e.Attribute("name")?.Value == "Crawler");

            return el != null && el.Attribute("value")?.Value.ToLowerInvariant() == "true";
        }
        
        /// <summary>
        /// Gets the additional crawler list
        /// </summary>
        /// <returns>List of crawlers</returns>
        protected virtual IEnumerable<XElement> GetAdditionalCrawlerItems(string additionalCrawlersFilePath)
        {
            try
            {
                using var sr = new StreamReader(additionalCrawlersFilePath);

                var crawlerItems = XDocument.Load(sr).Root?.Elements("browscapitem").ToList();

                return crawlerItems;
            }
            catch
            {
                //ignore
            }

            return Enumerable.Empty<XElement>();
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="userAgentStringsPath">User agent file path</param>
        /// <param name="crawlerOnlyUserAgentStringsPath">User agent with crawlers only file path</param>
        /// <param name="additionalCrawlersFilePath">Additional crawlers user agent file path</param>
        /// <exception cref="Exception"></exception>
        protected virtual void Initialize(string userAgentStringsPath, string crawlerOnlyUserAgentStringsPath, string additionalCrawlersFilePath)
        {
            List<XElement> crawlerItems = null;
            var comments = new XElement("comments");
            var needSaveCrawlerOnly = false;

            if (!string.IsNullOrEmpty(crawlerOnlyUserAgentStringsPath) && _fileProvider.FileExists(crawlerOnlyUserAgentStringsPath))
            {
                //try to load crawler list from crawlers only file
                using var sr = new StreamReader(crawlerOnlyUserAgentStringsPath);
                crawlerItems = XDocument.Load(sr).Root?.Elements("browscapitem").ToList();
            }

            if (crawlerItems == null || !crawlerItems.Any())
            {
                //try to load crawler list from full user agents file
                using var sr = new StreamReader(userAgentStringsPath);
                var rootElement = XDocument.Load(sr).Root;
                crawlerItems = rootElement?.Element("browsercapitems")?.Elements("browscapitem")
                    //only crawlers
                    .Where(IsBrowscapItemIsCrawler).ToList();
                needSaveCrawlerOnly = true;
                comments = rootElement?.Element("comments");
            }

            if (crawlerItems == null || !crawlerItems.Any())
                throw new Exception("Incorrect file format");

            if (_fileProvider.FileExists(additionalCrawlersFilePath))
                crawlerItems.AddRange(GetAdditionalCrawlerItems(additionalCrawlersFilePath));

            var crawlerRegexpPattern = string.Join("|", crawlerItems
                //get only user agent names
                .Select(e => e.Attribute("name"))
                .Where(e => !string.IsNullOrEmpty(e?.Value))
                .Select(e => e.Value)
                .Select(attributeValue =>
                {
                    var sb = new StringBuilder(Regex.Escape(attributeValue));
                    sb.Replace("&amp;", "&").Replace("\\?", ".").Replace("\\*", ".*?");

                    return $"^{sb}$";
                }));

            _crawlerUserAgentsRegexp = new Regex(crawlerRegexpPattern);

            if ((string.IsNullOrEmpty(crawlerOnlyUserAgentStringsPath) || _fileProvider.FileExists(crawlerOnlyUserAgentStringsPath)) && !needSaveCrawlerOnly)
                return;

            //try to write crawlers file
            using var sw = new StreamWriter(crawlerOnlyUserAgentStringsPath);
            var root = new XElement("browsercapitems");

            comments?.AddFirst(new XElement("comment", new XCData("nopCommerce uses a short version of the \"browscap.xml\" file. This short version contains crawlers only. If you want to keep the crawlers list up to date, please download the full version of the original file from the official browscap site (http://browscap.org/). Please save it in the \\App_Data folder (The file name should be \"browscap.xml\"), delete \"browscap.crawlersonly.xml\", and restart the website.")));
            root.Add(comments);

            foreach (var crawler in crawlerItems)
            {
                foreach (var element in crawler.Elements().ToList())
                {
                    if ((element.Attribute("name")?.Value.ToLowerInvariant() ?? string.Empty) == "crawler")
                        continue;
                    element.Remove();
                }

                root.Add(crawler);
            }

            root.Save(sw);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether a user agent is a crawler
        /// </summary>
        /// <param name="userAgent">User agent string</param>
        /// <returns>True if user agent is a crawler, otherwise - false</returns>
        public virtual bool IsCrawler(string userAgent)
        {
            return _crawlerUserAgentsRegexp.IsMatch(userAgent);
        }

        #endregion
    }
}
