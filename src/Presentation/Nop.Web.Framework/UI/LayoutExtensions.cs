using System.Web.Mvc;
using Nop.Core.Infrastructure;

namespace Nop.Web.Framework.UI
{
    public static class LayoutExtensions
    {
        public static void AddTitleParts(this HtmlHelper html, params string[] parts)
        {
            var pageHeadBuilder  = EngineContext.Current.Resolve<IPageHeadBuilder>();
            pageHeadBuilder.AddTitleParts(parts);
        }
        public static void AppendTitleParts(this HtmlHelper html, params string[] parts)
        {
            var pageHeadBuilder  = EngineContext.Current.Resolve<IPageHeadBuilder>();
            pageHeadBuilder.AppendTitleParts(parts);
        }
        public static MvcHtmlString NopTitle(this HtmlHelper html, bool addDefaultTitle, params string[] parts)
        {
            var pageHeadBuilder = EngineContext.Current.Resolve<IPageHeadBuilder>();
            html.AppendTitleParts(parts);
            return MvcHtmlString.Create(html.Encode(pageHeadBuilder.GenerateTitle(addDefaultTitle)));
        }


        public static void AddMetaDescriptionParts(this HtmlHelper html, params string[] parts)
        {
            var pageHeadBuilder = EngineContext.Current.Resolve<IPageHeadBuilder>();
            pageHeadBuilder.AddMetaDescriptionParts(parts);
        }
        public static void AppendMetaDescriptionParts(this HtmlHelper html, params string[] parts)
        {
            var pageHeadBuilder = EngineContext.Current.Resolve<IPageHeadBuilder>();
            pageHeadBuilder.AppendMetaDescriptionParts(parts);
        }
        public static MvcHtmlString NopMetaDescription(this HtmlHelper html, params string[] parts)
        {
            var pageHeadBuilder = EngineContext.Current.Resolve<IPageHeadBuilder>();
            html.AppendMetaDescriptionParts(parts);
            return MvcHtmlString.Create(html.Encode(pageHeadBuilder.GenerateMetaDescription()));
        }


        public static void AddMetaKeywordParts(this HtmlHelper html, params string[] parts)
        {
            var pageHeadBuilder = EngineContext.Current.Resolve<IPageHeadBuilder>();
            pageHeadBuilder.AddMetaKeywordParts(parts);
        }
        public static void AppendMetaKeywordParts(this HtmlHelper html, params string[] parts)
        {
            var pageHeadBuilder = EngineContext.Current.Resolve<IPageHeadBuilder>();
            pageHeadBuilder.AppendMetaKeywordParts(parts);
        }
        public static MvcHtmlString NopMetaKeywords(this HtmlHelper html, params string[] parts)
        {
            var pageHeadBuilder = EngineContext.Current.Resolve<IPageHeadBuilder>();
            html.AppendMetaKeywordParts(parts);
            return MvcHtmlString.Create(html.Encode(pageHeadBuilder.GenerateMetaKeywords()));
        }



        public static void AddScriptParts(this HtmlHelper html, params string[] parts)
        {
            AddScriptParts(html, ResourceLocation.Head, parts);
        }
        public static void AddScriptParts(this HtmlHelper html, ResourceLocation location, params string[] parts)
        {
            var pageHeadBuilder = EngineContext.Current.Resolve<IPageHeadBuilder>();
            pageHeadBuilder.AddScriptParts(location, parts);
        }
        public static void AppendScriptParts(this HtmlHelper html, params string[] parts)
        {
            AppendScriptParts(html, ResourceLocation.Head, parts);
        }
        public static void AppendScriptParts(this HtmlHelper html, ResourceLocation location, params string[] parts)
        {
            var pageHeadBuilder = EngineContext.Current.Resolve<IPageHeadBuilder>();
            pageHeadBuilder.AppendScriptParts(location, parts);
        }
        public static MvcHtmlString NopScripts(this HtmlHelper html, UrlHelper urlHelper, 
            ResourceLocation location, bool? bundleFiles = null)
        {
            var pageHeadBuilder = EngineContext.Current.Resolve<IPageHeadBuilder>();
            return MvcHtmlString.Create(pageHeadBuilder.GenerateScripts(urlHelper, location, bundleFiles));
        }



        public static void AddCssFileParts(this HtmlHelper html, params string[] parts)
        {
            AddCssFileParts(html, ResourceLocation.Head, parts);
        }
        public static void AddCssFileParts(this HtmlHelper html, ResourceLocation location, params string[] parts)
        {
            var pageHeadBuilder = EngineContext.Current.Resolve<IPageHeadBuilder>();
            pageHeadBuilder.AddCssFileParts(location, parts);
        }
        public static void AppendCssFileParts(this HtmlHelper html, params string[] parts)
        {
            AppendCssFileParts(html, ResourceLocation.Head, parts);
        }
        public static void AppendCssFileParts(this HtmlHelper html, ResourceLocation location, params string[] parts)
        {
            var pageHeadBuilder = EngineContext.Current.Resolve<IPageHeadBuilder>();
            pageHeadBuilder.AppendCssFileParts(location, parts);
        }
        public static MvcHtmlString NopCssFiles(this HtmlHelper html, UrlHelper urlHelper, 
            ResourceLocation location)
        {
            var pageHeadBuilder = EngineContext.Current.Resolve<IPageHeadBuilder>();
            return MvcHtmlString.Create(pageHeadBuilder.GenerateCssFiles(urlHelper, location));
        }



        public static void AddCanonicalUrlParts(this HtmlHelper html, params string[] parts)
        {
            var pageHeadBuilder = EngineContext.Current.Resolve<IPageHeadBuilder>();
            pageHeadBuilder.AddCanonicalUrlParts(parts);
        }
        public static void AppendCanonicalUrlParts(this HtmlHelper html, params string[] parts)
        {
            var pageHeadBuilder = EngineContext.Current.Resolve<IPageHeadBuilder>();
            pageHeadBuilder.AppendCanonicalUrlParts(parts);
        }
        public static MvcHtmlString NopCanonicalUrls(this HtmlHelper html, params string[] parts)
        {
            var pageHeadBuilder = EngineContext.Current.Resolve<IPageHeadBuilder>();
            html.AppendCanonicalUrlParts(parts);
            return MvcHtmlString.Create(pageHeadBuilder.GenerateCanonicalUrls());
        }
    }
}
