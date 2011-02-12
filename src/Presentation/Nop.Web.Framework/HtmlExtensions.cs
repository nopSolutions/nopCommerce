using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nop.Web.Framework
{
    public static class HtmlExtensions
    {
        public static string ResolveUrl(this HtmlHelper helper, string url)
        {
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
            return urlHelper.Content(url);
        }
        public static MvcHtmlString Hint(this HtmlHelper helper, string resourceName)
        {
            // Create tag builder
            var builder = new TagBuilder("img");

            // Add attributes
            var resource =
                DependencyResolver.Current.GetService<Services.Localization.ILocalizationService>().GetResource(
                    resourceName);
            builder.MergeAttribute("src", ResolveUrl(helper, "/Areas/Admin/Content/images/ico-help.gif"));
            builder.MergeAttribute("alt", resource);
            builder.MergeAttribute("title", resource);

            // Render tag
            return MvcHtmlString.Create(builder.ToString());
        }
    }
}
