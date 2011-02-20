
using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Hosting;
using Nop.Core.Infrastructure.DependencyManagement;

namespace Nop.Core
{
    /// <summary>
    /// Represents a common helper
    /// </summary>
    public partial class WebHelper : IWebHelper
    {
        /// <summary>
        /// Get URL referrer
        /// </summary>
        /// <returns>URL referrer</returns>
        public virtual string GetUrlReferrer()
        {
            string referrerUrl = string.Empty;

            if (HttpContext.Current != null &&
                HttpContext.Current.Request != null &&
                HttpContext.Current.Request.UrlReferrer != null)
                referrerUrl = HttpContext.Current.Request.UrlReferrer.ToString();

            return referrerUrl;
        }

        /// <summary>
        /// Get context IP address
        /// </summary>
        /// <returns>URL referrer</returns>
        public virtual string GetCurrentIpAddress()
        {
            if (HttpContext.Current != null &&
                    HttpContext.Current.Request != null &&
                    HttpContext.Current.Request.UserHostAddress != null)
                return HttpContext.Current.Request.UserHostAddress;
            else
                return string.Empty;
        }

        /// <summary>
        /// Gets this page name
        /// </summary>
        /// <returns></returns>
        public virtual string GetThisPageUrl(bool includeQueryString)
        {
            string url = string.Empty;
            if (HttpContext.Current == null)
                return url;

            if (includeQueryString)
            {
                bool useSsl = IsCurrentConnectionSecured();
                string storeHost = GetStoreHost(useSsl);
                if (storeHost.EndsWith("/"))
                    storeHost = storeHost.Substring(0, storeHost.Length - 1);
                url = storeHost + HttpContext.Current.Request.RawUrl;
            }
            else
            {
                url = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path);
            }
            url = url.ToLowerInvariant();
            return url;
        }

        /// <summary>
        /// Gets a value indicating whether current connection is secured
        /// </summary>
        /// <returns>true - secured, false - not secured</returns>
        public virtual bool IsCurrentConnectionSecured()
        {
            bool useSsl = false;
            if (HttpContext.Current != null && HttpContext.Current.Request != null)
            {
                useSsl = HttpContext.Current.Request.IsSecureConnection;
                //when your hosting uses a load balancer on their server then the Request.IsSecureConnection is never got set to true, use the statement below
                //just uncomment it
                //useSSL = HttpContext.Current.Request.ServerVariables["HTTP_CLUSTER_HTTPS"] == "on" ? true : false;
            }

            return useSsl;
        }
        
        /// <summary>
        /// Gets server variable by name
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Server variable</returns>
        public virtual string ServerVariables(string name)
        {
            string tmpS = string.Empty;
            try
            {
                if (HttpContext.Current.Request.ServerVariables[name] != null)
                {
                    tmpS = HttpContext.Current.Request.ServerVariables[name];
                }
            }
            catch
            {
                tmpS = string.Empty;
            }
            return tmpS;
        }

        /// <summary>
        /// Gets store host location
        /// </summary>
        /// <param name="useSsl">Use SSL</param>
        /// <returns>Store host location</returns>
        public virtual string GetStoreHost(bool useSsl)
        {
            string result = "http://" + ServerVariables("HTTP_HOST");
            if (!result.EndsWith("/"))
                result += "/";
            if (useSsl)
            {
                //shared SSL certificate URL
                string sharedSslUrl = string.Empty;
                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["SharedSSLUrl"]))
                    sharedSslUrl = ConfigurationManager.AppSettings["SharedSSLUrl"].Trim();

                if (!String.IsNullOrEmpty(sharedSslUrl))
                {
                    //shared SSL
                    result = sharedSslUrl;
                }
                else
                {
                    //standard SSL
                    result = result.Replace("http:/", "https:/");
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["UseSSL"])
                    && Convert.ToBoolean(ConfigurationManager.AppSettings["UseSSL"]))
                {
                    //SSL is enabled

                    //get shared SSL certificate URL
                    string sharedSslUrl = string.Empty;
                    if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["SharedSSLUrl"]))
                        sharedSslUrl = ConfigurationManager.AppSettings["SharedSSLUrl"].Trim();
                    if (!String.IsNullOrEmpty(sharedSslUrl))
                    {
                        //shared SSL

                        /* we need to set a store URL here (IoC.Resolve<ISettingManager>().StoreUrl property)
                         * but we cannot reference Nop.BusinessLogic.dll assembly.
                         * So we are using one more app config settings - <add key="NonSharedSSLUrl" value="http://www.yourStore.com" />
                         */
                        string nonSharedSslUrl = string.Empty;
                        if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["NonSharedSSLUrl"]))
                            nonSharedSslUrl = ConfigurationManager.AppSettings["NonSharedSSLUrl"].Trim();
                        if (string.IsNullOrEmpty(nonSharedSslUrl))
                            throw new Exception("NonSharedSSLUrl app config setting is not empty");
                        result = nonSharedSslUrl;
                    }
                }
            }

            if (!result.EndsWith("/"))
                result += "/";

            return result.ToLowerInvariant();
        }
        
        /// <summary>
        /// Gets store location
        /// </summary>
        /// <returns>Store location</returns>
        public virtual string GetStoreLocation()
        {
            bool useSsl = IsCurrentConnectionSecured();
            return GetStoreLocation(useSsl);
        }

        /// <summary>
        /// Gets store location
        /// </summary>
        /// <param name="useSsl">Use SSL</param>
        /// <returns>Store location</returns>
        public virtual string GetStoreLocation(bool useSsl)
        {
            //return HostingEnvironment.ApplicationVirtualPath;

            string result = GetStoreHost(useSsl);
            if (result.EndsWith("/"))
                result = result.Substring(0, result.Length - 1);
            result = result + HttpContext.Current.Request.ApplicationPath;
            if (!result.EndsWith("/"))
                result += "/";

            return result.ToLowerInvariant();
        }
        
        /// <summary>
        /// Returns true if the requested resource is one of the typical resources that needn't be processed by the cms engine.
        /// </summary>
        /// <param name="application">HTTP Application</param>
        /// <returns>True if the request targets a static resource file.</returns>
        /// <remarks>
        /// These are the file extensions considered to be static resources:
        /// .css
        ///	.gif
        /// .png 
        /// .jpg
        /// .jpeg
        /// .js
        /// .axd
        /// .ashx
        /// </remarks>
        public virtual bool IsStaticResource(HttpApplication application)
        {
            if (application == null)
                throw new ArgumentNullException("application");

            string path = application.Request.Path;
            string extension = VirtualPathUtility.GetExtension(path);

            if (extension == null) return false;

            switch (extension.ToLower())
            {
                case ".css":
                case ".gif":
                case ".png":
                case ".jpg":
                case ".jpeg":
                case ".js":
                case ".axd":
                case ".ashx":
                    return true;
            }

            return false;
        }
        
        /// <summary>
        /// Maps a virtual path to a physical disk path.
        /// </summary>
        /// <param name="path">The path to map. E.g. "~/bin"</param>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        public virtual string MapPath(string path)
        {
            if (HttpContext.Current != null)
            {
                return HostingEnvironment.MapPath(path);
            }
            else
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                int binIndex = baseDirectory.IndexOf("\\bin\\");
                if (binIndex >= 0)
                    baseDirectory = baseDirectory.Substring(0, binIndex);
                else if (baseDirectory.EndsWith("\\bin"))
                    baseDirectory = baseDirectory.Substring(0, baseDirectory.Length - 4);

                path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
                return Path.Combine(baseDirectory, path);
            }
        }
    }
}
