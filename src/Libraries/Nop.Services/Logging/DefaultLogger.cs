
using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Logging;
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
        private readonly IWorkContext _workContext;
        
        #endregion
        
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="logRepository">Log repository</param>
        public DefaultLogger(IRepository<Log> logRepository,
            IWorkContext workContext)
        {
            this._logRepository = logRepository;
            this._workContext = workContext;
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
            if (_workContext != null && _workContext.CurrentCustomer != null)
                customerId = _workContext.CurrentCustomer.Id;
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
