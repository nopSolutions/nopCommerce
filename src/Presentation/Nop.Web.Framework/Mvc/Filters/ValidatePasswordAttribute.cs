using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Data;
using Nop.Services.Customers;

namespace Nop.Web.Framework.Mvc.Filters
{
    /// <summary>
    /// Represents filter attribute that validates customer password expiration
    /// </summary>
    public class ValidatePasswordAttribute : TypeFilterAttribute
    {
        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        public ValidatePasswordAttribute() : base(typeof(ValidatePasswordFilter))
        {
        }

        #endregion

        #region Nested filter

        /// <summary>
        /// Represents a filter that validates customer password expiration
        /// </summary>
        private class ValidatePasswordFilter : IActionFilter
        {
            #region Fields

            private readonly IUrlHelperFactory _urlHelperFactory;
            private readonly IWorkContext _workContext;

            #endregion

            #region Ctor

            public ValidatePasswordFilter(IUrlHelperFactory urlHelperFactory, 
                IWorkContext workContext)
            {
                this._urlHelperFactory = urlHelperFactory;
                this._workContext = workContext;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Called before the action executes, after model binding is complete
            /// </summary>
            /// <param name="context">A context for action filters</param>
            public void OnActionExecuting(ActionExecutingContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                if (context.HttpContext.Request == null)
                    return;

                if (!DataSettingsHelper.DatabaseIsInstalled())
                    return;

                //get action and controller names
                var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
                var actionName = actionDescriptor?.ActionName;
                var controllerName = actionDescriptor?.ControllerName;

                if (string.IsNullOrEmpty(actionName) || string.IsNullOrEmpty(controllerName))
                    return;
                
                //don't validate on ChangePassword page
                if (!(controllerName.Equals("Customer", StringComparison.InvariantCultureIgnoreCase) &&
                    actionName.Equals("ChangePassword", StringComparison.InvariantCultureIgnoreCase)))
                {
                    //check password expiration
                    if (_workContext.CurrentCustomer.PasswordIsExpired())
                    {
                        //redirect to ChangePassword page if expires
                        var changePasswordUrl = _urlHelperFactory.GetUrlHelper(context).RouteUrl("CustomerChangePassword");
                        context.Result = new RedirectResult(changePasswordUrl);
                    }
                }
            }

            /// <summary>
            /// Called after the action executes, before the action result
            /// </summary>
            /// <param name="context">A context for action filters</param>
            public void OnActionExecuted(ActionExecutedContext context)
            {
                //do nothing
            }

            #endregion
        }

        #endregion
    }
}