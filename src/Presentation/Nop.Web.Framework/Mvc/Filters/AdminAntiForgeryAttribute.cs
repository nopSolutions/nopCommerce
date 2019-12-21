using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Nop.Core.Domain.Security;

namespace Nop.Web.Framework.Mvc.Filters
{
    /// <summary>
    /// Represents a filter attribute that enables anti-forgery feature for the admin area
    /// </summary>
    public class AdminAntiForgeryAttribute : TypeFilterAttribute
    {
        #region Fields

        private readonly bool _ignoreFilter;

        #endregion

        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        /// <param name="ignore">Whether to ignore the execution of filter actions</param>
        public AdminAntiForgeryAttribute(bool ignore = false) : base(typeof(AdminAntiForgeryFilter))
        {
            _ignoreFilter = ignore;
            Arguments = new object[] {ignore};
        }
        
        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether to ignore the execution of filter actions
        /// </summary>
        public bool IgnoreFilter => _ignoreFilter;

        #endregion

        #region Nested filter

        /// <summary>
        /// Represents a filter that enables anti-forgery feature for the admin area
        /// </summary>
        private class AdminAntiForgeryFilter : IAsyncAuthorizationFilter, IAntiforgeryPolicy
        {
            #region Fields

            private readonly bool _ignoreFilter;
            private readonly SecuritySettings _securitySettings;
            private readonly IAntiforgery _antiforgery;

            #endregion

            #region Ctor

            public AdminAntiForgeryFilter(bool ignoreFilter, 
                SecuritySettings securitySettings, 
                IAntiforgery antiforgery)
            {
                _ignoreFilter = ignoreFilter;
                _securitySettings = securitySettings;
                _antiforgery = antiforgery;
            }

            #endregion

            #region Methods

            public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                if (!context.IsEffectivePolicy<IAntiforgeryPolicy>(this))
                {
                    return;
                }

                if (ShouldValidate(context))
                {
                    try
                    {
                        await _antiforgery.ValidateRequestAsync(context.HttpContext);
                    }
                    catch
                    {
                        context.Result = new AntiforgeryValidationFailedResult();
                    }
                }
            }

            /// <summary>
            /// Check whether the action should be validated
            /// </summary>
            /// <param name="context">Authorization filter context</param>
            /// <returns>True if the action should be validated; otherwise false</returns>
            protected virtual bool ShouldValidate(AuthorizationFilterContext context)
            {
                if (context == null)
                    return false;

                if (context.HttpContext.Request == null)
                    return false;

                //ignore GET requests
                if (context.HttpContext.Request.Method.Equals(WebRequestMethods.Http.Get, StringComparison.InvariantCultureIgnoreCase))
                    return false;

                if (!_securitySettings.EnableXsrfProtectionForAdminArea)
                    return false;

                //check whether this filter has been overridden for the Action
                var actionFilter = context.ActionDescriptor.FilterDescriptors
                    .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
                    .Select(filterDescriptor => filterDescriptor.Filter).OfType<AdminAntiForgeryAttribute>().FirstOrDefault();

                //ignore this filter
                if (actionFilter?.IgnoreFilter ?? _ignoreFilter)
                    return false;

                return true;
            }

            #endregion
        }

        #endregion
    }
}