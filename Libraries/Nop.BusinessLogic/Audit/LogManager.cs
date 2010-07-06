//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.BusinessLogic.Audit
{
    /// <summary>
    /// Log manager
    /// </summary>
    public partial class LogManager
    {
        #region Methods

        /// <summary>
        /// Deletes a log item
        /// </summary>
        /// <param name="logId">Log item identifier</param>
        public static void DeleteLog(int logId)
        {
            var log = GetLogById(logId);
            if (log == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(log))
                context.Log.Attach(log);
            context.DeleteObject(log);
            context.SaveChanges();
        }

        /// <summary>
        /// Clears a log
        /// </summary>
        public static void ClearLog()
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            context.Sp_LogClear();
        }

        /// <summary>
        /// Gets all log items
        /// </summary>
        /// <returns>Log item collection</returns>
        public static List<Log> GetAllLogs()
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from l in context.Log
                        orderby l.CreatedOn descending
                        select l;
            var collection = query.ToList();
            return collection;
        }

        /// <summary>
        /// Gets a log item
        /// </summary>
        /// <param name="logId">Log item identifier</param>
        /// <returns>Log item</returns>
        public static Log GetLogById(int logId)
        {
            if (logId == 0)
                return null;
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from l in context.Log
                        where l.LogId == logId
                        select l;
            var log = query.SingleOrDefault();
            return log;
        }
        
        /// <summary>
        /// Inserts a log item
        /// </summary>
        /// <param name="logType">Log item type</param>
        /// <param name="message">The short message</param>
        /// <param name="exception">The exception</param>
        /// <returns>A log item</returns>
        public static Log InsertLog(LogTypeEnum logType, string message, string exception)
        {
            return InsertLog(logType, message, new Exception(String.IsNullOrEmpty(exception) ? string.Empty : exception));
        }

        /// <summary>
        /// Inserts a log item
        /// </summary>
        /// <param name="logType">Log item type</param>
        /// <param name="message">The short message</param>
        /// <param name="exception">The exception</param>
        /// <returns>A log item</returns>
        public static Log InsertLog(LogTypeEnum logType, string message, Exception exception)
        {
            int customerId = 0;
            if (NopContext.Current != null && NopContext.Current.User != null)
                customerId = NopContext.Current.User.CustomerId;
            string IPAddress = string.Empty;
            if (HttpContext.Current != null && HttpContext.Current.Request!=null)
                IPAddress = HttpContext.Current.Request.UserHostAddress;
            string pageUrl = CommonHelper.GetThisPageUrl(true);

            return InsertLog(logType, 11, message, exception, IPAddress, customerId, pageUrl);
        }

        /// <summary>
        /// Inserts a log item
        /// </summary>
        /// <param name="logType">Log item type</param>
        /// <param name="severity">The severity</param>
        /// <param name="message">The short message</param>
        /// <param name="exception">The full exception</param>
        /// <param name="IPAddress">The IP address</param>
        /// <param name="customerId">The customer identifier</param>
        /// <param name="pageUrl">The page URL</param>
        /// <returns>Log item</returns>
        public static Log InsertLog(LogTypeEnum logType, int severity, string message,
            Exception exception, string IPAddress,
            int customerId, string pageUrl)
        {
            //don't log thread abort exception
            if ((exception != null) && (exception is System.Threading.ThreadAbortException))
                return null;

            if (IPAddress == null)
                IPAddress = string.Empty;

            string exceptionStr = exception == null ? string.Empty : exception.ToString();

            string referrerUrl = string.Empty;
            if (HttpContext.Current != null &&
                HttpContext.Current.Request != null &&
                HttpContext.Current.Request.UrlReferrer != null)
                referrerUrl = HttpContext.Current.Request.UrlReferrer.ToString();
            if (referrerUrl == null)
                referrerUrl = string.Empty;

            message = CommonHelper.EnsureMaximumLength(message, 1000);
            exceptionStr = CommonHelper.EnsureMaximumLength(exceptionStr, 4000);
            IPAddress = CommonHelper.EnsureMaximumLength(IPAddress, 100);
            pageUrl = CommonHelper.EnsureMaximumLength(pageUrl, 100);
            referrerUrl = CommonHelper.EnsureMaximumLength(referrerUrl, 100);

            DateTime createdOn = DateTime.UtcNow;

            var context = ObjectContextHelper.CurrentObjectContext;

            var log = context.Log.CreateObject();
            log.LogTypeId = (int)logType;
            log.Severity = severity;
            log.Message = message;
            log.Exception = exceptionStr;
            log.IPAddress = IPAddress;
            log.CustomerId = customerId;
            log.PageUrl = pageUrl;
            log.ReferrerUrl = referrerUrl;
            log.CreatedOn = createdOn;

            context.Log.AddObject(log);
            context.SaveChanges();

            return log;
        }

        #endregion
    }
}
