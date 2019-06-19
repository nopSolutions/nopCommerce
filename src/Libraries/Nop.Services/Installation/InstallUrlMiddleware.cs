using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Data;

namespace Nop.Services.Installation
{
    /// <summary>
    /// Represents middleware that checks whether database is installed and redirects to installation URL in otherwise
    /// </summary>
    public class InstallUrlMiddleware
    {
        private readonly RequestDelegate _next;

        public InstallUrlMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Invoke middleware actions
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <param name="webHelper">Web helper</param>
        /// <returns>Task</returns>
        public async Task Invoke(HttpContext context, IWebHelper webHelper)
        {
            //whether database is installed
            if (!DataSettingsManager.GetDatabaseIsInstalled())
            {
                var installUrl = $"{webHelper.GetStoreLocation()}{NopInstallationDefaults.InstallPath}";
                if (!webHelper.GetThisPageUrl(false).StartsWith(installUrl, StringComparison.InvariantCultureIgnoreCase))
                {
                    //redirect
                    context.Response.Redirect(installUrl);
                    return;
                }
            }

            //or call the next middleware in the request pipeline
            await _next(context);
        }
    }
}