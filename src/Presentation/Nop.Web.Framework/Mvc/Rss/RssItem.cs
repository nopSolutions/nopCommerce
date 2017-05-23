using System;
using System.Collections.Generic;
using System.Xml.Linq;

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
        /// Content
        /// </summary>
        public XElement Content { get; private set; }

        /// <summary>
        /// Link
        /// </summary>
        public XElement Link { get; private set; }

        /// <summary>
        /// Unique identifier
        /// </summary>
        public XElement Id { get; private set; }

        /// <summary>
        /// Last build date
        /// </summary>
        public XElement PubDate { get; private set; }

        /// <summary>
        /// Element extensions
        /// </summary>
        public List<XElement> ElementExtensions { get; } = new List<XElement>();
    }
}
