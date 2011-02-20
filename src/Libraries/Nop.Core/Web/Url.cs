using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using Nop.Core.Infrastructure;

namespace Nop.Core.Web
{
    /// <summary>
    /// A lightweight and somewhat forgiving URI helper class.
    /// </summary>
    public class Url
    {
        /// <summary>Ampersand string.</summary>
        public const string Amp = "&";

        /// <summary>The token used for resolving the management url.</summary>
        public const string ManagementUrlToken = "{ManagementUrl}";

        static readonly string[] querySplitter = new[] { "&amp;", Amp };
        static readonly char[] slashes = new char[] { '/' };
        static readonly char[] dotsAndSlashes = new char[] { '.', '/' };
        static string defaultExtension = ".aspx";
        static Dictionary<string, string> replacements = new Dictionary<string, string>();

        string scheme;
        string authority;
        string path;
        string query;
        string fragment;

        public Url(Url other)
        {
            scheme = other.scheme;
            authority = other.authority;
            path = other.path;
            query = other.query;
            fragment = other.fragment;
        }

        public Url(string scheme, string authority, string path, string query, string fragment)
        {
            this.scheme = scheme;
            this.authority = authority;
            this.path = path;
            this.query = query;
            this.fragment = fragment;
        }

        public Url(string scheme, string authority, string rawUrl)
        {
            int queryIndex = QueryIndex(rawUrl);
            int hashIndex = rawUrl.IndexOf('#', queryIndex > 0 ? queryIndex : 0);
            LoadFragment(rawUrl, hashIndex);
            LoadQuery(rawUrl, queryIndex, hashIndex);
            LoadSiteRelativeUrl(rawUrl, queryIndex, hashIndex);
            this.scheme = scheme;
            this.authority = authority;
        }

        public Url(string url)
        {
            if (url == null)
            {
                ClearUrl();
            }
            else
            {
                int queryIndex = QueryIndex(url);
                int hashIndex = url.IndexOf('#', queryIndex > 0 ? queryIndex : 0);
                int authorityIndex = url.IndexOf("://");
                if (queryIndex >= 0 && authorityIndex > queryIndex)
                    authorityIndex = -1;

                LoadFragment(url, hashIndex);
                LoadQuery(url, queryIndex, hashIndex);
                if (authorityIndex >= 0)
                    LoadBasedUrl(url, queryIndex, hashIndex, authorityIndex);
                else
                    LoadSiteRelativeUrl(url, queryIndex, hashIndex);
            }
        }

        void ClearUrl()
        {
            scheme = null;
            authority = null;
            path = "";
            query = null;
            fragment = null;
        }

        void LoadSiteRelativeUrl(string url, int queryIndex, int hashIndex)
        {
            scheme = null;
            authority = null;
            if (queryIndex >= 0)
                path = url.Substring(0, queryIndex);
            else if (hashIndex >= 0)
                path = url.Substring(0, hashIndex);
            else if (url.Length > 0)
                path = url;
            else
                path = "";
        }

        void LoadBasedUrl(string url, int queryIndex, int hashIndex, int authorityIndex)
        {
            scheme = url.Substring(0, authorityIndex);
            int slashIndex = url.IndexOf('/', authorityIndex + 3);
            if (slashIndex > 0)
            {
                authority = url.Substring(authorityIndex + 3, slashIndex - authorityIndex - 3);
                if (queryIndex >= slashIndex)
                    path = url.Substring(slashIndex, queryIndex - slashIndex);
                else if (hashIndex >= 0)
                    path = url.Substring(slashIndex, hashIndex - slashIndex);
                else
                    path = url.Substring(slashIndex);
            }
            else
            {
                // is this case tolerated?
                authority = url.Substring(authorityIndex + 3);
                path = "/";
            }
        }

        void LoadQuery(string url, int queryIndex, int hashIndex)
        {
            if (hashIndex >= 0 && queryIndex >= 0)
                query = EmptyToNull(url.Substring(queryIndex + 1, hashIndex - queryIndex - 1));
            else if (queryIndex >= 0)
                query = EmptyToNull(url.Substring(queryIndex + 1));
            else
                query = null;
        }

