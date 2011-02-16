using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;

namespace Nop.Core.Web
{
    /// <summary>
    /// Is notified of errors in the web context and tries to do something barely useful about them.
    /// </summary>
    [Service(typeof(IErrorHandler))]
    public class ErrorHandler : IErrorHandler
    {
        private readonly object syncLock = new object();
        private long errorCount;
        private long errorsThisHour;
        private int hour = DateTime.Now.Hour;

        public ErrorHandler()
        {
        }

        /// <summary>Total number of errors since startup.</summary>
        public long ErrorCount
        {
            get { return errorCount; }
        }

        #region IErrorHandler Members

        public void Notify(Exception ex)
        {
            errorCount++;

            if (ex is HttpUnhandledException)
                ex = ex.InnerException;
            if (ex is TargetInvocationException)
                ex = ex.InnerException;

            if (ex != null)
            {
                Trace.TraceError("ErrorHandler.Notify: " + FormatError(ex));

                UpdateErrorCount();
            }
        }

        #endregion

        private void UpdateErrorCount()
        {
            lock (syncLock)
            {
                if (DateTime.Now.Hour == hour)
                {
                    ++errorsThisHour;
                }
                else
                {
                    hour = DateTime.Now.Hour;
                    errorsThisHour = 0;
                }
            }
        }

        

        private static string FormatError(Exception ex)
        {
            var ctx = HttpContext.Current;
            var body = new StringBuilder();
            if (ctx != null)
            {
                if (ctx.Request != null)
                {
                    body.Append("Url: ").AppendLine(ctx.Request.Url.ToString());
                    if (ctx.Request.UrlReferrer != null)
                        body.Append("Referrer: ").AppendLine(ctx.Request.UrlReferrer.ToString());
                    body.Append("User Address: ").AppendLine(ctx.Request.UserHostAddress);
                }
                if (ctx.User != null)
                {
                    body.Append("User: ").AppendLine(ctx.User.Identity.Name);
                }
            }
            body.Append(ex.ToString());
            return body.ToString();
        }
    }
}
