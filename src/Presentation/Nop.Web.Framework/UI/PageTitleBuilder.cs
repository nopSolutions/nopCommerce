using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using Nop.Core.Domain.Seo;
using Nop.Services.Seo;

namespace Nop.Web.Framework.UI
{
    public partial class PageTitleBuilder : IPageTitleBuilder
    {
        #region Fields

        private static readonly object s_lock = new object();

        private readonly SeoSettings _seoSettings;
        private readonly List<string> _titleParts;
        private readonly List<string> _metaDescriptionParts;
        private readonly List<string> _metaKeywordParts;
        private readonly Dictionary<ResourceLocation, List<string>> _scriptParts;
        private readonly Dictionary<ResourceLocation, List<string>> _cssParts;
        private readonly List<string> _canonicalUrlParts;
        private readonly HttpContextBase _httpContext;
        #endregion

        #region Ctor

        public PageTitleBuilder(SeoSettings seoSettings, HttpContextBase httpContext)
        {
            this._seoSettings = seoSettings;
            this._titleParts = new List<string>();
            this._metaDescriptionParts = new List<string>();
            this._metaKeywordParts = new List<string>();
            this._scriptParts = new Dictionary<ResourceLocation, List<string>>();
            this._cssParts = new Dictionary<ResourceLocation, List<string>>();
            this._canonicalUrlParts = new List<string>();
            this._httpContext = httpContext;
        }

        #endregion

        #region Utilities

        protected string GetBundleVirtualPath(string prefix, string postfix, string[] parts)
        {
            if (parts == null || parts.Length == 0)
                throw new ArgumentException("parts");

            //calculate hash
            var hash = "";
            using (SHA256 sha = new SHA256Managed())
            {
                // string concatenation
                var hashInput = "";
                foreach (var part in parts)
                {
                    hashInput += part;
                    hashInput += ",";
                }

                byte[] input = sha.ComputeHash(Encoding.Unicode.GetBytes(hashInput));
                hash = HttpServerUtility.UrlTokenEncode(input);
            }
            //ensure only valid chars
            hash = SeoExtensions.GetSeName(hash);

            var sb = new StringBuilder(prefix);
            sb.Append(hash);
            sb.Append(postfix);
            return sb.ToString();
        }

        #endregion

        #region Methods

        public void AddTitleParts(params string[] parts)
        {
            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _titleParts.Add(part);
        }
        public void AppendTitleParts(params string[] parts)
        {
            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _titleParts.Insert(0, part);
        }
        public string GenerateTitle(bool addDefaultTitle)
        {
            string result = "";
            var specificTitle = string.Join(_seoSettings.PageTitleSeparator, _titleParts.AsEnumerable().Reverse().ToArray());
            if (!String.IsNullOrEmpty(specificTitle))
            {
                if (addDefaultTitle)
                {
                    //store name + page title
                    switch (_seoSettings.PageTitleSeoAdjustment)
                    {
                        case PageTitleSeoAdjustment.PagenameAfterStorename:
                            {
                                result = string.Join(_seoSettings.PageTitleSeparator, _seoSettings.DefaultTitle, specificTitle);
                            }
                            break;
                        case PageTitleSeoAdjustment.StorenameAfterPagename:
                        default:
                            {
                                result = string.Join(_seoSettings.PageTitleSeparator, specificTitle, _seoSettings.DefaultTitle);
                            }
                            break;
                            
                    }
                }
                else
                {
                    //page title only
                    result = specificTitle;
                }
            }
            else
            {
                //store name only
                result = _seoSettings.DefaultTitle;
            }
            return result;
        }


        public void AddMetaDescriptionParts(params string[] parts)
        {
            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _metaDescriptionParts.Add(part);
        }
        public void AppendMetaDescriptionParts(params string[] parts)
        {
            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _metaDescriptionParts.Insert(0, part);
        }
        public string GenerateMetaDescription()
        {
            var metaDescription = string.Join(", ", _metaDescriptionParts.AsEnumerable().Reverse().ToArray());
            var result = !String.IsNullOrEmpty(metaDescription) ? metaDescription : _seoSettings.DefaultMetaDescription;
            return result;
        }


        public void AddMetaKeywordParts(params string[] parts)
        {
            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _metaKeywordParts.Add(part);
        }
        public void AppendMetaKeywordParts(params string[] parts)
        {
            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _metaKeywordParts.Insert(0, part);
        }
        public string GenerateMetaKeywords()
        {
            var metaKeyword = string.Join(", ", _metaKeywordParts.AsEnumerable().Reverse().ToArray());
            var result = !String.IsNullOrEmpty(metaKeyword) ? metaKeyword : _seoSettings.DefaultMetaKeywords;
            return result;
        }