        void LoadFragment(string url, int hashIndex)
        {
            if (hashIndex >= 0)
                fragment = EmptyToNull(url.Substring(hashIndex + 1));
            else
                fragment = null;
        }

        private string EmptyToNull(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            return text;
        }

        public Url HostUrl
        {
            get { return new Url(scheme, authority, string.Empty, null, null); }
        }

        public Url LocalUrl
        {
            get { return new Url(null, null, path, query, fragment); }
        }

        /// <summary>E.g. http</summary>
        public string Scheme
        {
            get { return scheme; }
        }

        /// <summary>The domain name and port information.</summary>
        public string Authority
        {
            get { return authority; }
        }

        /// <summary>The path after domain name and before query string, e.g. /path/to/a/page.aspx.</summary>
        public string Path
        {
            get { return path; }
        }

        public string ApplicationRelativePath
        {
            get
            {
                string appPath = ApplicationPath;
                if (appPath.Equals("/"))
                    return "~" + Path;
                if (Path.StartsWith(appPath, StringComparison.InvariantCultureIgnoreCase))
                    return Path.Substring(appPath.Length);
                return Path;
            }
        }

        /// <summary>The query string, e.g. key=value.</summary>
        public string Query
        {
            get { return query; }
        }

        public string Extension
        {
            get
            {
                int index = path.LastIndexOfAny(dotsAndSlashes);

                if (index < 0)
                    return null;
                if (path[index] == '/')
                    return null;

                return path.Substring(index);
            }
        }

        public string PathWithoutExtension
        {
            get { return RemoveExtension(path); }
        }

        /// <summary>The combination of the path and the query string, e.g. /path.aspx?key=value.</summary>
        public string PathAndQuery
        {
            get { return string.IsNullOrEmpty(Query) ? Path : Path + "?" + Query; }
        }

        /// <summary>The bookmark.</summary>
        public string Fragment
        {
            get { return fragment; }
        }

        /// <summary>Tells whether the url contains authority information.</summary>
        public bool IsAbsolute
        {
            get { return authority != null; }
        }

        public override string ToString()
        {
            string url;
            if (authority != null)
                url = scheme + "://" + authority + path;
            else
                url = path;
            if (query != null)
                url += "?" + query;
            if (fragment != null)
                url += "#" + fragment;
            return url;
        }

        public static implicit operator string(Url u)
        {
            if (u == null)
                return null;
            return u.ToString();
        }

        public static implicit operator Url(string url)
        {
            return Parse(url);
        }

        /// <summary>Retrieves the path part of an url, e.g. /path/to/page.aspx.</summary>
        public static string PathPart(string url)
        {
            url = RemoveHash(url);

            int queryIndex = QueryIndex(url);
            if (queryIndex >= 0)
                url = url.Substring(0, queryIndex);

            return url;
        }

        /// <summary><![CDATA[Retrieves the query part of an url, e.g. page=12&value=something.]]></summary>
        public static string QueryPart(string url)
        {
            url = RemoveHash(url);

            int queryIndex = QueryIndex(url);
            if (queryIndex >= 0)
                return url.Substring(queryIndex + 1);
            return string.Empty;
        }

        static int QueryIndex(string url)
        {
            return url.IndexOf('?');
        }

        /// <summary>The extension used for url's to content items.</summary>
        public static string DefaultExtension
        {
            get { return defaultExtension; }
            set { defaultExtension = value; }
        }

        /// <summary>Removes the hash (#...) from an url.</summary>
        /// <param name="url">An url that might hav a hash in it.</param>
        /// <returns>An url without the hash part.</returns>
        public static string RemoveHash(string url)
        {
            int hashIndex = url.IndexOf('#');
            if (hashIndex >= 0)
                url = url.Substring(0, hashIndex);
            return url;
        }

        /// <summary>Converts a string URI to an Url class. The method will make any app-relative managementUrls absolute.</summary>
        /// <param name="url">The URI to parse.</param>
        /// <returns>An Url object or null if the input was null.</returns>
        public static Url Parse(string url)
        {
            if (url == null)
                return null;
            if (url.StartsWith("~"))
                url = ToAbsolute(url);

            return new Url(url);
        }

