using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.Logging;
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
            this._ignoreFilter = ignore;
            this.Arguments = new object[] {ignore};
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
        private class AdminAntiForgeryFilter : ValidateAntiforgeryTokenAuthorizationFilter
        {
            #region Fields

            private readonly bool _ignoreFilter;
            private readonly SecuritySettings _securitySettings;

            #endregion

            #region Ctor

            public AdminAntiForgeryFilter(bool ignoreFilter, 
                SecuritySettings securitySettings, 
                IAntiforgery antiforgery,
                ILoggerFactory loggerFactory) : base(antiforgery, loggerFactory)
            {
                this._ignoreFilter = ignoreFilter;
                this._securitySettings = securitySettings;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Check whether the action should be validated
            /// </summary>
            /// <param name="context">Authorization filter context</param>
            /// <returns>True if the action should be validated; otherwise false</returns>
            protected override bool ShouldValidate(AuthorizationFilterContext context)
            {
                if (!base.ShouldValidate(context))
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