        public void AddScriptParts(ResourceLocation location, params string[] parts)
        {
            if (!_scriptParts.ContainsKey(location))
                _scriptParts.Add(location, new List<string>());

            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _scriptParts[location].Add(part);
        }
        public void AppendScriptParts(ResourceLocation location, params string[] parts)
        {
            if (!_scriptParts.ContainsKey(location))
                _scriptParts.Add(location, new List<string>());

            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _scriptParts[location].Insert(0, part);
        }
        public string GenerateScripts(UrlHelper urlHelper, ResourceLocation location, bool? bundleFiles = null)
        {
            if (!_scriptParts.ContainsKey(location) || _scriptParts[location] == null)
                return "";
            
            //use only distinct rows
            var distinctParts = _scriptParts[location].Distinct().ToList();
            if (distinctParts.Count == 0)
                return "";
            
            if (!bundleFiles.HasValue)
            {
                //use setting if not value is specified
                bundleFiles = _seoSettings.EnableJsBundling;
            }
            if (bundleFiles.Value)
            {
                //IMPORTANT: Do not use bundling in web farms or Windows Azure
                string bundleVirtualPath = GetBundleVirtualPath("~/bundles/scripts/", ".js", distinctParts.ToArray());
                //System.Web.Optimization library does not support dynamic bundles yet.
                //But we know how System.Web.Optimization library stores cached results.
                //so let's clear the cache because we add new file references dynamically based on a page
                //until it's officially supported in System.Web.Optimization we have to "workaround" it manually
                //var cacheKey = (string)typeof(Bundle)
                //    .GetMethod("GetCacheKey", BindingFlags.Static | BindingFlags.NonPublic)
                //    .Invoke(null, new object[] { bundleVirtualPath });
                //or use the code below
                //TODO: ...but periodically ensure that cache key which we use is valid (decompile Bundle.GetCacheKey method)
                //var cacheKey = "System.Web.Optimization.Bundle:" + bundleVirtualPath;

                //if (_httpContext.Cache[cacheKey] != null)
                //    _httpContext.Cache.Remove(cacheKey);

                //create bundle
                lock (s_lock)
                {
                    var bundleFor = BundleTable.Bundles.GetBundleFor(bundleVirtualPath);
                    if (bundleFor == null)
                    {
                        var bundle = new ScriptBundle(bundleVirtualPath);
                        //bundle.Transforms.Clear();

                        //"As is" ordering
                        bundle.Orderer = new AsIsBundleOrderer();
                        //disable file extension replacements. renders scripts which were specified by a developer
                        bundle.EnableFileExtensionReplacements = false;
                        bundle.Include(distinctParts.ToArray());
                        BundleTable.Bundles.Add(bundle);
                        //we clear ignore list because System.Web.Optimization library adds ignore patterns such as "*.min", "*.debug".
                        //we think it's bad decision and should be disabled by default
                        BundleTable.Bundles.IgnoreList.Clear();
                    }
                }

                var result = Scripts.Render(bundleVirtualPath).ToString();
                return result;
            }
            else
            {
                //bundling is disabled
                var result = new StringBuilder();
                foreach (var path in distinctParts)
                {
                    result.AppendFormat("<script src=\"{0}\" type=\"text/javascript\"></script>", urlHelper.Content(path));
                    result.Append(Environment.NewLine);
                }
                return result.ToString();
            }
        }


        public void AddCssFileParts(ResourceLocation location, params string[] parts)
        {
            if (!_cssParts.ContainsKey(location))
                _cssParts.Add(location, new List<string>());

            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _cssParts[location].Add(part);
        }
        public void AppendCssFileParts(ResourceLocation location, params string[] parts)
        {
            if (!_cssParts.ContainsKey(location))
                _cssParts.Add(location, new List<string>());

            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _cssParts[location].Insert(0, part);
        }
        public string GenerateCssFiles(UrlHelper urlHelper, ResourceLocation location)
        {
            if (!_cssParts.ContainsKey(location) || _cssParts[location] == null)
                return "";

            //use only distinct rows
            var distinctParts = _cssParts[location].Distinct().ToList();
            if (distinctParts.Count == 0)
                return "";

            var result = new StringBuilder();
            foreach (var path in distinctParts)
            {
                result.AppendFormat("<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />", urlHelper.Content(path));
                result.Append(Environment.NewLine);
            }
            return result.ToString();
        }


        public void AddCanonicalUrlParts(params string[] parts)
        {
            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _canonicalUrlParts.Add(part);
        }
        public void AppendCanonicalUrlParts(params string[] parts)
        {
            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _canonicalUrlParts.Insert(0, part);
        }
        public string GenerateCanonicalUrls()
        {
            var result = new StringBuilder();
            foreach (var canonicalUrl in _canonicalUrlParts)
            {
                result.AppendFormat("<link rel=\"canonical\" href=\"{0}\" />", canonicalUrl);
                result.Append(Environment.NewLine);
            }
            return result.ToString();
        }

        #endregion
    }
}
