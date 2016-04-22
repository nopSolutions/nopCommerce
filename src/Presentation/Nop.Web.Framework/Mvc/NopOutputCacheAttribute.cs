using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Services.Logging;

namespace Nop.Web.Framework.Mvc
{
    public class NopOutputCacheAttribute : FilterAttribute, IActionFilter, IResultFilter, IDisposable
    {
        private const string RefreshKey = "__r";
        private static readonly long Epoch = new DateTime(2014, DateTimeKind.Utc).Ticks;

        // Dependencies.
        private readonly CatalogSettings _catalogSettings;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger _logger;
        
        private bool _isDisposed;

        public NopOutputCacheAttribute()
        {
            _cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static");
            _logger = EngineContext.Current.Resolve<ILogger>();
            _catalogSettings = EngineContext.Current.Resolve<CatalogSettings>();

            CacheKeyPrefix = "nop.output.shared";
            DurationInMinutes = CacheProfiles.DefaultProfile;
        }

        public NopOutputCacheAttribute(
            ICacheManager cacheManager,
            ILogger logger,
            CatalogSettings catalogSettings)
        {
            _cacheManager = cacheManager;
            _logger = logger;
            _catalogSettings = catalogSettings;
            CacheKeyPrefix = "nop.output.shared";
            DurationInMinutes = CacheProfiles.DefaultProfile;
        }

        // State.
        private DateTime _now;
        private string _cacheKey;
        private string _invariantCacheKey;
        private bool _transformRedirect;
        private bool _isCachingRequest;

        public bool VaryByCustomer { get; set; }
        public string CacheKeyPrefix { get; set; }
        public bool VaryByCulture { get; set; }
        public string VaryByRequestHeaders { get; set; }
        public string VaryByQueryStringParameters { get; set; }
        public int DurationInMinutes { get; set; }
        public int GraceDurationInMinutes { get; set; }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _logger.Debug(string.Format("Incoming request for URL '{0}'.",
                filterContext.RequestContext.HttpContext.Request.RawUrl));

            //check if caching enabled as setting
            if (!_catalogSettings.EnableOutputCache)
            {
                _logger.Debug(string.Format("Action '{0}' ignored because output caching is not enabled.",
                    filterContext.ActionDescriptor.ActionName));
                return;
            }

            // This filter is not reentrant (multiple executions within the same request are
            // not supported) so child actions are ignored completely.
            if (filterContext.IsChildAction)
            {
                _logger.Debug(string.Format("Action '{0}' ignored because it's a child action.",
                    filterContext.ActionDescriptor.ActionName));
                return;
            }

            _now = DateTime.UtcNow;

         
            if (!RequestIsCacheable(filterContext))
                return;

            // Computing the cache key after we know that the request is cacheable means that we are only performing this calculation on requests that require it
            _cacheKey = String.Intern(ComputeCacheKey(filterContext, GetCacheKeyParameters(filterContext)));
            _invariantCacheKey = ComputeCacheKey(filterContext, null);

            _logger.Debug(string.Format("Cache key '{0}' was created.", _cacheKey));

