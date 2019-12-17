//using Microsoft.AspNet.WebHooks.Diagnostics;
//using Nop.Core;
//using Nop.Core.Data;
//using Nop.Core.Domain.Logging;
//using Nop.Core.Infrastructure;
//using Nop.Plugin.Api.Domain;
//using System;
//using System.IO;
//using System.Web.Http.Tracing;

//namespace Nop.Plugin.Api.WebHooks
//{
//    /// <summary>
//    /// This Logger is injected into the WebHooks classes that use async calls, which are 
//    /// executed in different threads and at the time of execution the HttpContext as well its
//    /// HttpContext.RequestServices may not be avilable. So any calls to EngineContext.Current.Resolve() will throw
//    /// an exception i.e we can't use the nopCommerce ILogger service which tries to resolve the current store etc.
//    /// </summary>
//    public class NopWebHooksLogger : ILogger
//    {
//        private readonly bool _enableLogging;
//        private readonly IRepository<Log> _logRepository; 

//        private static object lockObject = new object();

//        public NopWebHooksLogger(ApiSettings apiSettings, IRepository<Log> logRepository)
//        {
//            _enableLogging = apiSettings.EnableLogging;
//            _logRepository = logRepository;
//        }

//        public void Log(TraceLevel level, string message, Exception ex)
//        {
//            try
//            {
//                if (_enableLogging)
//                {
//                    if (message != null)
//                    {
//                        lock (lockObject)
//                        {
//                            var log = new Log
//                            {
//                                LogLevel = LogLevel.Information,
//                                ShortMessage = message,
//                                FullMessage = ex?.ToString(),
//                                IpAddress = "",
//                                Customer = null,
//                                PageUrl = "",
//                                ReferrerUrl = "",
//                                CreatedOnUtc = DateTime.UtcNow
//                            };

//                            _logRepository.Insert(log);
//                        }
//                    }

//                }
//            }
//            catch (Exception e)
//            {
//            }
//        }
//    }
//}
