using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Data;

namespace Nop.Web.Framework.Globalization
{
    /// <summary>
    /// Represents middleware that set current culture based on request
    /// </summary>
    public class CultureMiddleware
    {
        #region Fields

        private readonly RequestDelegate _next;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="next">Next</param>
        public CultureMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Set working culture
        /// </summary>
        /// <param name="webHelper">Web helper</param>
        /// <param name="workContext">Work context</param>
        protected void SetWorkingCulture(IWebHelper webHelper, IWorkContext workContext)
        {
            if (!DataSettingsManager.DatabaseIsInstalled)
                return;

            if (webHelper.IsStaticResource())
                return;

            var adminAreaUrl = $"{webHelper.GetStoreLocation()}admin";
            if (webHelper.GetThisPageUrl(false).StartsWith(adminAreaUrl, StringComparison.InvariantCultureIgnoreCase))
            {
                //set work context to admin mode
                workContext.IsAdmin = true;
            }
            
            //set working language culture
            var culture = new CultureInfo(workContext.WorkingLanguage.LanguageCulture);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke middleware actions
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <param name="webHelper">Web helper</param>
        /// <param name="workContext">Work context</param>
        /// <returns>Task</returns>
        public Task Invoke(HttpContext context, IWebHelper webHelper, IWorkContext workContext)
        {
            //set culture
            SetWorkingCulture(webHelper, workContext);

            //call the next middleware in the request pipeline
            return _next(context);
        }
        
        #endregion
    }
}
