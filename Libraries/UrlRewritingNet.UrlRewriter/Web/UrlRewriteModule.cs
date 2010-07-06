/* UrlRewritingNet.UrlRewrite
 * Version 2.0
 * 
 * This Library is Copyright 2006 by Albert Weinert and Thomas Bandt.
 * 
 * http://der-albert.com, http://blog.thomasbandt.de
 * 
 * This Library is provided as is. No warrenty is expressed or implied.
 * 
 * You can use these Library in free and commercial projects without a fee.
 * 
 * No charge should be made for providing these Library to a third party.
 * 
 * It is allowed to modify the source to fit your special needs. If you 
 * made improvements you should make it public available by sending us 
 * your modifications or publish it on your site. If you publish it on 
 * your own site you have to notify us. This is not a commitment that we 
 * include your modifications. 
 * 
 * This Copyright notice must be included in the modified source code.
 * 
 * You are not allowed to build a commercial rewrite engine based on 
 * this code.
 * 
 * Based on http://weblogs.asp.net/fmarguerie/archive/2004/11/18/265719.aspx
 * 
 * For further informations see: http://www.urlrewriting.net/
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using UrlRewritingNet.Configuration;

namespace UrlRewritingNet.Web
{

    public class UrlRewriteModule : IHttpModule
    {

        const string ItemsVirtualUrl = "UrlRewritingNet.UrlRewriter.VirtualUrl";
        const string ItemsClientQueryString = "UrlRewritingNet.UrlRewriter.ClientQueryString";
        const string ItemsRewriteUrlParameter = "UrlRewritingNet.UrlRewriter.RewriteUrlParameter";
        const string ItemsCachedPathAfterRewrite = "UrlRewritingNet.UrlRewriter.CachedPathAfterRewrite";

        public const string PhysicalPath = "UrlRewritingNet.UrlRewriter.CachedPathAfterRewrite";

        private bool rewriteOnlyVirtualUrls = false;
        private string _contextItemsPrefix = string.Empty;

        private RewriteRuleCollection _rewrites;
        private ReaderWriterLock lockRewrites = new ReaderWriterLock();
        private ReaderWriterLock lockRedirects = new ReaderWriterLock();

        internal RewriteRuleCollection Rewrites
        {
            get
            {
                if (_rewrites == null)
                {
                    lock (this)
                    {
                        if (_rewrites == null)
                        {
                            _rewrites = new RewriteRuleCollection();
                        }
                    }
                }
                return _rewrites;
            }
        }

        private RewriteRuleCollection _redirects;

        internal RewriteRuleCollection Redirects
        {
            get
            {
                if (_redirects == null)
                {
                    lock (this)
                    {
                        if (_redirects == null)
                        {
                            _redirects = new RewriteRuleCollection();
                        }
                    }
                }
                return _redirects;
            }
        }



        private UrlHelper urlHelper;

        public UrlRewriteModule()
        {
            this.urlHelper = new UrlHelper();
        }

        private string JoinUrlParameter(string destinationUrl, string requestQuerystring)
        {
            int pos = destinationUrl.IndexOf('?');
            if (requestQuerystring.Length > 0)
            {
                if (pos >= 0)
                {
                    destinationUrl += "&" + requestQuerystring;
                }
                else
                {
                    destinationUrl += "?" + requestQuerystring;
                }
            }
            return destinationUrl;
        }
        /// <summary>
        /// Redirects the URL.
        /// </summary>
        /// <param name="app">The app.</param>
        /// <returns>true if redirected, false if not</returns>
        private bool RedirectUrl(HttpApplication app)
        {
            string requestUrl;
            bool redirected = false;
            // First, checking for Redirects
            lockRedirects.AcquireReaderLock(0);
            foreach (RewriteRule rewrite in Redirects)
            {
                if (rewrite.Redirect == RedirectOption.Domain)
                    requestUrl = app.Request.Url.AbsoluteUri;
                else
                    requestUrl = app.Request.RawUrl;

                if (rewrite.IsRewrite(requestUrl))
                {
                    bool includeQueryStringForRewrite = (rewrite.RewriteUrlParameter & RewriteUrlParameterOption.IncludeQueryStringForRewrite) != 0;

                    string urlForRewrite = requestUrl;
                    string requestQuerystring = string.Empty;
                    int pos = requestUrl.IndexOf('?');
                    if (!includeQueryStringForRewrite && pos >= 0)
                    {
                        requestQuerystring = requestUrl.Substring(pos + 1);
                        urlForRewrite = requestUrl.Substring(0, pos);
                    }
                    string destinationUrl = rewrite.RewriteUrl(urlForRewrite);

                    if (includeQueryStringForRewrite)
                        destinationUrl = urlHelper.HandleRootOperator(destinationUrl);
                    else
                        destinationUrl = JoinUrlParameter(urlHelper.HandleRootOperator(destinationUrl), requestQuerystring);

                    StringBuilder sb = new StringBuilder();

                    if (rewrite.Redirect == RedirectOption.Domain)
                    {
                        sb.Append(destinationUrl);
                    }
                    else
                    {
                        sb.Append(app.Request.Url.Scheme);
                        sb.Append("://");
                        sb.Append(app.Request.Url.Authority);
                        sb.Append(destinationUrl);
                    }

                    // Nur für den Fall, dass die Weiterleitung explizit auf permanent
                    // gestellt ist, 301 festlegen - sonst kann das Konsequenzen haben
                    if (rewrite.RedirectMode == RedirectModeOption.Permanent)
                        app.Response.StatusCode = 301;
                    else
                        app.Response.StatusCode = 302;

                    // Location MUST be absolut.
                    app.Response.AddHeader("Location", sb.ToString());
                    app.Response.End();
                    redirected = true;
                    break;
                }
            }
            lockRedirects.ReleaseReaderLock();
            return redirected;
        }

        /// <summary>
        /// Rewrites the URL.
        /// </summary>
        /// <param name="app">The app.</param>
        private bool RewriteUrl(HttpApplication app)
        {
            string requestUrl;
            bool rewritten = false;
            // Do the Rewrites
            lockRewrites.AcquireReaderLock(0);
            foreach (RewriteRule rewrite in Rewrites)
            {
                if ((rewrite.Rewrite & RewriteOption.Domain) != 0)
                    requestUrl = app.Request.Url.AbsoluteUri;
                else
                    requestUrl = app.Request.RawUrl;

                if (rewrite.IsRewrite(requestUrl))
                {
                    bool includeQueryStringForRewrite = (rewrite.RewriteUrlParameter & RewriteUrlParameterOption.IncludeQueryStringForRewrite) != 0;

                    string urlForRewrite = requestUrl;
                    string requestQuerystring = string.Empty;
                    int pos = requestUrl.IndexOf('?');
                    if (!includeQueryStringForRewrite && pos >= 0)
                    {
                        requestQuerystring = requestUrl.Substring(pos + 1);
                        urlForRewrite = requestUrl.Substring(0, pos);
                    }

                    app.Context.Items[ItemsClientQueryString] = requestQuerystring;
                    app.Context.Items[ItemsRewriteUrlParameter] = rewrite.RewriteUrlParameter;

                    if ((rewrite.Rewrite & RewriteOption.Domain) != 0)
                    {
                        // Remove domain if exists
                        int slashPos = requestUrl.IndexOf("://");
                        if (slashPos > 0)
                        {
                            slashPos = requestUrl.IndexOf("/", slashPos + 3);
                            if (slashPos > 0)
                            {
                                app.Context.Items[ItemsVirtualUrl] = requestUrl.Substring(slashPos);
                            }
                        }
                    }
                    else
                    {
                        app.Context.Items[ItemsVirtualUrl] = requestUrl;

                    }

                    string destinationUrl = rewrite.RewriteUrl(urlForRewrite);


                    if (includeQueryStringForRewrite)
                        destinationUrl = urlHelper.HandleRootOperator(destinationUrl);
                    else
                        destinationUrl = JoinUrlParameter(urlHelper.HandleRootOperator(destinationUrl), requestQuerystring);
                    // Save original querystring, url and rewrite-parameters for OnPagePreInit()

                    app.Response.StatusCode = 200;
                    app.Context.RewritePath(destinationUrl, false);
                    rewritten = true;
                    break;
                }
            }
            lockRewrites.ReleaseReaderLock();
            return rewritten;
        }
        /// <summary>
        /// Called when [begin request].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void OnBeginRequest(object sender, EventArgs e)
        {
            HttpApplication app = sender as HttpApplication;
            if (!RedirectUrl(app))
            {
                if (rewriteOnlyVirtualUrls && File.Exists(app.Request.PhysicalPath)) return;

                if (!RewriteUrl(app))
                {
                    if (!File.Exists(app.Request.PhysicalPath) && !string.IsNullOrEmpty(UrlRewriting.Configuration.DefaultPage))
                    {
                        Uri url = app.Request.Url;
                        if (url.Segments.Length > 0)
                        {
                            // Don't redirect if it may be a file
                            if (url.Segments[url.Segments.Length - 1].IndexOf('.') >= 0) return;
                        }
                        StringBuilder sb = new StringBuilder();
                        sb.Append(url.Scheme);
                        sb.Append("://");
                        sb.Append(url.Authority);
                        string lastSeg = "";
                        foreach (string segment in url.Segments)
                        {
                            sb.Append(segment);
                            lastSeg = segment;
                        }
                        if (!lastSeg.EndsWith("/"))
                            sb.Append("/");
                        sb.Append(UrlRewriting.Configuration.DefaultPage);
                        if (!string.IsNullOrEmpty(url.Query))
                        {
                            sb.Append(url.Query);
                        }
                        app.Response.Redirect(sb.ToString());
                    }
                }
            }
        }


        #region IHttpModule members

        public void Init(System.Web.HttpApplication context)
        {

            rewriteOnlyVirtualUrls = UrlRewriting.Configuration.RewriteOnlyVirtualUrls;
            _contextItemsPrefix = UrlRewriting.Configuration.ContextItemsPrefix;

            // Copy Settings for easier and faster Accesss during requests
            foreach (RewriteSettings rewriteSettings in UrlRewriting.Configuration.Rewrites)
            {
                RewriteRule rewrite = null;
                string providerName = rewriteSettings.Provider;
                if (string.IsNullOrEmpty(providerName))
                {
                    rewrite = UrlRewriting.CreateRewriteRule();
                }
                else
                {
                    rewrite = UrlRewriting.CreateRewriteRule(providerName);
                }
                rewrite.Initialize(rewriteSettings);
                AddRewriteRuleInternal(rewrite);
            }
            context.BeginRequest += new EventHandler(OnBeginRequest);
            context.PreRequestHandlerExecute += new EventHandler(OnAppPreRequestHandlerExecute);
            context.PostRequestHandlerExecute += new EventHandler(OnAppPostRequestHandlerExecute);
        }

        internal void AddRewriteRuleInternal(RewriteRule rewriteRule)
        {

            if (rewriteRule.Redirect == RedirectOption.None)
            {
                lockRewrites.AcquireWriterLock(0);
                Rewrites.Add(rewriteRule);
                lockRewrites.ReleaseWriterLock();
            }
            else
            {
                lockRedirects.AcquireWriterLock(0);
                Redirects.Add(rewriteRule);
                lockRedirects.ReleaseWriterLock();
            }
        }
        internal void RemoveRewriteRuleInternal(string ruleName)
        {
            lockRewrites.AcquireWriterLock(0);
            Rewrites.RemoveByName(ruleName);
            lockRewrites.ReleaseWriterLock();

            lockRedirects.AcquireWriterLock(0);
            Redirects.RemoveByName(ruleName);
            lockRedirects.ReleaseWriterLock();
        }

        internal void ReplaceRewriteRuleInternal(string ruleName, RewriteRule rewriteRule)
        {
            if (rewriteRule.Redirect == RedirectOption.None)
            {
                lockRewrites.AcquireWriterLock(0);
                Rewrites.ReplaceRuleByName(ruleName, rewriteRule);
                lockRewrites.ReleaseWriterLock();
            }
            else
            {
                lockRedirects.AcquireWriterLock(0);
                Redirects.ReplaceRuleByName(ruleName, rewriteRule);
                lockRedirects.ReleaseWriterLock();
            }
        }

        internal void InsertRewriteRuleBeforeInternal(string positionRuleName, string ruleName, RewriteRule rewriteRule)
        {
            if (rewriteRule.Redirect == RedirectOption.None)
            {
                lockRewrites.AcquireWriterLock(0);
                Rewrites.InsertRuleBeforeName(positionRuleName, ruleName, rewriteRule);
                lockRewrites.ReleaseWriterLock();
            }
            else
            {
                lockRedirects.AcquireWriterLock(0);
                Redirects.InsertRuleBeforeName(positionRuleName, ruleName, rewriteRule);
                lockRedirects.ReleaseWriterLock();
            }
        }

        public void Dispose()
        {
        }

        #endregion

        private void OnAppPostRequestHandlerExecute(object sender, EventArgs e)
        {
            HttpApplication app = sender as HttpApplication;
            string cachedPath = (string)app.Context.Items[ItemsCachedPathAfterRewrite];
            if (!string.IsNullOrEmpty(cachedPath))
            {
                app.Context.RewritePath(cachedPath);
            }
        }

        private void OnAppPreRequestHandlerExecute(object sender, EventArgs e)
        {
            HttpApplication app = sender as HttpApplication;
            System.Web.UI.Page page = app.Context.CurrentHandler as System.Web.UI.Page;
            if (page != null)
            {
                page.PreInit += new EventHandler(OnPagePreInit);
            }
        }

        private void OnPagePreInit(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;
            string virtualUrl = (string)context.Items[ItemsVirtualUrl];
            if (!string.IsNullOrEmpty(virtualUrl))
            {
                int pos = virtualUrl.IndexOf('?');
                if (pos >= 0)
                {
                    virtualUrl = virtualUrl.Substring(0, pos);
                }

                RewriteUrlParameterOption option = (RewriteUrlParameterOption)context.Items[ItemsRewriteUrlParameter];
                string fullQueryString = context.Request.QueryString.ToString();
                context.Items[ItemsCachedPathAfterRewrite] = context.Request.Path + "?" + fullQueryString;

                if ((option & RewriteUrlParameterOption.StoreInContextItems) != 0)
                {
                    foreach (string key in context.Request.QueryString.AllKeys)
                    {
                        context.Items[_contextItemsPrefix + key] = context.Request.QueryString[key];
                    }
                }

                string clientQueryString = (string)context.Items[ItemsClientQueryString];
                if ((option & RewriteUrlParameterOption.ExcludeFromClientQueryString) != 0)
                {
                    context.RewritePath(virtualUrl, string.Empty, clientQueryString, true);
                    Page page = sender as Page;
                    clientQueryString = page.ClientQueryString;
                }

                if ((option & RewriteUrlParameterOption.StoreInContextItems) == 0)
                {
                    context.RewritePath(virtualUrl, string.Empty, fullQueryString, true);
                }
                else if ((option & RewriteUrlParameterOption.ExcludeFromClientQueryString) == 0)
                {
                    context.RewritePath(virtualUrl, string.Empty, clientQueryString, true);
                }
            }
        }

    }
}
