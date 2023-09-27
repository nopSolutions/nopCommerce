using System.Xml;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Rss;

namespace Nop.Web.Framework.Mvc
{
    /// <summary>
    /// RSS action result
    /// </summary>
    public partial class RssActionResult : ContentResult
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="feed">Syndication feed</param>
        /// <param name="feedPageUrl">Feed page url for atom self link</param>
        public RssActionResult(RssFeed feed, string feedPageUrl)
        {
            ContentType = "application/atom+xml";
            Feed = feed;

            //add atom namespace
            XNamespace atom = "http://www.w3.org/2005/Atom";
            Feed.AttributeExtension = new KeyValuePair<XmlQualifiedName, string>(new XmlQualifiedName("atom", XNamespace.Xmlns.NamespaceName), atom.NamespaceName);
            //add atom:link with rel='self' 
            Feed.ElementExtensions.Add(new XElement(atom + "link", new XAttribute("href", new Uri(feedPageUrl)), new XAttribute("rel", "self"), new XAttribute("type", "application/rss+xml")));
        }

        /// <summary>
        /// Feed
        /// </summary>
        public RssFeed Feed { get; set; }

        /// <summary>
        /// Execute result async
        /// </summary>
        /// <param name="context">ActionContext</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override Task ExecuteResultAsync(ActionContext context)
        {
            Content = Feed.GetContent();
            return base.ExecuteResultAsync(context);
        }

        /// <summary>
        /// Execute result
        /// </summary>
        /// <param name="context">ActionContext</param>
        public override void ExecuteResult(ActionContext context)
        {
            Content = Feed.GetContent();
            base.ExecuteResult(context);
        }
    }
}