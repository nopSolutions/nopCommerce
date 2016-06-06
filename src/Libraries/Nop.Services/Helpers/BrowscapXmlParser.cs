using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Nop.Core;
using System.Text.RegularExpressions;

namespace Nop.Services.Helpers
{
    /// <summary>
    /// Helper class for working with XML file of Browser Capabilities Project (http://browscap.org/)
    /// </summary>
    public class BrowscapXmlHelper
    {
        private readonly List<string> _crawlerUserAgentsRegexp;
        private readonly List<string> _crawlerUserAgents;
        private readonly List<string> _notCrawlerUserAgents;

        public BrowscapXmlHelper(string filePath)
        {
            _crawlerUserAgents = new List<string>();
            _crawlerUserAgentsRegexp = new List<string>();
            _notCrawlerUserAgents=new List<string>();

            Initialize(filePath);
        }

        private void Initialize(string filePath)
        {
            using (var sr = new StreamReader(filePath))
            {
                var text = sr.ReadToEnd().Replace("&", "&amp;");

                var browsercapItems = XDocument.Parse(text).Root.Return(x => x.Element("browsercapitems"), null);

                if (browsercapItems == null)
                    throw new Exception("Incorrect file format");

                _crawlerUserAgentsRegexp.AddRange(browsercapItems.Elements("browscapitem")
                    //only crawlers
                    .Where(IsBrowscapItemIsCrawler)
                    //get only user agent names
                    .Select(e => e.Attribute("name").Return(a => a.Value.Replace("&amp;", "&"), ""))
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Select(ToRegexp));
            }
        }

        private static bool IsBrowscapItemIsCrawler(XElement browscapItem)
        {
            var el = browscapItem.Elements("item").FirstOrDefault(e => e.Attribute("name").Return(a => a.Value, "") == "Crawler");

            return el == null ? false : el.Attribute("value").Return(a => a.Value.ToLower() == "true", false);
        }

        private string ToRegexp(string str)
        {
            str = String.Format("^{0}$", Regex.Escape(str));
            return str.Replace("\\?", ".").Replace("\\*", ".*?");
        }

        /// <summary>
        /// Determines whether a user agent is a crawler
        /// </summary>
        /// <param name="userAgent">User agent string</param>
        /// <returns>True if user agent is a crawler, otherwise - false</returns>
        public bool IsCrawler(string userAgent)
        {
            if (_crawlerUserAgents.Any(p => p == userAgent))
                return true;
            
            if (_notCrawlerUserAgents.Any(p => p == userAgent))
                return false;

            var flag = _crawlerUserAgentsRegexp.Any(p => Regex.IsMatch(userAgent, p));
            if (flag)
                _crawlerUserAgents.Add(userAgent);
            else
                _notCrawlerUserAgents.Add(userAgent);

            return flag;
        }
    }
}
