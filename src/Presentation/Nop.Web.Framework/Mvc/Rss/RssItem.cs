using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Nop.Core;

namespace Nop.Web.Framework.Mvc.Rss
{
    /// <summary>
    /// The class representing the item of RSS feed
    /// </summary>
    public class RssItem
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="content">Content</param>
        /// <param name="link">Link</param>
        /// <param name="id">Unique identifier</param>
        /// <param name="pubDate">Last build date</param>
        public RssItem(string title, string content, Uri link, string id, DateTimeOffset pubDate)
        {
            Init(title, content, link, id, pubDate);
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="item">XML view of rss item</param>
        public RssItem(XContainer item)
        {
            var title = item.Element("title")?.Value ?? string.Empty;
            var content = item.Element("content")?.Value ?? string.Empty;
            if (string.IsNullOrEmpty(content))
                content = item.Element("description")?.Value ?? string.Empty;
            var link = new Uri(item.Element("link")?.Value ?? string.Empty);
            var pubDateValue = item.Element("pubDate")?.Value;
            var pubDate = pubDateValue == null ? DateTimeOffset.Now : DateTimeOffset.ParseExact(pubDateValue, "r", null);
            var id = item.Element("guid")?.Value ?? string.Empty;

            Init(title, content, link, id, pubDate);
        }

        /// <summary>
        /// Initialize base filds of rss item
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="content">Content</param>
        /// <param name="link">Link</param>
        /// <param name="id">Unique identifier</param>
        /// <param name="pubDate">Last build date</param>
        private void Init(string title, string content, Uri link, string id, DateTimeOffset pubDate)
        {
            this.Title = new XElement("title", title);
            this.Content = new XElement("description", content);
            this.Link = new XElement("link", link);
            this.Id = new XElement("guid", new XAttribute("isPermaLink", false), id);
            this.PubDate = new XElement("pubDate", pubDate.ToString("r"));
        }

        /// <summary>
        /// Get representation item of RSS feed as XElement object
        /// </summary>
        /// <returns></returns>
        public XElement ToXElement()
        {
            var element = new XElement("item", Id, Link, Title, Content);

            foreach (var elementExtensions in ElementExtensions)
            {
                element.Add(elementExtensions);
            }

            return element;
        }

        /// <summary>
        /// Title
        /// </summary>
        public XElement Title { get; private set; }

        /// <summary>
        /// Get title text
        /// </summary>
        public string TitleText => Title?.Value ?? string.Empty;

        /// <summary>
        /// Content
        /// </summary>
        public XElement Content { get; private set; }

        /// <summary>
        /// Get content text
        /// </summary>
        public string ContentText => XmlHelper.XmlDecode(Content?.Value ?? string.Empty);

        /// <summary>
        /// Link
        /// </summary>
        public XElement Link { get; private set; }

        /// <summary>
        /// Get URL
        /// </summary>
        public Uri Url => new Uri(Link.Value);

        /// <summary>
        /// Unique identifier
        /// </summary>
        public XElement Id { get; private set; }

        /// <summary>
        /// Last build date
        /// </summary>
        public XElement PubDate { get; private set; }

        /// <summary>
        /// Publish date
        /// </summary>
        public DateTimeOffset PublishDate => PubDate?.Value == null ? DateTimeOffset.Now : DateTimeOffset.ParseExact(PubDate.Value, "r", null);

        /// <summary>
        /// Element extensions
        /// </summary>
        public List<XElement> ElementExtensions { get; } = new List<XElement>();
    }
}
