using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace Nop.Core.Web
{
    /// <summary>
    /// A web context wrapper that maps to the web request context for calls in 
    /// web application scope and thread context when no request has been made 
    /// (e.g. when executing code in scheduled action).
    /// </summary>
    public class AdaptiveContext : IWebContext, IDisposable
    {
        readonly IWebContext thread = new ThreadContext();
        readonly IWebContext web = new WebRequestContext();

        /// <summary>Gets wether there is a web context availabe.</summary>
        public bool IsWeb
        {
            get { return HttpContext.Current != null; }
        }

        /// <summary>Returns either the web or the thread context depending on <see cref="IsWeb"/>.</summary>
        protected IWebContext CurrentContext
        {
            get { return IsWeb ? web : thread; }
        }

        /// <summary>Gets a dictionary of request scoped items.</summary>
        public IDictionary RequestItems
        {
            get { return CurrentContext.RequestItems; }
        }

        /// <summary>Gets the current user principal (may be null).</summary>
        public IPrincipal User
        {
            get { return CurrentContext.User; }
        }

        /// <summary>The current request object.</summary>
        public HttpRequest Request
        {
            get { return CurrentContext.Request; }
        }

        /// <summary>The current response object.</summary>
        public HttpResponse Response
        {
            get { return CurrentContext.Response; }
        }

        /// <summary>The handler associated with this request.</summary>
        public IHttpHandler Handler
        {
            get { return CurrentContext.Handler; }
        }

        /// <summary>Specifies whether the UrlAuthorizationModule should skip authorization for the current request.</summary>
        public bool SkipAuthorization
        {
            get { return CurrentContext.SkipAuthorization; }
        }

        /// <summary>The physical path on disk to the requested page.</summary>
        public string PhysicalPath
        {
            get { return CurrentContext.PhysicalPath; }
        }

        /// <summary>The host part of the requested url, e.g. http://n2cms.com/path/to/a/page.aspx?some=query.</summary>
        public Url Url
        {
            get { return CurrentContext.Url; }
        }

        /// <summary>Converts a virtual path to an an absolute path. E.g. ~/hello.aspx -> /MyVirtualDirectory/hello.aspx.</summary>
        /// <param name="virtualPath">The virtual url to make absolute.</param>
        /// <returns>The absolute url.</returns>
        public string ToAbsolute(string virtualPath)
        {
            return CurrentContext.ToAbsolute(virtualPath);
        }

        /// <summary>Converts an absolute url to an app relative path. E.g. /MyVirtualDirectory/hello.aspx -> ~/hello.aspx.</summary>
        /// <param name="virtualPath">The absolute url to convert.</param>
        /// <returns>An app relative url.</returns>
        public string ToAppRelative(string virtualPath)
        {
            return CurrentContext.ToAppRelative(virtualPath);
        }

        /// <summary>Maps a virtual path to a physical disk path.</summary>
        /// <param name="path">The path to map. E.g. "~/bin"</param>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        public string MapPath(string path)
        {
            return CurrentContext.MapPath(path);
        }

        /// <summary>Calls into HttpContext.ClearError().</summary>
        public void ClearError()
        {
            CurrentContext.ClearError();
        }

        /// <summary>Disposes request items that needs disposing. This method should be called at the end of each request.</summary>
        public void Close()
        {
            CurrentContext.Close();
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            Close();
        }

        #endregion
    }
}
