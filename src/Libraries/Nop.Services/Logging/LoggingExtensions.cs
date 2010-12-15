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
using System.Collections.Generic;

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