        public string GetQuery(string key)
        {
            IDictionary<string, string> queries = GetQueries();
            if (queries.ContainsKey(key))
                return queries[key];

            return null;
        }

        public string this[string queryKey]
        {
            get { return GetQuery(queryKey); }
        }

        public IDictionary<string, string> GetQueries()
        {
            return ParseQueryString(query);
        }

        public Url AppendQuery(string key, string value)
        {
            return AppendQuery(key + "=" + HttpUtility.UrlEncode(value));
        }

        public Url AppendQuery(string key, int value)
        {
            return AppendQuery(key + "=" + value);
        }

        public Url AppendQuery(string key, bool value)
        {
            return AppendQuery(key + (value ? "=true" : "=false"));
        }

        public Url AppendQuery(string key, object value)
        {
            if (value == null)
                return this;

            return AppendQuery(key + "=" + value);
        }

        public Url AppendQuery(string keyValue)
        {
            var clone = new Url(this);
            if (string.IsNullOrEmpty(query))
                clone.query = keyValue;
            else if (!string.IsNullOrEmpty(keyValue))
                clone.query += Amp + keyValue;
            return clone;
        }

        public Url SetQueryParameter(string key, int value)
        {
            return SetQueryParameter(key, value.ToString());
        }

        public Url SetQueryParameter(string key, string value)
        {
            return SetQueryParameter(key, value, false);
        }

