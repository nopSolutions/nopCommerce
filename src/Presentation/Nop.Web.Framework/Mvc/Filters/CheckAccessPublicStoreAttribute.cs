using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core.Data;
using Nop.Services.Security;

namespace Nop.Web.Framework.Mvc.Filters
{
    /// <summary>
    /// Represents a filter attribute that confirms access to public store
    /// </summary>
    public class CheckAccessPublicStoreAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        /// <param name="ignore">Whether to ignore the execution of filter actions</param>
        public CheckAccessPublicStoreAttribute(bool ignore = false) : base(typeof(CheckAccessPublicStoreFilter))
        {
            this.Arguments = new object[] { ignore };
        }

        #region Nested filter

        /// <summary>
        /// Represents a filter that confirms access to public store
        /// </summary>
        private class CheckAccessPublicStoreFilter : IAuthorizationFilter
        {
            #region Fields

            private readonly bool _ignoreFilter;
            private readonly IPermissionService _permissionService;

            #endregion

            #region Ctor

            public CheckAccessPublicStoreFilter(bool ignoreFilter, IPermissionService permissionService)
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
                //ignore filter (the action available even when navigation is not allowed)
                if (_ignoreFilter)
                    return;

                if (filterContext == null)
                    throw new ArgumentNullException(nameof(filterContext));

                if (!DataSettingsHelper.DatabaseIsInstalled())
                    return;

                //check whether current customer has access to a public store
                if (_permissionService.Authorize(StandardPermissionProvider.PublicStoreAllowNavigation))
                    return;

                //customer hasn't access to a public store
                filterContext.Result = new UnauthorizedResult();
            }

            #endregion
        }

        #endregion
    }
}