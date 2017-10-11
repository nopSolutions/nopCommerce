using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DiagnosticAdapter;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Services.Logging;

namespace Nop.Web.Framework.Diagnostic
{
    /// <summary>
    /// Represents default Nop diagnostic listener
    /// </summary>
    public class NopDiagnosticListener
    {
        /// <summary>
        /// Log 'View was not found' errors
        /// </summary>
        /// <param name="actionContext">Action context</param>
        /// <param name="isMainPage">Whether it's a main page</param>
        /// <param name="result">Action result</param>
        /// <param name="viewName">View name</param>
        /// <param name="searchedLocations">Searched locations</param>
        [DiagnosticName("Microsoft.AspNetCore.Mvc.ViewNotFound")]
        public virtual void OnViewNotFound(ActionContext actionContext, bool isMainPage, IActionResult result, string viewName, IEnumerable<string> searchedLocations)
        {
            //check whether database is installed
            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;

            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var logger = EngineContext.Current.Resolve<ILogger>();

            //log error
            var error = $"The view '{viewName}' was not found. The following locations were searched:{Environment.NewLine}{string.Join(Environment.NewLine, searchedLocations)}";
            logger.Error(error, customer: workContext.CurrentCustomer);
        }
    }
}