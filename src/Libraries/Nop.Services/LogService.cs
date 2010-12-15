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
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain;
using Nop.Data;

namespace Nop.Services
{
    /// <summary>
    /// Log service
    /// </summary>
    public partial class LogService : ILogService
    {
        #region Fields

        private readonly IRepository<Log> _logRespository;
        
        #endregion
        
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="logRespository">Log repository</param>
        public LogService(IRepository<Log> logRespository)
        {
            this._logRespository = logRespository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a log item
        /// </summary>
        /// <param name="log">Log item</param>
        public void DeleteLog(Log log)
        {
            if (log == null)
                return;

            _logRespository.Delete(log);
        }

        /// <summary>
        /// Clears a log
        /// </summary>
        public void ClearLog()
        {
            var log = _logRespository.Table.ToList();
            foreach (var logItem in log)
                _logRespository.Delete(logItem);
        }

        /// <summary>
        /// Gets all log items
        /// </summary>
        /// <param name="createdOnFrom">Log item creation from; null to load all customers</param>
        /// <param name="createdOnTo">Log item creation to; null to load all customers</param>
        /// <param name="message">Message</param>
        /// <param name="logTypeId">Log type identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Log item collection</returns>
        public PagedList<Log> GetAllLogs(DateTime? createdOnFrom,
           DateTime? createdOnTo, string message, int logTypeId, int pageIndex, int pageSize)
        {
            var query = from l in _logRespository.Table
                        where (!createdOnFrom.HasValue || createdOnFrom.Value <= l.CreatedOnUtc) &&
                        (!createdOnTo.HasValue || createdOnTo.Value >= l.CreatedOnUtc) &&
                        (logTypeId == 0 || logTypeId == l.LogTypeId) &&
                        (String.IsNullOrEmpty(message) || l.Message.Contains(message) || l.Exception.Contains(message))
                        orderby l.CreatedOnUtc descending
                        select l;
            var log = new PagedList<Log>(query, pageIndex, pageSize);
            return log;
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

            var log = _logRespository.GetById(logId);
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
            //TODO: uncomment when customers are implemented
            //if (NopContext.Current.User != null)
            //    customerId = NopContext.Current.User.Id;
            string ipAddress = CommonHelper.GetCurrentIpAddress();
            string pageUrl = CommonHelper.GetThisPageUrl(true);

            return InsertLog(logType, 11, message, exception, ipAddress, customerId, pageUrl);
        }

        /// <summary>
        /// Inserts a log item
        /// </summary>
        /// <param name="logType">Log item type</param>
        /// <param name="severity">The severity</param>
        /// <param name="message">The short message</param>
        /// <param name="exception">The full exception</param>
        /// <param name="ipAddress">The IP address</param>
        /// <param name="customerId">The customer identifier</param>
        /// <param name="pageUrl">The page URL</param>
        /// <returns>Log item</returns>
        public Log InsertLog(LogTypeEnum logType, int severity, string message,
            Exception exception, string ipAddress,
            int customerId, string pageUrl)
        {
            //don't log thread abort exception
            if ((exception != null) && (exception is System.Threading.ThreadAbortException))
                return null;

            string exceptionStr = exception == null ? string.Empty : exception.ToString();

            string referrerUrl = CommonHelper.GetUrlReferrer();
           
            message = CommonHelper.EnsureNotNull(message);
            exceptionStr = CommonHelper.EnsureNotNull(exceptionStr);
            ipAddress = CommonHelper.EnsureNotNull(ipAddress);
            ipAddress = CommonHelper.EnsureMaximumLength(ipAddress, 200);
            pageUrl = CommonHelper.EnsureNotNull(pageUrl);
            referrerUrl = CommonHelper.EnsureNotNull(referrerUrl);

            var log = new Log()
                          {
                              LogTypeId = (int) logType,
                              Severity = severity,
                              Message = message,
                              Exception = exceptionStr,
                              IpAddress = ipAddress,
                              CustomerId = customerId,
                              PageUrl = pageUrl,
                              ReferrerUrl = referrerUrl,
                              CreatedOnUtc = DateTime.UtcNow
                          };

            _logRespository.Insert(log);

            return log;
        }

        #endregion
    }
}
