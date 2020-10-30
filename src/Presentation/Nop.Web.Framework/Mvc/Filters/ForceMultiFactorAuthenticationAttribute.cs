using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Services.Authentication.MultiFactor;
using Nop.Services.Common;
using Nop.Services.Customers;

namespace Nop.Web.Framework.Mvc.Filters
{
    /// <summary>
    /// Represents filter attribute that validates force multiFactor authentication
    /// </summary>
    public sealed class ForceMultiFactorAuthenticationAttribute : TypeFilterAttribute
    {

        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        public ForceMultiFactorAuthenticationAttribute() : base(typeof(ForceMultiFactorAuthenticationFilter))
        {
        }

        #endregion

        #region Nested filter

        private class ForceMultiFactorAuthenticationFilter : IActionFilter
        {
            #region Fields

            private readonly ICustomerService _customerService;
            private readonly IGenericAttributeService _genericAttributeService;
            private readonly IMultiFactorAuthenticationPluginManager _multiFactorAuthenticationPluginManager;
            private readonly IUrlHelperFactory _urlHelperFactory;
            private readonly IWorkContext _workContext;
            private readonly MultiFactorAuthenticationSettings _multiFactorAuthenticationSettings;

            #endregion

            #region Ctor

            public ForceMultiFactorAuthenticationFilter(
                ICustomerService customerService,
                IGenericAttributeService genericAttributeService,
                IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager,
                IUrlHelperFactory urlHelperFactory,
                IWorkContext workContext,
                MultiFactorAuthenticationSettings multiFactorAuthenticationSettings)
            {
                _customerService = customerService;
                _genericAttributeService = genericAttributeService;
                _multiFactorAuthenticationPluginManager = multiFactorAuthenticationPluginManager;
                _urlHelperFactory = urlHelperFactory;
                _workContext = workContext;
                _multiFactorAuthenticationSettings = multiFactorAuthenticationSettings;
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

                if (!DataSettingsManager.DatabaseIsInstalled)
                    return;

                //get action and controller names
                var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
                var actionName = actionDescriptor?.ActionName;
                var controllerName = actionDescriptor?.ControllerName;
                var customer = _workContext.CurrentCustomer;

                if (string.IsNullOrEmpty(actionName) || string.IsNullOrEmpty(controllerName) || _customerService.IsGuest(customer))
                    return;

                //don't validate on MultiFactorAuthentication page
                if (!(controllerName.Equals("Customer", StringComparison.InvariantCultureIgnoreCase) &&
                    actionName.Equals("MultiFactorAuthentication", StringComparison.InvariantCultureIgnoreCase)))
                {
                    //check selected provider of MFA
                    if (_multiFactorAuthenticationSettings.ForceMultifactorAuthentication && _multiFactorAuthenticationPluginManager.HasActivePlugins())
                    {
                        var selectedProvider = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.SelectedMultiFactorAuthenticationProviderAttribute);
                        if (string.IsNullOrEmpty(selectedProvider))
                        {
                            //redirect to MultiFactorAuthenticationSettings page if provider is not selected
                            var redirectUrl = _urlHelperFactory.GetUrlHelper(context).RouteUrl("MultiFactorAuthenticationSettings");
                            context.Result = new RedirectResult(redirectUrl);
                        }
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
