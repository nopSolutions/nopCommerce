using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Services.Authentication.MultiFactor;
using Nop.Services.Common;
using Nop.Services.Security;

namespace Nop.Web.Framework.Mvc.Filters
{
    /// <summary>
    /// Represents filter attribute that validates force of the multi-factor authentication
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

        private class ForceMultiFactorAuthenticationFilter : IAsyncActionFilter
        {
            #region Fields

            protected readonly IGenericAttributeService _genericAttributeService;
            protected readonly IMultiFactorAuthenticationPluginManager _multiFactorAuthenticationPluginManager;
            protected readonly IPermissionService _permissionService;
            protected readonly IWorkContext _workContext;
            protected readonly MultiFactorAuthenticationSettings _multiFactorAuthenticationSettings;

            #endregion

            #region Ctor

            public ForceMultiFactorAuthenticationFilter(IGenericAttributeService genericAttributeService,
                IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager,
                IPermissionService permissionService,
                IWorkContext workContext,
                MultiFactorAuthenticationSettings multiFactorAuthenticationSettings)
            {
                _genericAttributeService = genericAttributeService;
                _multiFactorAuthenticationPluginManager = multiFactorAuthenticationPluginManager;
                _permissionService = permissionService;
                _workContext = workContext;
                _multiFactorAuthenticationSettings = multiFactorAuthenticationSettings;
            }

            #endregion

            #region Utilities

            /// <summary>
            /// Called asynchronously before the action, after model binding is complete.
            /// </summary>
            /// <param name="context">A context for action filters</param>
            /// <returns>A task that represents the asynchronous operation</returns>
            private async Task ValidateAuthenticationForceAsync(ActionExecutingContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                if (context.HttpContext.Request == null)
                    return;

                if (!DataSettingsManager.IsDatabaseInstalled())
                    return;

                //whether the feature is enabled
                if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableMultiFactorAuthentication))
                    return;

                //don't validate on the 'Multi-factor authentication settings' page
                var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
                var actionName = actionDescriptor?.ActionName;
                var controllerName = actionDescriptor?.ControllerName;
                if (string.IsNullOrEmpty(actionName) || string.IsNullOrEmpty(controllerName))
                    return;

                if (controllerName.Equals("Customer", StringComparison.InvariantCultureIgnoreCase) &&
                    actionName.Equals("MultiFactorAuthentication", StringComparison.InvariantCultureIgnoreCase))
                {
                    return;
                }

                //whether multi-factor authentication is enforced
                if (!_multiFactorAuthenticationSettings.ForceMultifactorAuthentication ||
                    !await _multiFactorAuthenticationPluginManager.HasActivePluginsAsync())
                {
                    return;
                }

                //check selected provider of MFA
                var customer = await _workContext.GetCurrentCustomerAsync();
                var selectedProvider = await _genericAttributeService
                    .GetAttributeAsync<string>(customer, NopCustomerDefaults.SelectedMultiFactorAuthenticationProviderAttribute);
                if (!string.IsNullOrEmpty(selectedProvider))
                    return;

                //redirect to MultiFactorAuthenticationSettings page if force is enabled
                context.Result = new RedirectToRouteResult("MultiFactorAuthenticationSettings", null);
            }

            #endregion

            #region Methods

            /// <summary>
            /// Called asynchronously before the action, after model binding is complete.
            /// </summary>
            /// <param name="context">A context for action filters</param>
            /// <param name="next">A delegate invoked to execute the next action filter or the action itself</param>
            /// <returns>A task that represents the asynchronous operation</returns>
            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                await ValidateAuthenticationForceAsync(context);
                if (context.Result == null)
                    await next();
            }

            #endregion
        }

        #endregion
    }
}
