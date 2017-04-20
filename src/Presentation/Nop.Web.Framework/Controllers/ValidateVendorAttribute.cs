using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;

namespace Nop.Web.Framework.Controllers
{
    /// <summary>
    /// Represents a filter attribute confirming that user with "Vendor" customer role has appropriate vendor account associated (and active)
    /// </summary>
    public class ValidateVendorAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        /// <param name="ignore">Whether to ignore the execution of filter actions</param>
        public ValidateVendorAttribute(bool ignore = false) : base(typeof(ValidateVendorFilter))
        {
            this.Arguments = new object[] { ignore };
        }

        #region Nested filter

        /// <summary>
        /// Represents a filter confirming that user with "Vendor" customer role has appropriate vendor account associated (and active)
        /// </summary>
        private class ValidateVendorFilter : IAuthorizationFilter
        {
            #region Fields

            private readonly bool _ignoreFilter;
            private readonly IWorkContext _workContext;

            #endregion

            #region Ctor

            public ValidateVendorFilter(bool ignoreFilter, IWorkContext workContext)
            {
                this._ignoreFilter = ignoreFilter;
                this._workContext = workContext;
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

                if (!DataSettingsHelper.DatabaseIsInstalled())
                    return;

                //whether current customer is vendor
                if (!_workContext.CurrentCustomer.IsVendor())
                    return;

                //ensure that this user has active vendor record associated
                if (_workContext.CurrentVendor == null)
                    filterContext.Result = new UnauthorizedResult();
            }

            #endregion
        }

        #endregion
    }
}