            try
            {
                // Is there a cached item, and are we allowed to serve it?
                // todo: enable this configurable.
                //var allowServeFromCache = filterContext.RequestContext.HttpContext.Request.Headers["Cache-Control"] !=
                //                          "no-cache";
                

                var cacheItem = GetCacheItem(_cacheKey);
                if (/*allowServeFromCache &&*/ cacheItem != null)
                {
                    _logger.Debug(string.Format("Item '{0}' was found in cache.", _cacheKey));

                    // Is the cached item in its grace period?
                    if (cacheItem.IsInGracePeriod(_now, GraceDurationInMinutes))
                    {
                        // Render the content unless another request is already doing so.
                        if (Monitor.TryEnter(_cacheKey))
                        {
                            _logger.Debug(string.Format("Item '{0}' is in grace period and not currently being rendered; rendering item...", _cacheKey));
                            BeginRenderItem(filterContext, cacheItem);
                            return;
                        }
                    }

                    // Cached item is not yet in its grace period, or is already being
                    // rendered by another request; serve it from cache.
                    _logger.Debug(string.Format("Serving item '{0}' from cache.", _cacheKey));
                    ServeCachedItem(filterContext, cacheItem);
                    return;
                }

                // No cached item found, or client doesn't want it; acquire the cache key
                // lock to render the item.
                _logger.Debug(string.Format("Item '{0}' was not found in cache or client refuses it. Acquiring cache key lock...", _cacheKey));
                if (Monitor.TryEnter(_cacheKey))
                {
                    _logger.Debug(string.Format("Cache key lock for item '{0}' was acquired.", _cacheKey));

                    // Item might now have been rendered and cached by another request; if so serve it from cache.
                    //if (allowServeFromCache)
                    //{
                        cacheItem = GetCacheItem(_cacheKey);
                        if (cacheItem != null)
                        {
                            _logger.Debug(string.Format("Item '{0}' was now found; releasing cache key lock and serving from cache.", _cacheKey));
                            Monitor.Exit(_cacheKey);
                            ServeCachedItem(filterContext, cacheItem);
                            return;
                        }
                    //}
                }

                // Either we acquired the cache key lock and the item was still not in cache, or
                // the lock acquisition timed out. In either case render the item.
                _logger.Debug(string.Format("Rendering item '{0}'...", _cacheKey));
                BeginRenderItem(filterContext, cacheItem);

            }
            catch
            {
                // Remember to release the cache key lock in the event of an exception!
                _logger.Debug(
                    string.Format("Exception occurred for item '{0}'; releasing any acquired lock.", _cacheKey));
                ReleaseCacheKeyLock();
                throw;
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            _transformRedirect = TransformRedirect(filterContext);
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
            var captureHandlerIsAttached = false;

            try
            {
                // This filter is not reentrant (multiple executions within the same request are
                // not supported) so child actions are ignored completely.
                if (filterContext.IsChildAction || !_isCachingRequest)
                    return;

                _logger.Debug(string.Format("Item '{0}' was rendered.", _cacheKey));

                if (!ResponseIsCacheable(filterContext))
                {
                    filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    filterContext.HttpContext.Response.Cache.SetNoStore();
                    filterContext.HttpContext.Response.Cache.SetMaxAge(new TimeSpan(0));
                    return;
                }

                // Capture the response output using a custom filter stream.
                var response = filterContext.HttpContext.Response;
                var captureStream = new CaptureStream(response.Filter);
                response.Filter = captureStream;

                // Add ETag header for the newly created item
                var etag = Guid.NewGuid().ToString("n");
                if (HttpRuntime.UsingIntegratedPipeline)
                {
                    if (response.Headers.Get("ETag") == null)
                    {
                        response.Headers["ETag"] = etag;
                    }
                }

                captureStream.Captured += output =>
                {
                    try
                    {
                        // Since this is a callback any call to injected dependencies can result in an Autofac exception: "Instances 
                        // cannot be resolved and nested lifetimes cannot be created from this LifetimeScope as it has already been disposed."
                        // To prevent access to the original lifetime scope a new work context scope should be created here and dependencies
                        // should be resolved from it.

                        var cacheItem = new CacheItem()
                        {
                            CachedOnUtc = _now,
                            ExpiresOnUtc = _now.AddMinutes(DurationInMinutes),
                            Data = output, // we can zip output to reduce memory (or redis memort)
                            ContentType = response.ContentType,
                            Key = _cacheKey,
                            InvariantKey = _invariantCacheKey,
                            StatusCode = response.StatusCode,
                            ETag = etag
                        };

                        // Write the rendered item to the cache.
                        var cacheManager =
                            EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static");

                        cacheManager.Set(_cacheKey, cacheItem, DurationInMinutes);

                        _logger.Debug(
                            string.Format("Item '{0}' was written to cache.",
                                _cacheKey));

                    }
                    finally
                    {
                        // Always release the cache key lock when the request ends.
                        ReleaseCacheKeyLock();
                    }
                };

                captureHandlerIsAttached = true;
            }
            finally
            {
                // If the response filter stream capture handler was attached then we'll trust
                // it to release the cache key lock at some point in the future when the stream
                // is flushed; otherwise we'll make sure we'll release it here.
                if (!captureHandlerIsAttached)
                    ReleaseCacheKeyLock();
            }
        }

