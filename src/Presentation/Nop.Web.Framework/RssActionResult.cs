using System.ServiceModel.Syndication;
using System.Web.Mvc;
using System.Xml;
using Nop.Core;

namespace Nop.Web.Framework
{
    public class RssActionResult : ActionResult
    {
        public SyndicationFeed Feed { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = MimeTypes.ApplicationRssXml;

            var rssFormatter = new Rss20FeedFormatter(Feed);
            using (var writer = XmlWriter.Create(context.HttpContext.Response.Output))
            {
                rssFormatter.WriteTo(writer);
            }
        }
    }
}
