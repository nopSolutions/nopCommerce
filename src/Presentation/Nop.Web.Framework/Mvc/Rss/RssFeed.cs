using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Nop.Core;

namespace Nop.Web.Framework.Mvc.Rss
{
    /// <summary>
    /// The class representing the RSS feed
    /// </summary>
    public class RssFeed
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="description">Description</param>
        /// <param name="link">Link</param>
        /// <param name="lastBuildDate">Last build date</param>
        public RssFeed(string title, string description, Uri link, DateTimeOffset lastBuildDate)
        {
            Init(title, description, link, lastBuildDate);
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="link">URL</param>
        public RssFeed(Uri link)
        {
            Init(string.Empty, string.Empty, link, DateTimeOffset.Now);
        }

        /// <summary>
        /// Initialize base filds of rss feed
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="description">Description</param>
        /// <param name="link">Link</param>
        /// <param name="lastBuildDate">Last build date</param>
        private void Init(string title, string description, Uri link, DateTimeOffset lastBuildDate)
        {
            this.Title = new XElement(NopRssDefaults.Title, title);
            this.Description = new XElement(NopRssDefaults.Description, description);
            this.Link = new XElement(NopRssDefaults.Link, link);
            this.LastBuildDate = new XElement(NopRssDefaults.LastBuildDate, lastBuildDate.ToString("r"));
        }

        /// <summary>
        /// Attribute extension
        /// </summary>
        public KeyValuePair<XmlQualifiedName, string> AttributeExtension { get; set; }

        /// <summary>
        /// Element extensions
        /// </summary>
        public List<XElement> ElementExtensions { get; } = new List<XElement>();

        /// <summary>
        /// List of rss items
        /// </summary>
        public List<RssItem> Items { get; set; } = new List<RssItem>();

        /// <summary>
        /// Title
        /// </summary>
        public XElement Title { get; private set; }

        /// <summary>
        /// Load rss feed from xml reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static RssFeed Load(XmlReader reader)
        {
            try
            {
                var document = XDocument.Load(reader);

                var channel = document.Root?.Element(NopRssDefaults.Channel);

                if (channel == null)
                    return null;

                var title = channel.Element(NopRssDefaults.Title)?.Value ?? string.Empty;
                var description = channel.Element(NopRssDefaults.Description)?.Value ?? string.Empty;
                var link = new Uri(channel.Element(NopRssDefaults.Link)?.Value ?? string.Empty);
                var lastBuildDateValue = channel.Element(NopRssDefaults.LastBuildDate)?.Value;
                var lastBuildDate = lastBuildDateValue == null ? DateTimeOffset.Now : DateTimeOffset.ParseExact(lastBuildDateValue, "r", null);

                var feed = new RssFeed(title, description, link, lastBuildDate);

                foreach (var item in channel.Elements(NopRssDefaults.Item))
                {
                    feed.Items.Add(new RssItem(item));
                }

                return feed;

            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Description
        /// </summary>
        public XElement Description { get; private set; }

        /// <summary>
        /// Link
        /// </summary>
        public XElement Link { get; private set; }

        /// <summary>
        /// Last build date
        /// </summary>
        public XElement LastBuildDate { get; private set; }

        /// <summary>
        /// Get content of RSS feed  
        /// </summary>
        /// <returns></returns>
        public string GetContent()
        {
            var document = new XDocument();
            var root = new XElement(NopRssDefaults.RSS, new XAttribute("version", "2.0"));
            var channel = new XElement(NopRssDefaults.Channel,
                new XAttribute(XName.Get(AttributeExtension.Key.Name, AttributeExtension.Key.Namespace), AttributeExtension.Value));

            channel.Add(Title, Description, Link, LastBuildDate);

            foreach (var element in ElementExtensions)
            {
                channel.Add(element);
            }

            foreach (var item in Items)
            {
                channel.Add(item.ToXElement());
            }

            root.Add(channel);
            document.Add(root);

            return document.ToString();
        }
    }
}
