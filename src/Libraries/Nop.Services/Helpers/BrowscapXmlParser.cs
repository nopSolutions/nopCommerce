using Nop.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Linq;

namespace Nop.Services.Helpers
{
    /// <summary>
    /// Helper class for working with XML file of Browser Capabilities Project (http://browscap.org/)
    /// </summary>
    public class BrowscapXmlHelper
    {
        private readonly List<string> _crawlerUserAgentsRegexp;
        private ConcurrentDictionary<string, DateTime> _foundCrawlersCache;     // user agents of found crawlers
        private ConcurrentQueue<Tuple<string, DateTime>> _expireOnIndexCache;   // queue of expireOns used to optimize when clean up of cache should occur
        private Object _lock;

        // TODO: move consts to configurable setting for tuning
        public const int FOUND_CRAWLER_CACHE_MAX_COUNT = 10000; 
        public const int CRAWLER_INDEX_EXPIRE_ON_MINS = 4320; // expire cache items after 3 days
       


        public BrowscapXmlHelper(string userAgentStringsPath, string crawlerOnlyUserAgentStringsPath)
        {
            _crawlerUserAgentsRegexp = new List<string>();

            Initialize(userAgentStringsPath, crawlerOnlyUserAgentStringsPath);

            this._foundCrawlersCache = new ConcurrentDictionary<string, DateTime>();
            this._expireOnIndexCache = new ConcurrentQueue<Tuple<string, DateTime>>();
            this._lock = new Object();
        }

        private void Initialize(string userAgentStringsPath, string crawlerOnlyUserAgentStringsPath)
        {
            List<XElement> crawlerItems = null;

            if (!string.IsNullOrEmpty(crawlerOnlyUserAgentStringsPath) && File.Exists(crawlerOnlyUserAgentStringsPath))
            {
                //try to load crawler list from crawlers only file
                using (var sr = new StreamReader(crawlerOnlyUserAgentStringsPath))
                {
                    crawlerItems = XDocument.Load(sr).Root.Return(x => x.Elements("browscapitem").ToList(), null);
                }
            }

            if (crawlerItems == null)
            {
                //try to load crawler list from full user agents file
                using (var sr = new StreamReader(userAgentStringsPath))
                {
                    crawlerItems = XDocument.Load(sr).Root.Return(x => x.Element("browsercapitems"), null)
                        //only crawlers
                        .Return(x => x.Elements("browscapitem").Where(IsBrowscapItemIsCrawler).ToList(), null);
                }
            }

            if (crawlerItems == null)
                throw new Exception("Incorrect file format");

            _crawlerUserAgentsRegexp.AddRange(crawlerItems
                //get only user agent names
                .Select(e => e.Attribute("name"))
                .Where(e => e != null && !string.IsNullOrEmpty(e.Value))
                .Select(e => e.Value)
                .Select(ToRegexp));

            if (string.IsNullOrEmpty(crawlerOnlyUserAgentStringsPath) || File.Exists(crawlerOnlyUserAgentStringsPath))
                return;

            //try to write crawlers file
            using (var sw = new StreamWriter(crawlerOnlyUserAgentStringsPath))
            {
                var root = new XElement("browsercapitems");

                foreach (var crawler in crawlerItems)
                {
                    foreach (var element in crawler.Elements().ToList())
                    {
                        if (element.Attribute("name").Return(x => x.Value.ToLower(), string.Empty) == "crawler")
                            continue;
                        element.Remove();
                    }

                    root.Add(crawler);
                }
                root.Save(sw);
            }
        }

        private static bool IsBrowscapItemIsCrawler(XElement browscapItem)
        {
            var el = browscapItem.Elements("item").FirstOrDefault(e => e.Attribute("name").Return(a => a.Value, "") == "Crawler");

            return el != null && el.Attribute("value").Return(a => a.Value.ToLower() == "true", false);
        }

        private static string ToRegexp(string str)
        {
            var sb = new StringBuilder(Regex.Escape(str));
            sb.Replace("&amp;", "&").Replace("\\?", ".").Replace("\\*", ".*?");
            return string.Format("^{0}$", sb);
        }

        /// <summary>
        /// Determines whether a user agent is a crawler
        /// </summary>
        /// <param name="userAgent">User agent string</param>
        /// <returns>True if user agent is a crawler, otherwise - false</returns>
        public bool IsCrawler(string userAgent)
        {
            // check in already found cache
            DateTime expireOn;
            if (_foundCrawlersCache.TryGetValue(userAgent, out expireOn))
            {
                if (expireOn <= DateTime.UtcNow)
                    _foundCrawlersCache.TryRemove(userAgent, out expireOn);
                return true;
            }

            bool result = _crawlerUserAgentsRegexp.Any(p => Regex.IsMatch(userAgent, p));

            // if IS search engine, save to cache
            if (result)
            {
                if (_foundCrawlersCache.Count < FOUND_CRAWLER_CACHE_MAX_COUNT)
                {
                    expireOn = DateTime.UtcNow.AddMinutes(CRAWLER_INDEX_EXPIRE_ON_MINS);
                    _foundCrawlersCache.TryAdd(userAgent, expireOn);
                    _expireOnIndexCache.Enqueue(Tuple.Create(userAgent, expireOn));
                }
                else
                {
                    TryExpireCacheItems();
                }
            }

            return result;
        }



        /// <summary>
        /// Removes expired items from cache. 
        /// </summary>
        /// <remarks>
        /// Concurrent object try failures are OK since this is not a critical operation, the worst case is
        /// that the cache will continue to stay full, however subsequent calls should eventually expire
        /// the old items out.
        /// </remarks>
        public void TryExpireCacheItems()
        {
            if (Monitor.TryEnter(_lock))
            {
                try
                {
                    var isCacheFull = _foundCrawlersCache.Count >= FOUND_CRAWLER_CACHE_MAX_COUNT;
                    if (!isCacheFull) return;

                    Tuple<string, DateTime> expireOnData;
                    _expireOnIndexCache.TryPeek(out expireOnData);
                    var hasExpirableItem = (expireOnData != null && expireOnData.Item2 < DateTime.UtcNow);

                    if (hasExpirableItem)
                    {
                        var dtUtcNow = DateTime.UtcNow;
                        DateTime expireOn; // used as out var for crawler cache removal below
                        while (expireOnData.Item2 < dtUtcNow)
                        {
                            // dequeue from index
                            _expireOnIndexCache.TryDequeue(out expireOnData);
                            // remove from cache using dequeued item from index
                            _foundCrawlersCache.TryRemove(expireOnData.Item1, out expireOn);
                            if (_expireOnIndexCache.IsEmpty) break;

                            // check next item in queue 
                            _expireOnIndexCache.TryPeek(out expireOnData);
                        }
                    }             
                }
                finally
                {
                    Monitor.Exit(_lock);
                }
            }

        }
    }
}