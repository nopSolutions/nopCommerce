#if NET451
using System;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using Nop.Core;

namespace Nop.Web.Framework
{
    public class RssActionResult : ActionResult
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="feed">Syndication feed</param>
        /// <param name="feedPageUrl">Feed page url for atom self link</param>
        public RssActionResult(SyndicationFeed feed, string feedPageUrl)
        {
            this.Feed = feed;
            //add atom namespace
            XNamespace atom = "http://www.w3.org/2005/Atom";
            this.Feed.AttributeExtensions.Add(new XmlQualifiedName("atom", XNamespace.Xmlns.NamespaceName), atom.NamespaceName);
            //add atom:link with rel='self' 
            this.Feed.ElementExtensions.Add(new XElement(atom + "link", new XAttribute("href", new Uri(feedPageUrl)), new XAttribute("rel", "self"), new XAttribute("type", "application/rss+xml")));
        }
        public SyndicationFeed Feed { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = MimeTypes.ApplicationRssXml;

            var rssFormatter = Feed.GetRss20Formatter();
            //remove a10 namespace
            rssFormatter.SerializeExtensionsAsAtom = false;

            using (var writer = XmlWriter.Create(context.HttpContext.Response.Output))
            {
                rssFormatter.WriteTo(writer);
            }
        }
    }
}
#endif
