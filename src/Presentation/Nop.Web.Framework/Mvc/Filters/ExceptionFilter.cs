using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Services.Logging;

namespace Nop.Web.Framework.Mvc.Filters
{
    /// <summary>
    /// Represents a filter that runs after an action has thrown an exception
    /// </summary>
    public class ExceptionFilter : IExceptionFilter
    {
        /// <summary>
        /// Called after an action has thrown an exception
        /// </summary>
        /// <param name="context">Exception context</param>
        public void OnException(ExceptionContext context)
        {
            if (context?.Exception == null)
                return;

            //just log an exception here, handle it later
            context.ExceptionHandled = false;

            //check whether database is already installed
            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;

            try
            {
                //get logger
                var logger = EngineContext.Current.Resolve<ILogger>();

                //get current customer
                var currentCustomer = EngineContext.Current.Resolve<IWorkContext>().CurrentCustomer;

                //log exception
                logger.Error(context.Exception.Message, context.Exception, currentCustomer);
            }
            catch (Exception)
            {
                //don't throw new exception if occurs
            }
        }
    }
}