        public Url SetQueryParameter(string key, string value, bool removeNullValue)
        {
            if (removeNullValue && value == null && query == null)
                return this;
            if (query == null)
                return AppendQuery(key, value);

            var clone = new Url(this);
            string[] queries = query.Split(querySplitter, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < queries.Length; i++)
            {
                if (queries[i].StartsWith(key + "=", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (value != null)
                    {
                        queries[i] = key + "=" + HttpUtility.UrlEncode(value);
                        clone.query = string.Join(Amp, queries);
                        return clone;
                    }

                    if (queries.Length == 1)
                        clone.query = null;
                    else if (query.Length == 2)
                        clone.query = queries[i == 0 ? 1 : 0];
                    else if (i == 0)
                        clone.query = string.Join(Amp, queries, 1, queries.Length - 1);
                    else if (i == queries.Length - 1)
                        clone.query = string.Join(Amp, queries, 0, queries.Length - 1);
                    else
                        clone.query = string.Join(Amp, queries, 0, i) + Amp + string.Join(Amp, queries, i + 1, queries.Length - i - 1);
                    return clone;
                }
            }
            return AppendQuery(key, value);
        }

        public Url SetQueryParameter(string keyValue)
        {
            if (query == null)
                return AppendQuery(keyValue);

            int eqIndex = keyValue.IndexOf('=');
            if (eqIndex >= 0)
                return SetQueryParameter(keyValue.Substring(0, eqIndex), keyValue.Substring(eqIndex + 1));
            else
                return SetQueryParameter(keyValue, string.Empty);
        }

        public Url SetScheme(string scheme)
        {
            return new Url(scheme, authority, path, query, fragment);
        }

        public Url SetAuthority(string authority)
        {
            return new Url(scheme ?? "http", authority, path, query, fragment);
        }

        public Url SetPath(string path)
        {
            if (path.StartsWith("~"))
                path = ToAbsolute(path);
            int queryIndex = QueryIndex(path);
            return new Url(scheme, authority, queryIndex < 0 ? path : path.Substring(0, queryIndex), query, fragment);
        }

        public Url SetQuery(string query)
        {
            return new Url(scheme, authority, path, query, fragment);
        }

        public Url SetExtension(string extension)
        {
            return new Url(scheme, authority, PathWithoutExtension + extension, query, fragment);
        }



        /// <summary>Returns an url with the specified fragment</summary>
        /// <param name="fragment">The fragment to use in the Url.</param>
        /// <returns>An url with the given fragment.</returns>
        public Url SetFragment(string fragment)
        {
            if (fragment == null)
                return this;

            return new Url(scheme, authority, path, query, fragment.TrimStart('#'));
        }

        public Url AppendSegment(string segment, string extension)
        {
            string newPath;
            if (string.IsNullOrEmpty(path) || path == "/")
                newPath = "/" + segment + extension;
            else if (!string.IsNullOrEmpty(extension))
            {
                int extensionIndex = path.LastIndexOf(extension);
                if (extensionIndex >= 0)
                    newPath = path.Insert(extensionIndex, "/" + segment);
                else if (path.EndsWith("/"))
                    newPath = path + segment + extension;
                else
                    newPath = path + "/" + segment + extension;
            }
            else if (path.EndsWith("/"))
                newPath = path + segment;
            else
                newPath = path + "/" + segment;

            return new Url(scheme, authority, newPath, query, fragment);
        }

        public Url AppendSegment(string segment)
        {
            if (string.IsNullOrEmpty(Path) || Path == "/")
                return AppendSegment(segment, DefaultExtension);

            return AppendSegment(segment, Extension);
        }

        public Url AppendSegment(string segment, bool useDefaultExtension)
        {
            return AppendSegment(segment, useDefaultExtension ? DefaultExtension : Extension);
        }

        public Url PrependSegment(string segment, string extension)
        {
            string newPath;
            if (string.IsNullOrEmpty(path) || path == "/")
                newPath = "/" + segment + extension;
            else if (extension != Extension)
            {
                newPath = "/" + segment + PathWithoutExtension + extension;
            }
            else
            {
                newPath = "/" + segment + path;
            }

            return new Url(scheme, authority, newPath, query, fragment);
        }

        public Url PrependSegment(string segment)
        {
            if (string.IsNullOrEmpty(Path) || Path == "/")
                return PrependSegment(segment, DefaultExtension);

            return PrependSegment(segment, Extension);
        }

        public Url UpdateQuery(NameValueCollection queryString)
        {
            Url u = new Url(this);
            foreach (string key in queryString.AllKeys)
                u = u.SetQueryParameter(key, queryString[key]);
            return u;
        }

        public Url UpdateQuery(IDictionary<string, string> queryString)
        {
            Url u = new Url(this);
            foreach (KeyValuePair<string, string> pair in queryString)
                u = u.SetQueryParameter(pair.Key, pair.Value);
            return u;
        }

        public Url UpdateQuery(IDictionary<string, object> queryString)
        {
            Url u = new Url(this);
            foreach (KeyValuePair<string, object> pair in queryString)
                if (pair.Value != null)
                    u = u.SetQueryParameter(pair.Key, pair.Value.ToString());
            return u;
        }

        /// <summary>Returns the url without the file extension (if any).</summary>
        /// <returns>An url with it's extension removed.</returns>
        public Url RemoveExtension()
        {
            return new Url(scheme, authority, PathWithoutExtension, query, fragment);
        }

        /// <summary>Converts a possibly relative to an absolute url.</summary>
        /// <param name="path">The url to convert.</param>
        /// <returns>The absolute url.</returns>
        public static string ToAbsolute(string path)
        {
            return ToAbsolute(ApplicationPath, path);
        }

        /// <summary>Converts a possibly relative to an absolute url.</summary>
        /// <param name="applicationPath">The application path to be absolute about.</param>
        /// <param name="path">The url to convert.</param>
        /// <returns>The absolute url.</returns>
        public static string ToAbsolute(string applicationPath, string path)
        {
            if (!string.IsNullOrEmpty(path) && path[0] == '~' && path.Length > 1)
                return applicationPath + path.Substring(2);
            else if (path == "~")
                return applicationPath;
            return path;
        }

        /// <summary>Converts a virtual path to a relative path, e.g. /myapp/path/to/a/page.aspx -> ~/path/to/a/page.aspx</summary>
        /// <param name="path">The virtual path.</param>
        /// <returns>A relative path</returns>
        public static string ToRelative(string path)
        {
            return ToRelative(ApplicationPath, path);
        }

        /// <summary>Converts a virtual path to a relative path, e.g. /myapp/path/to/a/page.aspx -> ~/path/to/a/page.aspx</summary>
        /// <param name="path">The virtual path.</param>
        /// <returns>A relative path</returns>
        public static string ToRelative(string applicationPath, string path)
        {
            if (!string.IsNullOrEmpty(path) && path.StartsWith(applicationPath, StringComparison.OrdinalIgnoreCase))
                return "~/" + path.Substring(applicationPath.Length);
            return path;
        }

        static string applicationPath;
        /// <summary>Gets the root path of the web application. e.g. "/" if the application doesn't run in a virtual directory.</summary>
        public static string ApplicationPath
        {
            get
            {
                if (applicationPath == null)
                {
                    try
                    {
                        applicationPath = VirtualPathUtility.ToAbsolute("~/");
                    }
                    catch
                    {
                        return "/";
                    }
                }
                return applicationPath;
            }
            set { applicationPath = value; }
        }

        private static string fallbackServerUrl;
        /// <summary>The address to the server where the site is running.</summary>
        public static string ServerUrl
        {
            get
            {
                // we need get the server url of current request, it can't be cached in multiple-sites environment 
                var webContext = EngineContext.Current.RequestContext;
                if (webContext == null)
                    return null;
                if (webContext.IsWeb)
                {
                    if (fallbackServerUrl == null)
                        fallbackServerUrl = webContext.Url.HostUrl;
                    return webContext.Url.HostUrl;
                }
                return fallbackServerUrl; // fallback for ThreadContext
            }
        }

        /// <summary>Removes the file extension from a path.</summary>
        /// <param name="path">The server relative path.</param>
        /// <returns>The path without the file extension or the same path if no extension was found.</returns>
        public static string RemoveExtension(string path)
        {
            int index = path.LastIndexOfAny(dotsAndSlashes);

            if (index < 0)
                return path;
            if (path[index] == '/')
                return path;

            return path.Substring(0, index);
        }

        /// <summary>Removes the last part from the url segments.</summary>
        /// <returns></returns>
        public Url RemoveTrailingSegment(bool maintainExtension)
        {
            if (string.IsNullOrEmpty(path) || path == "/")
                return this;

            string newPath = "/";

            int lastSlashIndex = path.LastIndexOf('/');
            if (lastSlashIndex == path.Length - 1)
                lastSlashIndex = path.TrimEnd(slashes).LastIndexOf('/');
            if (lastSlashIndex > 0)
                newPath = path.Substring(0, lastSlashIndex) + (maintainExtension ? Extension : "");

            return new Url(scheme, authority, newPath, query, fragment);
        }

        /// <summary>Removes the last part from the url segments.</summary>
        public Url RemoveTrailingSegment()
        {
            return RemoveTrailingSegment(true);
        }

        /// <summary>Removes the segment at the specified location.</summary>
        /// <param name="index">The segment index to remove</param>
        /// <returns>An url without the specified segment.</returns>
        public Url RemoveSegment(int index)
        {
            if (string.IsNullOrEmpty(path) || path == "/" || index < 0)
                return this;

            if (index == 0)
            {
                int slashIndex = path.IndexOf('/', 1);
                if (slashIndex < 0)
                    return new Url(scheme, authority, "/", query, fragment);
                return new Url(scheme, authority, path.Substring(slashIndex), query, fragment);
            }

            string[] segments = PathWithoutExtension.Split(slashes, StringSplitOptions.RemoveEmptyEntries);
            if (index >= segments.Length)
                return this;

            if (index == segments.Length - 1)
                return RemoveTrailingSegment();

            string newPath = "/" + string.Join("/", segments, 0, index) + "/" + string.Join("/", segments, index + 1, segments.Length - index - 1) + Extension;
            return new Url(scheme, authority, newPath, query, fragment);
        }

        /// <summary>Removes the last segment from a path, e.g. "/hello/world" -> "/hello/"</summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string RemoveLastSegment(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            int slashIndex = GetLastSignificatSlash(path);
            return path.Substring(0, slashIndex + 1);
        }

