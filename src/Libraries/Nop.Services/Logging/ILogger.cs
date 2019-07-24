using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;

namespace Nop.Services.Logging
{
    /// <summary>
    /// Logger interface
    /// </summary>
    public partial interface ILogger
    {
        /// <summary>
        /// Determines whether a log level is enabled
        /// </summary>
        /// <param name="level">Log level</param>
        /// <returns>Result</returns>
        bool IsEnabled(LogLevel level);

        /// <summary>
        /// Deletes a log item
        /// </summary>
        /// <param name="log">Log item</param>
        void DeleteLog(Log log);

        /// <summary>
        /// Deletes a log items
        /// </summary>
        /// <param name="logs">Log items</param>
        void DeleteLogs(IList<Log> logs);

        /// <summary>
        /// Clears a log
        /// </summary>
        void ClearLog();

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
        IPagedList<Log> GetAllLogs(DateTime? fromUtc = null, DateTime? toUtc = null,
            string message = "", LogLevel? logLevel = null,
            int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets a log item
        /// </summary>
        /// <param name="logId">Log item identifier</param>
        /// <returns>Log item</returns>
        Log GetLogById(int logId);

        /// <summary>
        /// Get log items by identifiers
        /// </summary>
        /// <param name="logIds">Log item identifiers</param>
        /// <returns>Log items</returns>
        IList<Log> GetLogByIds(int[] logIds);

        /// <summary>
        /// Inserts a log item
        /// </summary>
        /// <param name="logLevel">Log level</param>
        /// <param name="shortMessage">The short message</param>
        /// <param name="fullMessage">The full message</param>
        /// <param name="customer">The customer to associate log record with</param>
        /// <returns>A log item</returns>
        Log InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "", Customer customer = null);

        /// <summary>
        /// Information
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        /// <param name="customer">Customer</param>
        void Information(string message, Exception exception = null, Customer customer = null);

        /// <summary>
        /// Warning
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        /// <param name="customer">Customer</param>
        void Warning(string message, Exception exception = null, Customer customer = null);

        /// <summary>
        /// Error
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        /// <param name="customer">Customer</param>
        void Error(string message, Exception exception = null, Customer customer = null);
    }
}