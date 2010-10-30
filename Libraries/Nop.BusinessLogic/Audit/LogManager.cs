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
    public partial class LogManager : ILogManager
    {
        #region Fields

        /// <summary>
        /// object context
        /// </summary>
        protected NopObjectContext _context;

        #endregion
        
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public LogManager(NopObjectContext context)
        {
            _context = context;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a log item
        /// </summary>
        /// <param name="logId">Log item identifier</param>
        public void DeleteLog(int logId)
        {
            var log = GetLogById(logId);
            if (log == null)
                return;

            
            if (!_context.IsAttached(log))
                _context.Log.Attach(log);
            _context.DeleteObject(log);
            _context.SaveChanges();
        }

        /// <summary>
        /// Clears a log
        /// </summary>
        public void ClearLog()
        {
            
            var log = _context.Log.ToList();
            foreach (var logItem in log)
                _context.DeleteObject(logItem);
            _context.SaveChanges();
        }

        /// <summary>
        /// Gets all log items
        /// </summary>
        /// <returns>Log item collection</returns>
        public List<Log> GetAllLogs()
        {
            
            var query = from l in _context.Log
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
        public Log GetLogById(int logId)
        {
            if (logId == 0)
                return null;
            
            var query = from l in _context.Log
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
        public Log InsertLog(LogTypeEnum logType, string message, string exception)
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
        public Log InsertLog(LogTypeEnum logType, string message, Exception exception)
        {
            int customerId = 0;
            if (NopContext.Current != null && NopContext.Current.User != null)
                customerId = NopContext.Current.User.CustomerId;
            string IPAddress = NopContext.Current.UserHostAddress;
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
        public Log InsertLog(LogTypeEnum logType, int severity, string message,
            Exception exception, string IPAddress,
            int customerId, string pageUrl)
        {
            //don't log thread abort exception
            if ((exception != null) && (exception is System.Threading.ThreadAbortException))
                return null;

            string exceptionStr = exception == null ? string.Empty : exception.ToString();

            string referrerUrl = string.Empty;
            if (HttpContext.Current != null &&
                HttpContext.Current.Request != null &&
                HttpContext.Current.Request.UrlReferrer != null)
                referrerUrl = HttpContext.Current.Request.UrlReferrer.ToString();
           
            message = CommonHelper.EnsureNotNull(message);
            message = CommonHelper.EnsureMaximumLength(message, 1000);
            exceptionStr = CommonHelper.EnsureNotNull(exceptionStr);
            exceptionStr = CommonHelper.EnsureMaximumLength(exceptionStr, 4000);
            IPAddress = CommonHelper.EnsureNotNull(IPAddress);
            IPAddress = CommonHelper.EnsureMaximumLength(IPAddress, 100);
            pageUrl = CommonHelper.EnsureNotNull(pageUrl);
            pageUrl = CommonHelper.EnsureMaximumLength(pageUrl, 100);
            referrerUrl = CommonHelper.EnsureNotNull(referrerUrl);
            referrerUrl = CommonHelper.EnsureMaximumLength(referrerUrl, 100);

            

            var log = _context.Log.CreateObject();
            log.LogTypeId = (int)logType;
            log.Severity = severity;
            log.Message = message;
            log.Exception = exceptionStr;
            log.IPAddress = IPAddress;
            log.CustomerId = customerId;
            log.PageUrl = pageUrl;
            log.ReferrerUrl = referrerUrl;
            log.CreatedOn = DateTime.UtcNow;

            _context.Log.AddObject(log);
            _context.SaveChanges();

            return log;
        }

        #endregion
    }
}