        /// <summary>Gets the last segment of a path, e.g. "/hello/world/" -> "world"</summary>
        /// <param name="path">The path whose name to get.</param>
        /// <returns>The name of the path or empty.</returns>
        public static string GetName(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            int slashIndex = GetLastSignificatSlash(path);
            int lastSlashIndex = path.LastIndexOf('/');
            if (lastSlashIndex == slashIndex)
                return path.Substring(slashIndex + 1);

            return path.Substring(slashIndex + 1, lastSlashIndex - slashIndex - 1);
        }

        private static int GetLastSignificatSlash(string path)
        {
            int i = path.Length - 1;
            for (; i >= 0; i--)
            {
                if (path[i] != '/')
                    break;
            }
            for (; i >= 0; i--)
            {
                if (path[i] == '/')
                    break;
            }
            return i;
        }

        public string Encode()
        {
            return ToString().Replace(Amp, "&amp;");
        }

        /// <summary>Mimics the behavior of VirtualPathUtility.Combine with less restrictions and minimal dependencies.</summary>
        /// <param name="url1">First part</param>
        /// <param name="url2">Last part</param>
        /// <returns>The combined URL.</returns>
        public static string Combine(string url1, string url2)
        {
            if (string.IsNullOrEmpty(url2))
                return ToAbsolute(url1);
            if (string.IsNullOrEmpty(url1))
                return ToAbsolute(url2);

            if (url2.StartsWith("/"))
                return url2;
            if (url2.StartsWith("~"))
                return ToAbsolute(url2);
            if (url2.StartsWith("{"))
                return url2;
            if (url2.IndexOf(':') >= 0)
                return url2;

            Url first = url1;
            Url second = url2;

            return first.AppendSegment(second.Path, "").AppendQuery(second.Query).SetFragment(second.Fragment);
        }

