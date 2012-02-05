using System.Web.Mvc;
using Nop.Core.Infrastructure;

namespace Nop.Web.Framework.UI
{
    public static class LayoutExtensions
    {
        public static void AddTitleParts(this HtmlHelper html, params string[] parts)
        {
            var pageTitleBuilder  = EngineContext.Current.Resolve<IPageTitleBuilder>();
            pageTitleBuilder.AddTitleParts(parts);
        }
        public static void AppendTitleParts(this HtmlHelper html, params string[] parts)
        {
            var pageTitleBuilder  = EngineContext.Current.Resolve<IPageTitleBuilder>();
            pageTitleBuilder.AppendTitleParts(parts);
        }
        public static MvcHtmlString NopTitle(this HtmlHelper html, bool addDefaultTitle, params string[] parts)
        {
            var pageTitleBuilder = EngineContext.Current.Resolve<IPageTitleBuilder>();
            html.AppendTitleParts(parts);
            return MvcHtmlString.Create(html.Encode(pageTitleBuilder.GenerateTitle(addDefaultTitle)));
        }


        public static void AddMetaDescriptionParts(this HtmlHelper html, params string[] parts)
        {
            var pageTitleBuilder = EngineContext.Current.Resolve<IPageTitleBuilder>();
            pageTitleBuilder.AddMetaDescriptionParts(parts);
        }
        public static void AppendMetaDescriptionParts(this HtmlHelper html, params string[] parts)
        {
            var pageTitleBuilder = EngineContext.Current.Resolve<IPageTitleBuilder>();
            pageTitleBuilder.AppendMetaDescriptionParts(parts);
        }
        public static MvcHtmlString NopMetaDescription(this HtmlHelper html, params string[] parts)
        {
            var pageTitleBuilder = EngineContext.Current.Resolve<IPageTitleBuilder>();
            html.AppendMetaDescriptionParts(parts);
            return MvcHtmlString.Create(html.Encode(pageTitleBuilder.GenerateMetaDescription()));
        }


        public static void AddMetaKeywordParts(this HtmlHelper html, params string[] parts)
        {
            var pageTitleBuilder = EngineContext.Current.Resolve<IPageTitleBuilder>();
            pageTitleBuilder.AddMetaKeywordParts(parts);
        }
        public static void AppendMetaKeywordParts(this HtmlHelper html, params string[] parts)
        {
            var pageTitleBuilder = EngineContext.Current.Resolve<IPageTitleBuilder>();
            pageTitleBuilder.AppendMetaKeywordParts(parts);
        }
        public static MvcHtmlString NopMetaKeywords(this HtmlHelper html, params string[] parts)
        {
            var pageTitleBuilder = EngineContext.Current.Resolve<IPageTitleBuilder>();
            html.AppendMetaKeywordParts(parts);
            return MvcHtmlString.Create(html.Encode(pageTitleBuilder.GenerateMetaKeywords()));
        }



        public static void AddScriptParts(this HtmlHelper html, params string[] parts)
        {
            AddScriptParts(html, ResourceLocation.Head, parts);
        }
        public static void AddScriptParts(this HtmlHelper html, ResourceLocation location, params string[] parts)
        {
            var pageTitleBuilder = EngineContext.Current.Resolve<IPageTitleBuilder>();
            pageTitleBuilder.AddScriptParts(location, parts);
        }
        public static void AppendScriptParts(this HtmlHelper html, params string[] parts)
        {
            AppendScriptParts(html, ResourceLocation.Head, parts);
        }
        public static void AppendScriptParts(this HtmlHelper html, ResourceLocation location, params string[] parts)
        {
            var pageTitleBuilder = EngineContext.Current.Resolve<IPageTitleBuilder>();
            pageTitleBuilder.AppendScriptParts(location, parts);
        }
        public static MvcHtmlString NopScripts(this HtmlHelper html,  params string[] parts)
        {
            return NopScripts(html, ResourceLocation.Head, parts);
        }
        public static MvcHtmlString NopScripts(this HtmlHelper html, ResourceLocation location, params string[] parts)
        {
            var pageTitleBuilder = EngineContext.Current.Resolve<IPageTitleBuilder>();
            html.AppendScriptParts(parts);
            return MvcHtmlString.Create(pageTitleBuilder.GenerateScripts(location));
        }



        public static void AddCssFileParts(this HtmlHelper html, params string[] parts)
        {
            AddCssFileParts(html, ResourceLocation.Head, parts);
        }
        public static void AddCssFileParts(this HtmlHelper html, ResourceLocation location, params string[] parts)
        {
            var pageTitleBuilder = EngineContext.Current.Resolve<IPageTitleBuilder>();
            pageTitleBuilder.AddCssFileParts(location, parts);
        }
        public static void AppendCssFileParts(this HtmlHelper html, params string[] parts)
        {
            AppendCssFileParts(html, ResourceLocation.Head, parts);
        }
        public static void AppendCssFileParts(this HtmlHelper html, ResourceLocation location, params string[] parts)
        {
            var pageTitleBuilder = EngineContext.Current.Resolve<IPageTitleBuilder>();
            pageTitleBuilder.AppendCssFileParts(location, parts);
        }
        public static MvcHtmlString NopCssFiles(this HtmlHelper html, params string[] parts)
        {
            return NopCssFiles(html, ResourceLocation.Head, parts);
        }
        public static MvcHtmlString NopCssFiles(this HtmlHelper html, ResourceLocation location, params string[] parts)
        {
            var pageTitleBuilder = EngineContext.Current.Resolve<IPageTitleBuilder>();
            html.AppendCssFileParts(parts);
            return MvcHtmlString.Create(pageTitleBuilder.GenerateCssFiles(location));
        }



        public static void AddCanonicalUrlParts(this HtmlHelper html, params string[] parts)
        {
            var pageTitleBuilder = EngineContext.Current.Resolve<IPageTitleBuilder>();
            pageTitleBuilder.AddCanonicalUrlParts(parts);
        }
        public static void AppendCanonicalUrlParts(this HtmlHelper html, params string[] parts)
        {
            var pageTitleBuilder = EngineContext.Current.Resolve<IPageTitleBuilder>();
            pageTitleBuilder.AppendCanonicalUrlParts(parts);
        }
        public static MvcHtmlString NopCanonicalUrls(this HtmlHelper html, params string[] parts)
        {
            var pageTitleBuilder = EngineContext.Current.Resolve<IPageTitleBuilder>();
            html.AppendCanonicalUrlParts(parts);
            return MvcHtmlString.Create(pageTitleBuilder.GenerateCanonicalUrls());
        }
    }
}
