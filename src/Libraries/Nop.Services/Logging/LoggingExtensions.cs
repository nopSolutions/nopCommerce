
using System;
using Nop.Core.Domain.Logging;

namespace Nop.Services.Logging
{
    public static class LoggingExtensions
    {
        public static void Debug(this ILogger logger, string message)
        {
            FilteredLog(logger, LogLevel.Debug, message, null);
        }
        public static void Information(this ILogger logger, string message)
        {
            FilteredLog(logger, LogLevel.Information, message, null);
        }
        public static void Warning(this ILogger logger, string message)
        {
            FilteredLog(logger, LogLevel.Warning, message, null);
        }
        public static void Error(this ILogger logger, string message)
        {
            FilteredLog(logger, LogLevel.Error, message, null);
        }
        public static void Fatal(this ILogger logger, string message)
        {
            FilteredLog(logger, LogLevel.Fatal, message, null);
        }

        public static void Debug(this ILogger logger, string message, Exception exception)
        {
            FilteredLog(logger, LogLevel.Debug, message, exception);
        }
        public static void Information(this ILogger logger, string message, Exception exception)
        {
            FilteredLog(logger, LogLevel.Information, message, exception);
        }
        public static void Warning(this ILogger logger, string message, Exception exception)
        {
            FilteredLog(logger, LogLevel.Warning, message, exception);
        }
        public static void Error(this ILogger logger,  string message, Exception exception)
        {
            FilteredLog(logger, LogLevel.Error, message, exception);
        }
        public static void Fatal(this ILogger logger,  string message, Exception exception)
        {
            FilteredLog(logger, LogLevel.Fatal, message, exception);
        }

        private static void FilteredLog(ILogger logger, LogLevel level, string message, Exception exception)
        {
            if (logger.IsEnabled(level))
            {
                logger.InsertLog(level, message, exception);
            }
        }
    }
}