        /// <summary>Converts a text query string to a dictionary.</summary>
        /// <param name="query">The query string</param>
        /// <returns>A dictionary of the query parts.</returns>
        public static IDictionary<string, string> ParseQueryString(string query)
        {
            var dictionary = new Dictionary<string, string>();
            if (query == null)
                return dictionary;

            string[] queries = query.Split(querySplitter, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < queries.Length; i++)
            {
                string q = queries[i];
                int eqIndex = q.IndexOf("=");
                if (eqIndex >= 0)
                    dictionary[q.Substring(0, eqIndex)] = q.Substring(eqIndex + 1);
            }
            return dictionary;
        }

        /// <summary>Changes the application base of an url.</summary>
        /// <param name="currentPath">Replaces an absolute url in one app, to the absolute path of another app.</param>
        /// <param name="fromAppPath">The origin application path.</param>
        /// <param name="toAppPath">The destination application path.</param>
        /// <returns>A rebased url.</returns>
        public static string Rebase(string currentPath, string fromAppPath, string toAppPath)
        {
            if (currentPath == null || fromAppPath == null || !currentPath.StartsWith(fromAppPath))
                return currentPath;

            return toAppPath + currentPath.Substring(fromAppPath.Length);
        }

        /// <summary>Gets a replacement used by <see cref="ResolveTokens"/>.</summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string GetToken(string token)
        {
            string value = null;
            replacements.TryGetValue(token, out value);
            return value;
        }

        /// <summary>Adds a replacement used by <see cref="ResolveTokens"/>.</summary>
        /// <param name="token">They token to replace.</param>
        /// <param name="value">The value to replace the token with.</param>
        public static void SetToken(string token, string value)
        {
            if (token == null) throw new ArgumentNullException("key");

            if (value != null)
                replacements[token] = value;
            else if (replacements.ContainsKey(token))
                replacements.Remove(token);
        }

        /// <summary>Formats an url using replacement tokens.</summary>
        /// <param name="urlFormat">The url format to resolve, e.g. {ManagementUrl}/Resources/icons/add.png</param>
        /// <returns>An an absolut path with all tokens replaced by their corresponding value.</returns>
        public static string ResolveTokens(string urlFormat)
        {
            if (string.IsNullOrEmpty(urlFormat))
                return urlFormat;

            foreach (var kvp in replacements)
                urlFormat = urlFormat.Replace(kvp.Key, kvp.Value);
            return ToAbsolute(urlFormat);
        }

        /// <summary>Formsats this url using replacement tokens.</summary>
        /// <returns>An url without replacement tokens.</returns>
        public Url ResolveTokens()
        {
            return new Url(ResolveTokens(ToString()));
        }
    }
}
