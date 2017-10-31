using System;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;

namespace Nop.Services.Logging
{
    /// <summary>
    /// Logging extensions
    /// </summary>
    public static class LoggingExtensions
    {
        /// <summary>
        /// Debug
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        /// <param name="customer">Customer</param>
        public static void Debug(this ILogger logger, string message, Exception exception = null, Customer customer = null)
        {
            FilteredLog(logger, LogLevel.Debug, message, exception, customer);
        }

        /// <summary>
        /// Information
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        /// <param name="customer">Customer</param>
        public static void Information(this ILogger logger, string message, Exception exception = null, Customer customer = null)
        {
            FilteredLog(logger, LogLevel.Information, message, exception, customer);
        }

        /// <summary>
        /// Warning
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        /// <param name="customer">Customer</param>
        public static void Warning(this ILogger logger, string message, Exception exception = null, Customer customer = null)
        {
            FilteredLog(logger, LogLevel.Warning, message, exception, customer);
        }

        /// <summary>
        /// Error
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        /// <param name="customer">Customer</param>
        public static void Error(this ILogger logger, string message, Exception exception = null, Customer customer = null)
        {
            FilteredLog(logger, LogLevel.Error, message, exception, customer);
        }

        /// <summary>
        /// Fatal
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        /// <param name="customer">Customer</param>
        public static void Fatal(this ILogger logger, string message, Exception exception = null, Customer customer = null)
        {
            FilteredLog(logger, LogLevel.Fatal, message, exception, customer);
        }

        /// <summary>
        /// Add a log records
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="level">Level</param>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        /// <param name="customer">Customer</param>
        private static void FilteredLog(ILogger logger, LogLevel level, string message, Exception exception = null, Customer customer = null)
        {
            //don't log thread abort exception
            if (exception is System.Threading.ThreadAbortException)
                return;

            if (logger.IsEnabled(level))
            {
                var fullMessage = exception?.ToString() ?? string.Empty;
                logger.InsertLog(level, message, fullMessage, customer);
            }
        }
    }
}
