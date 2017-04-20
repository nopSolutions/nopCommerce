using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Services.Security;

namespace Nop.Web.Framework.Controllers
{
    /// <summary>
    /// Represents a filter attribute that confirms access to the admin panel
    /// </summary>
    public class AdminAuthorizeAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        /// <param name="ignore">Whether to ignore the execution of filter actions</param>
        public AdminAuthorizeAttribute(bool ignore = false) : base(typeof(AuthorizeAdminFilter))
        {
            this.Arguments = new object[] { ignore };
        }

        #region Nested filter

        /// <summary>
        /// Represents a filter that confirms access to the admin panel
        /// </summary>
        private class AuthorizeAdminFilter : IAuthorizationFilter
        {
            #region Fields

            private readonly bool _ignoreFilter;
            private readonly IPermissionService _permissionService;

            #endregion

            #region Ctor

            public AuthorizeAdminFilter(bool ignoreFilter, IPermissionService permissionService)
            {
                this._ignoreFilter = ignoreFilter;
                this._permissionService = permissionService;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Called early in the filter pipeline to confirm request is authorized
            /// </summary>
            /// <param name="filterContext">Authorization filter context</param>
            public void OnAuthorization(AuthorizationFilterContext filterContext)
            {
                //ignore filter actions
                if (_ignoreFilter)
                    return;

                if (filterContext == null)
                    throw new ArgumentNullException("filterContext");
#if NET451
                if (OutputCacheAttribute.IsChildActionCacheActive(filterContext))
                    throw new InvalidOperationException("You cannot use [AdminAuthorize] attribute when a child action cache is active");
#endif
                //there is AdminAuthorizeFilter, so check access
                if (filterContext.Filters.Any(filter => filter is AuthorizeAdminFilter))
                {
                    //authorize permission of access to the admin area
                    if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                        filterContext.Result = new UnauthorizedResult();
                }
            }

            #endregion
        }

        #endregion
    }
}