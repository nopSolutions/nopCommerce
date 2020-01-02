using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Data;
using Nop.Data.Extensions;

namespace Nop.Services.Logging
{
    /// <summary>
    /// Default logger
    /// </summary>
    public partial class DefaultLogger : ILogger
    {
        #region Fields

        private readonly CommonSettings _commonSettings;
        private readonly IDbContext _dbContext;
        private readonly IRepository<Log> _logRepository;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public DefaultLogger(CommonSettings commonSettings,
            IDbContext dbContext,
            IRepository<Log> logRepository,
            IWebHelper webHelper)
        {
            _commonSettings = commonSettings;
            _dbContext = dbContext;
            _logRepository = logRepository;
            _webHelper = webHelper;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets a value indicating whether this message should not be logged
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>Result</returns>
        protected virtual bool IgnoreLog(string message)
        {
            if (!_commonSettings.IgnoreLogWordlist.Any())
                return false;

            if (string.IsNullOrWhiteSpace(message))
                return false;

            return _commonSettings
                .IgnoreLogWordlist
                .Any(x => message.IndexOf(x, StringComparison.InvariantCultureIgnoreCase) >= 0);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether a log level is enabled
        /// </summary>
        /// <param name="level">Log level</param>
        /// <returns>Result</returns>
        public virtual bool IsEnabled(LogLevel level)
        {
            switch (level)
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
        public virtual void DeleteLog(Log log)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            _logRepository.Delete(log);
        }

        /// <summary>
        /// Deletes a log items
        /// </summary>
        /// <param name="logs">Log items</param>
        public virtual void DeleteLogs(IList<Log> logs)
        {
            if (logs == null)
                throw new ArgumentNullException(nameof(logs));

            _logRepository.Delete(logs);
        }

        /// <summary>
        /// Clears a log
        /// </summary>
        public virtual void ClearLog()
        {
            //do all databases support "Truncate command"?
            var logTableName = _dbContext.GetTableName<Log>();
            _dbContext.ExecuteSqlCommand($"TRUNCATE TABLE [{logTableName}]");

            //var log = _logRepository.Table.ToList();
            //foreach (var logItem in log)
            //    _logRepository.Delete(logItem);
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
        /// <returns>Log item items</returns>
        public virtual IPagedList<Log> GetAllLogs(DateTime? fromUtc = null, DateTime? toUtc = null,
            string message = "", LogLevel? logLevel = null,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _logRepository.Table;
            if (fromUtc.HasValue)
                query = query.Where(l => fromUtc.Value <= l.CreatedOnUtc);
            if (toUtc.HasValue)
                query = query.Where(l => toUtc.Value >= l.CreatedOnUtc);
            if (logLevel.HasValue)
            {
                var logLevelId = (int)logLevel.Value;
                query = query.Where(l => logLevelId == l.LogLevelId);
            }

            if (!string.IsNullOrEmpty(message))
                query = query.Where(l => l.ShortMessage.Contains(message) || l.FullMessage.Contains(message));
            query = query.OrderByDescending(l => l.CreatedOnUtc);

            var log = new PagedList<Log>(query, pageIndex, pageSize);
            return log;
        }

        /// <summary>
        /// Gets a log item
        /// </summary>
        /// <param name="logId">Log item identifier</param>
        /// <returns>Log item</returns>
        public virtual Log GetLogById(int logId)
        {
            if (logId == 0)
                return null;

            return _logRepository.GetById(logId);
        }

        /// <summary>
        /// Get log items by identifiers
        /// </summary>
        /// <param name="logIds">Log item identifiers</param>
        /// <returns>Log items</returns>
        public virtual IList<Log> GetLogByIds(int[] logIds)
        {
            if (logIds == null || logIds.Length == 0)
                return new List<Log>();

            var query = from l in _logRepository.Table
                        where logIds.Contains(l.Id)
                        select l;
            var logItems = query.ToList();
            //sort by passed identifiers
            var sortedLogItems = new List<Log>();
            foreach (var id in logIds)
            {
                var log = logItems.Find(x => x.Id == id);
                if (log != null)
                    sortedLogItems.Add(log);
            }

            return sortedLogItems;
        }

        /// <summary>
        /// Inserts a log item
        /// </summary>
        /// <param name="logLevel">Log level</param>
        /// <param name="shortMessage">The short message</param>
        /// <param name="fullMessage">The full message</param>
        /// <param name="customer">The customer to associate log record with</param>
        /// <returns>A log item</returns>
        public virtual Log InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "", Customer customer = null)
        {
            //check ignore word/phrase list?
            if (IgnoreLog(shortMessage) || IgnoreLog(fullMessage))
                return null;

            var log = new Log
            {
                LogLevel = logLevel,
                ShortMessage = shortMessage,
                FullMessage = fullMessage,
                IpAddress = _webHelper.GetCurrentIpAddress(),
                Customer = customer,
                PageUrl = _webHelper.GetThisPageUrl(true),
                ReferrerUrl = _webHelper.GetUrlReferrer(),
                CreatedOnUtc = DateTime.UtcNow
            };

            _logRepository.Insert(log);

            return log;
        }

        /// <summary>
        /// Information
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        /// <param name="customer">Customer</param>
        public virtual void Information(string message, Exception exception = null, Customer customer = null)
        {
            //don't log thread abort exception
            if (exception is System.Threading.ThreadAbortException)
                return;

            if (IsEnabled(LogLevel.Information))
                InsertLog(LogLevel.Information, message, exception?.ToString() ?? string.Empty, customer);
        }

        /// <summary>
        /// Warning
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        /// <param name="customer">Customer</param>
        public virtual void Warning(string message, Exception exception = null, Customer customer = null)
        {
            //don't log thread abort exception
            if (exception is System.Threading.ThreadAbortException)
                return;

            if (IsEnabled(LogLevel.Warning))
                InsertLog(LogLevel.Warning, message, exception?.ToString() ?? string.Empty, customer);
        }

        /// <summary>
        /// Error
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        /// <param name="customer">Customer</param>
        public virtual void Error(string message, Exception exception = null, Customer customer = null)
        {
            //don't log thread abort exception
            if (exception is System.Threading.ThreadAbortException)
                return;

            if (IsEnabled(LogLevel.Error))
                InsertLog(LogLevel.Error, message, exception?.ToString() ?? string.Empty, customer);
        }

        #endregion
    }
}