        protected virtual bool RequestIsCacheable(ActionExecutingContext filterContext)
        {
            var itemDescriptor = string.Empty;
            
            // Don't cache POST requests.
            if (filterContext.HttpContext.Request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                _logger.Debug(string.Format("Request for item '{0}' ignored because HTTP method is POST.",
                    itemDescriptor));
                return false;
            }

            // Ignore requests with the refresh key on the query string.
            foreach (var key in filterContext.RequestContext.HttpContext.Request.QueryString.AllKeys)
            {
                if (String.Equals(RefreshKey, key, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.Debug(
                        string.Format("Request for item '{0}' ignored because refresh key was found on query string.",
                            itemDescriptor));
                    return false;
                }
            }

            return true;
        }

        protected virtual bool ResponseIsCacheable(ResultExecutedContext filterContext)
        {

            if (filterContext.HttpContext.Request.Url == null)
            {
                return false;
            }

            // Don't cache non-200 responses or results of a redirect.
            var response = filterContext.HttpContext.Response;
            if (response.StatusCode != (int)HttpStatusCode.OK || _transformRedirect)
            {
                return false;
            }

            //// Don't cache if request created notifications.
            //var hasNotifications = !String.IsNullOrEmpty(Convert.ToString(filterContext.Controller.TempData["messages"]));
            //if (hasNotifications)
            //{
            //    _logger.Debug(
            //        string.Format(
            //            "Response for item '{0}' will not be cached because one or more notifications were created.",
            //            _cacheKey));
            //    return false;
            //}

            return true;
        }

        protected virtual IDictionary<string, object> GetCacheKeyParameters(ActionExecutingContext filterContext)
        {
            var result = new Dictionary<string, object>();

            // Vary by action parameters.
            foreach (var p in filterContext.ActionParameters)
                result.Add("PARAM:" + p.Key, JsonConvert.SerializeObject(p.Value, new JsonSerializerSettings()
                {
                    DefaultValueHandling = DefaultValueHandling.Ignore
                }));

            
            // Vary by configured query string parameters.
            var queryString = filterContext.RequestContext.HttpContext.Request.QueryString;
            foreach (var key in queryString.AllKeys)
            {
                if (key == null || (VaryByQueryStringParameters != null && !VaryByQueryStringParameters.Contains(key)))
                    continue;
                result[key] = queryString[key];
            }

            // Vary by configured request headers.
            var requestHeaders = filterContext.RequestContext.HttpContext.Request.Headers;
            if (VaryByRequestHeaders != null)
                foreach (var varyByRequestHeader in VaryByRequestHeaders.Split(','))
                {
                    if (requestHeaders.AllKeys.Contains(varyByRequestHeader))
                        result["HEADER:" + varyByRequestHeader] = requestHeaders[varyByRequestHeader];
                }


            // Vary by request culture if configured.
            if (VaryByCulture)
            {
                result["culture"] = CultureInfo.CurrentUICulture.ToString();// _workContext.WorkingLanguage.LanguageCulture.ToLowerInvariant();
            }

            if (VaryByCustomer)
            {
                var workContext = EngineContext.Current.Resolve<IWorkContext>();
                result["customer"] = workContext.CurrentCustomer.Id.ToString();
            }
            
            return result;
        }

        protected virtual bool TransformRedirect(ActionExecutedContext filterContext)
        {

            // Removes the target of the redirection from cache after a POST.

            if (filterContext.Result == null)
            {
                throw new ArgumentNullException();
            }

            var redirectResult = filterContext.Result as RedirectResult;

            // status code can't be tested at this point, so test the result type instead
            if (redirectResult == null || !filterContext.HttpContext.Request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            _logger.Debug("Redirect on POST detected; removing from cache and adding refresh key.");

            var redirectUrl = redirectResult.Url;
            
            //if (filterContext.HttpContext.Request.Url.IsLocalUrl(redirectUrl))
            //{
                // Remove all cached versions of the same item.
                //var helper = new UrlHelper(filterContext.HttpContext.Request.RequestContext);
                //var absolutePath = new Uri(helper.MakeAbsolute(redirectUrl)).AbsolutePath;
                var invariantCacheKey = ComputeCacheKey(redirectUrl, null);
                _cacheManager.RemoveByPattern(invariantCacheKey);
            //}

            // Adding a refresh key so that the redirection doesn't get restored
            // from a cached version on a proxy. This can happen when using public
            // caching, we want to force the client to get a fresh copy of the
            // redirectUrl content.
            var epIndex = redirectUrl.IndexOf('?');
            var qs = new NameValueCollection();
            if (epIndex > 0)
            {
                qs = HttpUtility.ParseQueryString(redirectUrl.Substring(epIndex));
            }

            // Substract Epoch to get a smaller number.
            var refresh = _now.Ticks - Epoch;
            qs.Remove(RefreshKey);

            qs.Add(RefreshKey, refresh.ToString("x"));
            var querystring = "?" + string.Join("&", Array.ConvertAll(qs.AllKeys, k => string.Format("{0}={1}", HttpUtility.UrlEncode(k), HttpUtility.UrlEncode(qs[k]))));

            if (epIndex > 0)
            {
                redirectUrl = redirectUrl.Substring(0, epIndex) + querystring;
            }
            else
            {
                redirectUrl = redirectUrl + querystring;
            }

            filterContext.Result = new RedirectResult(redirectUrl, redirectResult.Permanent);
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            return true;
        }
        
        private void ServeCachedItem(ActionExecutingContext filterContext, CacheItem cacheItem)
        {
            var response = filterContext.HttpContext.Response;
            var request = filterContext.HttpContext.Request;

            // Fix for missing charset in response headers
            response.Charset = response.Charset;

            //add cachetie headers
            //todo: enable only on debug mode
            response.AddHeader("X-Cached-On", cacheItem.CachedOnUtc.ToString("r"));
            response.AddHeader("X-Cached-Until", cacheItem.ExpiresOnUtc.ToString("r"));

            // Shorcut action execution.
            filterContext.Result = new FileContentResult(cacheItem.Data, cacheItem.ContentType);
            response.StatusCode = cacheItem.StatusCode;

            // Add ETag header
            if (HttpRuntime.UsingIntegratedPipeline && response.Headers.Get("ETag") == null)
            {
                response.Headers["ETag"] = cacheItem.ETag;
            }

            // Check ETag in request
            // https://www.w3.org/2005/MWI/BPWG/techs/CachingWithETag.html
            var etag = request.Headers["If-None-Match"];
            if (!String.IsNullOrEmpty(etag))
            {
                if (String.Equals(etag, cacheItem.ETag, StringComparison.Ordinal))
                {
                    // ETag matches the cached item, we return a 304
                    filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.NotModified);
                    return;
                }
            }

            ApplyCacheControl(response, cacheItem);
        }

        private void BeginRenderItem(ActionExecutingContext filterContext, CacheItem cacheItem)
        {
            var response = filterContext.HttpContext.Response;

            ApplyCacheControl(response, cacheItem);

            // Remember that we should intercept the rendered response output.
            _isCachingRequest = true;
        }

        private void ApplyCacheControl(HttpResponseBase response, CacheItem cacheItem)
        {
            if (cacheItem != null)
            {
                var maxAge = cacheItem.ExpiresOnUtc - _now;
                if (maxAge.TotalMilliseconds < 0)
                {
                    maxAge = TimeSpan.Zero;
                }

                if (maxAge.TotalMilliseconds > 0)
                {
                    response.Cache.SetCacheability(HttpCacheability.Public);
                    response.Cache.SetOmitVaryStar(true);
                    response.Cache.SetMaxAge(maxAge);
                    response.Cache.SetSlidingExpiration(true);
                }
            }
            
            if (!string.IsNullOrEmpty(VaryByQueryStringParameters))
                foreach (var queryStringParam in VaryByQueryStringParameters.Split(','))
                {
                    response.Cache.VaryByParams[queryStringParam] = true;
                }

            if (VaryByRequestHeaders != null)
                foreach (var varyRequestHeader in VaryByRequestHeaders.Split(','))
                {
                    response.Cache.VaryByHeaders[varyRequestHeader] = true;
                }
        }

        private void ReleaseCacheKeyLock()
        {
            if (_cacheKey != null && Monitor.IsEntered(_cacheKey))
            {
                _logger.Debug(string.Format("Releasing cache key lock for item '{0}'.", _cacheKey));
                Monitor.Exit(_cacheKey);
                _cacheKey = null;
            }
        }

        protected virtual bool IsIgnoredUrl(string url, IEnumerable<string> ignoredUrls)
        {
            if (ignoredUrls == null || !ignoredUrls.Any())
            {
                return false;
            }

            url = url.TrimStart(new[] { '~' });

            foreach (var ignoredUrl in ignoredUrls)
            {
                var relativePath = ignoredUrl.TrimStart(new[] { '~' }).Trim();
                if (String.IsNullOrWhiteSpace(relativePath))
                {
                    continue;
                }

                // Ignore comments
                if (relativePath.StartsWith("#"))
                {
                    continue;
                }

                if (String.Equals(relativePath, url, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        protected virtual string ComputeCacheKey(ControllerContext controllerContext, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            var url = controllerContext.HttpContext.Request.Url.GetLeftPart(UriPartial.Path);
            return ComputeCacheKey(url, parameters);
        }

        protected virtual string ComputeCacheKey(string url, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            var keyBuilder = new StringBuilder();

            keyBuilder.AppendFormat("{0}.{1}.", CacheKeyPrefix, url.ToLowerInvariant());
            
            if (parameters != null)
            {
                var ordered = parameters.OrderBy(m => m.Key);
                foreach (var pair in ordered)
                {
                    keyBuilder.AppendFormat("{0}={1}.", pair.Key.ToLowerInvariant(), Convert.ToString(pair.Value).ToLowerInvariant());
                }
            }

            return keyBuilder.ToString();
        }

        protected virtual CacheItem GetCacheItem(string key)
        {
            try
            {
                var cacheItem = _cacheManager.Get<CacheItem>(key);
                return cacheItem;
            }
            catch (Exception e)
            {
                _logger.Error("An unexpected error occured while reading a cache entry", e);
            }

            return null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    // Free other state (managed objects).
                }

                if (_cacheKey != null && Monitor.IsEntered(_cacheKey))
                {
                    Monitor.Exit(_cacheKey);
                }

                _isDisposed = true;
            }
        }

        ~NopOutputCacheAttribute()
        {
            // Ensure locks are released even after an unexpected exception
            Dispose(false);
        }

        [Serializable]
        public class CacheItem
        {
            public string Version { get; set; }

            public string Key { get; set; }

            public string InvariantKey { get; set; }

            public byte[] Data { get; set; }

            public DateTime ExpiresOnUtc { get; set; }

            public DateTime CachedOnUtc { get; set; }

            public string ContentType { get; set; }

            public int StatusCode { get; set; }

            public string ETag { get; set; }

            public bool IsInGracePeriod(DateTime utcNow, int graceDurationInSeconds)
            {
                return utcNow > ExpiresOnUtc && utcNow < ExpiresOnUtc.AddSeconds(graceDurationInSeconds);
            }
        }

        public class CaptureStream : Stream
        {
            public CaptureStream(Stream innerStream)
            {
                _innerStream = innerStream;
                _captureStream = new MemoryStream();
            }

            private readonly Stream _innerStream;
            private readonly MemoryStream _captureStream;

            public override bool CanRead
            {
                get { return _innerStream.CanRead; }
            }

            public override bool CanSeek
            {
                get { return _innerStream.CanSeek; }
            }

            public override bool CanWrite
            {
                get { return _innerStream.CanWrite; }
            }

            public override long Length
            {
                get { return _innerStream.Length; }
            }

            public override long Position
            {
                get { return _innerStream.Position; }
                set { _innerStream.Position = value; }
            }

            public override long Seek(long offset, SeekOrigin direction)
            {
                return _innerStream.Seek(offset, direction);
            }

            public override void SetLength(long length)
            {
                _innerStream.SetLength(length);
            }

            public override void Close()
            {
                _innerStream.Close();
            }

            public override void Flush()
            {
                if (_captureStream.Length > 0)
                {
                    OnCaptured();
                    _captureStream.SetLength(0);
                }

                _innerStream.Flush();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return _innerStream.Read(buffer, offset, count);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                _captureStream.Write(buffer, offset, count);
                _innerStream.Write(buffer, offset, count);
            }

            public event Action<byte[]> Captured;

            protected virtual void OnCaptured()
            {
                Captured(_captureStream.ToArray());
            }
        }
    }
}