using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;

namespace Nop.Core.Web
{
    /// <summary>
    /// A thread local context. It's used to store the nhibernate session 
    /// instance in situations where we don't have a request available such
    /// as code executed by the scheduler.
    /// </summary>
    public class ThreadContext : IWebContext, IDisposable
    {
        private static string baseDirectory;
       
        static IDictionary items;
        [ThreadStatic]
        Url localUrl = new Url("/");
        [ThreadStatic]
        Url url = new Url("http://localhost");

        static ThreadContext()
        {
            baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            int binIndex = baseDirectory.IndexOf("\\bin\\");
            if (binIndex >= 0)
                baseDirectory = baseDirectory.Substring(0, binIndex);
            else if (baseDirectory.EndsWith("\\bin"))
                baseDirectory = baseDirectory.Substring(0, baseDirectory.Length - 4);
        }


        public virtual IDictionary RequestItems
        {
            get { return items ?? (items = new Hashtable()); }
        }

        public virtual IPrincipal User
        {
            get { return Thread.CurrentPrincipal; }
        }

        public virtual bool IsWeb
        {
            get { return false; }
        }

        /// <summary>Specifies whether the UrlAuthorizationModule should skip authorization for the current request.</summary>
        public bool SkipAuthorization { get; set; }

        public virtual void Close()
        {
            string[] keys = new string[RequestItems.Keys.Count];
            RequestItems.Keys.CopyTo(keys, 0);

            foreach (string key in keys)
            {
                IClosable value = RequestItems[key] as IClosable;
                if (value != null)
                {
                    (value as IClosable).Dispose();
                }
            }
            items = null;
        }

        /// <summary>The requested url, e.g. http://n2cms.com/path/to/a/page.aspx?some=query.</summary>
        public virtual Url Url
        {
            get { return url; }
            set { url = value; }
        }

        public virtual string MapPath(string path)
        {
            path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
            return Path.Combine(baseDirectory, path);
        }

        public virtual IHttpHandler Handler
        {
            get { throw new NotSupportedException("In thread context. No handler when not running in http web context."); }
        }

        public virtual HttpRequest Request
        {
            get { throw new NotSupportedException("In thread context. No handler when not running in http web context."); }
        }

        public virtual HttpResponse Response
        {
            get { throw new NotSupportedException("In thread context. No handler when not running in http web context."); }
        }

        public virtual string PhysicalPath
        {
            get { return MapPath(Url.Path); }
        }

        public virtual string ToAbsolute(string virtualPath)
        {
            return virtualPath.TrimStart('~');
        }

        public virtual string ToAppRelative(string virtualPath)
        {
            return "~" + ToAbsolute(virtualPath);
        }

        /// <summary>Doen't do anything.</summary>
        public void ClearError()
        {
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            Close();
        }

        #endregion
    }
}
