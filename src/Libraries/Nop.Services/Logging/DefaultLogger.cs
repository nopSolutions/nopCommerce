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

namespace Nop.Services.Logging
{
    /// <summary>
    /// Default logger
    /// </summary>
    public partial class DefaultLogger : ILogger
    {
        #region Fields

        private readonly IRepository<Log> _logRepository;
        
        #endregion
        
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="logRepository">Log repository</param>
        public DefaultLogger(IRepository<Log> logRepository)
        {
            this._logRepository = logRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether a log level is enabled
        /// </summary>
        /// <param name="level">Log level</param>
        /// <returns>Result</returns>
        public bool IsEnabled(LogLevel level)
        {
            //TODO: use ISettingService to determine it
            switch(level)
            {
                case LogLevel.Debug:
                    return false;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Deletes a log item
        /// </summary>
        /// <param name="log">Log item</param>
        public void DeleteLog(Log log)
        {
            if (log == null)
                return;

            _logRepository.Delete(log);
        }

        /// <summary>
        /// Clears a log
        /// </summary>
        public void ClearLog()
        {
            var log = _logRepository.Table.ToList();
            foreach (var logItem in log)
                _logRepository.Delete(logItem);
        }

        /// <summary>
        /// Gets all log items
        /// </summary>
        /// <param name="fromUtc">Log item creation from; null to load all records</param>
        /// <param name="toUtc">Log item creation to; null to load all records</param>
        /// <param name="message">Message</param>
        /// <param name="logLevel">Log level; null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Log item collection</returns>
        public IPagedList<Log> GetAllLogs(DateTime? fromUtc, DateTime? toUtc,
            string message, LogLevel? logLevel, int pageIndex, int pageSize)
        {
            int? logLevelId = null;
            if (logLevel.HasValue)
                logLevelId = (int)logLevel.Value;

            var query = from l in _logRepository.Table
                        where (!fromUtc.HasValue || fromUtc.Value <= l.CreatedOnUtc) &&
                        (!toUtc.HasValue || toUtc.Value >= l.CreatedOnUtc) &&
                        (!logLevelId.HasValue || logLevelId.Value == l.LogLevelId) &&
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

            var log = _logRepository.GetById(logId);
            return log;
        }
        
        /// <summary>
        /// Inserts a log item
        /// </summary>
        /// <param name="logLevel">Log level</param>
        /// <param name="message">The short message</param>
        /// <param name="exception">The exception</param>
        /// <returns>A log item</returns>
        public Log InsertLog(LogLevel logLevel, string message, string exception)
        {
            return InsertLog(logLevel, message, new Exception(String.IsNullOrEmpty(exception) ? string.Empty : exception));
        }

        /// <summary>
        /// Inserts a log item
        /// </summary>
        /// <param name="logLevel">Log level</param>
        /// <param name="message">The short message</param>
        /// <param name="exception">The exception</param>
        /// <returns>A log item</returns>
        public Log InsertLog(LogLevel logLevel, string message, Exception exception)
        {
            //don't log thread abort exception
            if ((exception != null) && (exception is System.Threading.ThreadAbortException))
                return null;

            string exceptionStr = exception == null ? string.Empty : exception.ToString();


            int customerId = 0;
            //TODO: uncomment when customers are implemented
            //if (NopContext.Current.User != null)
            //    customerId = NopContext.Current.User.Id;
            string ipAddress = WebHelper.GetCurrentIpAddress();
            string pageUrl = WebHelper.GetThisPageUrl(true);
            string referrerUrl = WebHelper.GetUrlReferrer();

            ipAddress = CommonHelper.EnsureNotNull(ipAddress);
            pageUrl = CommonHelper.EnsureNotNull(pageUrl);
            referrerUrl = CommonHelper.EnsureNotNull(referrerUrl);

            var log = new Log()
            {
                LogLevel = logLevel,
                Message = message,
                Exception = exceptionStr,
                IpAddress = ipAddress,
                CustomerId = customerId,
                PageUrl = pageUrl,
                ReferrerUrl = referrerUrl,
                CreatedOnUtc = DateTime.UtcNow
            };

            _logRepository.Insert(log);

            return log;
        }

        #endregion
    }
}
