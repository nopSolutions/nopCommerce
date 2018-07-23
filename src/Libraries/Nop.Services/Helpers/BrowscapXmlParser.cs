using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Nop.Core.Infrastructure;

namespace Nop.Services.Helpers
{
    /// <summary>
    /// Helper class for working with XML file of Browser Capabilities Project (http://browscap.org/)
    /// </summary>
    public class BrowscapXmlHelper
    {
        private readonly List<string> _crawlerUserAgentsRegexp;
        private readonly INopFileProvider _fileProvider;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="userAgentStringsPath">User agent file path</param>
        /// <param name="crawlerOnlyUserAgentStringsPath">User agent with crawlers only file path</param>
        /// <param name="fileProvider">File provider</param>
        public BrowscapXmlHelper(string userAgentStringsPath, string crawlerOnlyUserAgentStringsPath, INopFileProvider fileProvider)
        {
            this._crawlerUserAgentsRegexp = new List<string>();
            this._fileProvider = fileProvider;

            Initialize(userAgentStringsPath, crawlerOnlyUserAgentStringsPath);
        }

        private static bool IsBrowscapItemIsCrawler(XElement browscapItem)
        {
            var el = browscapItem.Elements("item").FirstOrDefault(e => e.Attribute("name")?.Value == "Crawler");

            return el != null && el.Attribute("value")?.Value.ToLower() == "true";
        }

        private static string ToRegexp(string str)
        {
            var sb = new StringBuilder(Regex.Escape(str));
            sb.Replace("&amp;", "&").Replace("\\?", ".").Replace("\\*", ".*?");
            return $"^{sb}$";
        }

        private void Initialize(string userAgentStringsPath, string crawlerOnlyUserAgentStringsPath)
        {
            List<XElement> crawlerItems = null;
            var needSaveCrawlerOnly = false;

            if (!string.IsNullOrEmpty(crawlerOnlyUserAgentStringsPath) && _fileProvider.FileExists(crawlerOnlyUserAgentStringsPath))
            {
                //try to load crawler list from crawlers only file
                using (var sr = new StreamReader(crawlerOnlyUserAgentStringsPath))
                {
                    crawlerItems = XDocument.Load(sr).Root?.Elements("browscapitem").ToList();
                }
            }

            if (crawlerItems == null || !crawlerItems.Any())
            {
                //try to load crawler list from full user agents file
                using (var sr = new StreamReader(userAgentStringsPath))
                {
                    crawlerItems = XDocument.Load(sr).Root?.Element("browsercapitems")?.Elements("browscapitem")
                        //only crawlers
                        .Where(IsBrowscapItemIsCrawler).ToList();
                    needSaveCrawlerOnly = true;
                }
            }

            if (crawlerItems == null || !crawlerItems.Any())
                throw new Exception("Incorrect file format");

            _crawlerUserAgentsRegexp.AddRange(crawlerItems
                //get only user agent names
                .Select(e => e.Attribute("name"))
                .Where(e => !string.IsNullOrEmpty(e?.Value))
                .Select(e => e.Value)
                .Select(ToRegexp));

            if ((string.IsNullOrEmpty(crawlerOnlyUserAgentStringsPath) || _fileProvider.FileExists(crawlerOnlyUserAgentStringsPath)) && !needSaveCrawlerOnly)
                return;

            //try to write crawlers file
            using (var sw = new StreamWriter(crawlerOnlyUserAgentStringsPath))
            {
                var root = new XElement("browsercapitems");

                foreach (var crawler in crawlerItems)
                {
                    foreach (var element in crawler.Elements().ToList())
                    {
                        if ((element.Attribute("name")?.Value.ToLower() ?? string.Empty) == "crawler")
                            continue;
                        element.Remove();
                    }

                    root.Add(crawler);
                }

                root.Save(sw);
            }
        }
        
        /// <summary>
        /// Determines whether a user agent is a crawler
        /// </summary>
        /// <param name="userAgent">User agent string</param>
        /// <returns>True if user agent is a crawler, otherwise - false</returns>
        public bool IsCrawler(string userAgent)
        {
            return _crawlerUserAgentsRegexp.Any(p => Regex.IsMatch(userAgent, p));
        }
    }
}
