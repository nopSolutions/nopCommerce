using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;

namespace Nop.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents filter attribute that validates customer password expiration
/// </summary>
public sealed class ValidatePasswordAttribute : TypeFilterAttribute
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
    private class ValidatePasswordFilter : IAsyncActionFilter
    {
        #region Fields

        protected readonly ICustomerService _customerService;
        protected readonly IGenericAttributeService _genericAttributeService;
        protected readonly ILocalizationService _localizationService;
        protected readonly INotificationService _notificationService;
        protected readonly IWebHelper _webHelper;
        protected readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ValidatePasswordFilter(ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IWebHelper webHelper,
            IWorkContext workContext)
        {
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _webHelper = webHelper;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Called asynchronously before the action, after model binding is complete.
        /// </summary>
        /// <param name="context">A context for action filters</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task ValidatePasswordAsync(ActionExecutingContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.HttpContext.Request == null)
                return;

            //ignore AJAX requests
            if (_webHelper.IsAjaxRequest(context.HttpContext.Request))
                return;

            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //get action and controller names
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            var actionName = actionDescriptor?.ActionName;
            var controllerName = actionDescriptor?.ControllerName;

            if (string.IsNullOrEmpty(actionName) || string.IsNullOrEmpty(controllerName))
                return;

            //don't validate on 'Change Password' & 'Logout' pages
            if (controllerName.Equals("Customer", StringComparison.InvariantCultureIgnoreCase) &&
                new[] { "ChangePassword", "Logout" }.Contains(actionName, StringComparer.InvariantCultureIgnoreCase))
                return;

            var customer = await _workContext.GetCurrentCustomerAsync();

            if (customer.MustChangePasswordAtNextLogin)
                _notificationService.WarningNotification(await _localizationService.GetResourceAsync("Account.ChangePassword.MustBeChanged"));

            //check password expiration
            if (!await _customerService.IsPasswordExpiredAsync(customer) && 
                (!customer.MustChangePasswordAtNextLogin ||
                !await _genericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.PasswordMustBeChangedAttribute)))
                return;

            var returnUrl = _webHelper.GetRawUrl(context.HttpContext.Request);
            //redirect to ChangePassword page if expires
            context.Result = new RedirectToRouteResult("CustomerChangePassword", new { returnUrl = returnUrl });
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
            await ValidatePasswordAsync(context);
            if (context.Result == null)
                await next();
        }

        #endregion
    }

    #endregion
}