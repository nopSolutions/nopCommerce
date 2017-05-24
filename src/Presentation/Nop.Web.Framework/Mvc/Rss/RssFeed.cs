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
            this.Title = new XElement("title", title);
            this.Description = new XElement("description", description);
            this.Link = new XElement("link", link);
            this.LastBuildDate = new XElement("lastBuildDate", lastBuildDate.ToString("r"));
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
            var root = new XElement("rss", new XAttribute("version", "2.0"));
            var channel = new XElement("channel",
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

            return XmlHelper.XmlDecode(document.ToString());
        }
    }
}
