using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Core.Domain.Localization;
using Nop.Services.Localization;
using Telerik.Web.Mvc.UI;
using System.Web.Mvc.Html;
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
            var resource = EngineContext.Current.Resolve<Services.Localization.ILocalizationService>().GetResource(
                    resourceName);
            builder.MergeAttribute("src", ResolveUrl(helper, "/Areas/Admin/Content/images/ico-help.gif"));
            builder.MergeAttribute("alt", resource);
            builder.MergeAttribute("title", resource);

            // Render tag
            return MvcHtmlString.Create(builder.ToString());
        }

        public static MvcHtmlString EditorForLocalized<T>(this HtmlHelper<T> helper, T model, params Expression<Func<T, string>>[] expressions)
        {
            //var tabs = helper.Telerik().TabStrip().Name("Test").Items(x =>
            //                                                                {
            //                                                                    var languages = EngineContext.Current.Resolve<ILanguageService>().GetAllLanguages();

            //                                                                    foreach (var language in languages)
            //                                                                    {
            //                                                                        var stringWriter = new StringWriter();
            //                                                                        using (var writer = new HtmlTextWriter(stringWriter))
            //                                                                        {
            //                                                                            foreach (
            //                                                                                var expression in expressions)
            //                                                                            {
            //                                                                                var template = helper.
            //                                                                                    EditorFor(
            //                                                                                        expression,
            //                                                                                        "LocalizedField", new { LanguageId = language.Id, Expression = expression}).ToHtmlString();
            //                                                                                writer.RenderBeginTag(
            //                                                                                    HtmlTextWriterTag.Div);
            //                                                                                writer.Write(template);
            //                                                                                writer.RenderEndTag();
            //                                                                            }
            //                                                                        }
            //                                                                        var content = stringWriter.ToString();
            //                                                                        x.Add().Text(language.Name).
            //                                                                                Content(content).
            //                                                                                Selected(true);
            //                                                                    }
            //                                                                });
            //return MvcHtmlString.Create(tabs.ToHtmlString());
            return null;
        }
    }
}
