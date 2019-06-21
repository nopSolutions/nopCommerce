using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Extensions;
using Nop.Services.Logging.Events;

namespace Nop.Services.Logging
{
    /// <summary>
    /// Default logger
    /// </summary>
    public partial class DefaultLogger : ILogger
    {
        private readonly CommonSettings _commonSettings;
        private readonly ILogger<DefaultLogger> _systemLog;

        public DefaultLogger(CommonSettings commonSettings,
            ILogger<DefaultLogger> systemLog)
        {
            _commonSettings = commonSettings;
            _systemLog = systemLog;
        }

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

        public virtual void DeleteLog(Log log)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            var logRepository = EngineContext.Current.Resolve<IRepository<Log>>();
            logRepository.Delete(log);
        }

        public virtual void DeleteLogs(IList<Log> logs)
        {
            if (logs == null)
                throw new ArgumentNullException(nameof(logs));

            var logRepository = EngineContext.Current.Resolve<IRepository<Log>>();
            logRepository.Delete(logs);
        }

        public virtual void ClearLog()
        {
            //do all databases support "Truncate command"?
            var dbContext = EngineContext.Current.Resolve<IDbContext>();

            var logTableName = dbContext.GetTableName<Log>();
            dbContext.ExecuteSqlCommand($"TRUNCATE TABLE [{logTableName}]");
        }

        public virtual IPagedList<Log> GetAllLogs(DateTime? fromUtc = null, DateTime? toUtc = null,
            string message = "", Core.Domain.Logging.LogLevel? logLevel = null,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var logRepository = EngineContext.Current.Resolve<IRepository<Log>>();

            var query = logRepository.Table;
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

        public virtual Log GetLogById(int logId)
        {
            if (logId == 0)
                return null;

            var logRepository = EngineContext.Current.Resolve<IRepository<Log>>();
            return logRepository.GetById(logId);
        }

        public virtual IList<Log> GetLogByIds(int[] logIds)
        {
            if (logIds == null || logIds.Length == 0)
                return new List<Log>();

            var logRepository = EngineContext.Current.Resolve<IRepository<Log>>();

            var query = from l in logRepository.Table
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

        public virtual Log InsertLog(Core.Domain.Logging.LogLevel logLevel, string shortMessage, string fullMessage = "", Customer customer = null)
        {
            switch (logLevel)
            {
                case Core.Domain.Logging.LogLevel.Debug:
                    _systemLog.LogDebug(LoggingEvents.DefaultLogger, shortMessage);
                    break;
                case Core.Domain.Logging.LogLevel.Error:
                    _systemLog.LogError(LoggingEvents.DefaultLogger, shortMessage);
                    break;
                case Core.Domain.Logging.LogLevel.Fatal:
                    _systemLog.LogCritical(LoggingEvents.DefaultLogger, shortMessage);
                    break;
                case Core.Domain.Logging.LogLevel.Information:
                    _systemLog.LogInformation(LoggingEvents.DefaultLogger, shortMessage);
                    break;
                case Core.Domain.Logging.LogLevel.Warning:
                    _systemLog.LogWarning(LoggingEvents.DefaultLogger, shortMessage);
                    break;
                default:
                    _systemLog.LogInformation(LoggingEvents.DefaultLogger, shortMessage);
                    break;
            }
            var log = new Log
            {
                LogLevel = logLevel,
                ShortMessage = shortMessage,
                FullMessage = fullMessage,
                //IpAddress = _webHelper.GetCurrentIpAddress(),
                Customer = customer,
                //PageUrl = _webHelper.GetThisPageUrl(true),
                //ReferrerUrl = _webHelper.GetUrlReferrer(),
                CreatedOnUtc = DateTime.UtcNow
            };

            //_logRepository.Insert(log);

            return log;
        }

        public virtual void Information(string message, Exception exception = null, Customer customer = null)
        {
            //don't log thread abort exception
            if (exception is System.Threading.ThreadAbortException)
                return;

            InsertLog(Core.Domain.Logging.LogLevel.Information, message, exception?.ToString() ?? string.Empty, customer);
        }

        public virtual void Warning(string message, Exception exception = null, Customer customer = null)
        {
            //don't log thread abort exception
            if (exception is System.Threading.ThreadAbortException)
                return;

            InsertLog(Core.Domain.Logging.LogLevel.Warning, message, exception?.ToString() ?? string.Empty, customer);
        }

        public virtual void Error(string message, Exception exception = null, Customer customer = null)
        {
            //don't log thread abort exception
            if (exception is System.Threading.ThreadAbortException)
                return;

            InsertLog(Core.Domain.Logging.LogLevel.Error, message, exception?.ToString() ?? string.Empty, customer);
        }

        public bool IsEnabled(Core.Domain.Logging.LogLevel level)
        {
            return true;
        }
    }
}