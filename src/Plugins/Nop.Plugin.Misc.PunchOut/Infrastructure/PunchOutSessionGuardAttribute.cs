using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Core.Http;
using Nop.Plugin.Misc.PunchOut.Services;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.UI;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.ShoppingCart;

namespace Nop.Plugin.Misc.PunchOut.Infrastructure;

/// <summary>
/// Represents filter attribute to check if PunchOut session is expired and restrict access to certain controllers and actions during active PunchOut session
/// </summary>
public class PunchOutSessionGuardAttribute : TypeFilterAttribute
{
    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute
    /// </summary>
    public PunchOutSessionGuardAttribute() : base(typeof(PunchOutSessionGuardFilter))
    {
    }

    #endregion

    #region Nested filter

    /// <summary>
    /// Represents filter to check if PunchOut session is expired and restrict access to certain controllers and actions during active PunchOut session
    /// </summary>
    private class PunchOutSessionGuardFilter : IAsyncActionFilter, IAsyncResultFilter
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly INopHtmlHelper _nopHtmlHelper;
        private readonly INotificationService _notificationService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly PunchOutService _punchOutService;
        private readonly PunchOutSettings _punchOutSettings;

        /// <summary>
        /// Controllers that are allowed during active PunchOut session
        /// </summary>
        private static readonly HashSet<string> _allowedControllers = new(StringComparer.OrdinalIgnoreCase)
        {
            "Home",
            "Catalog",
            "Product",
            "PunchOut",
            "ShoppingCart",
            "Error",
            "Common",
            "Customer"
        };

        /// <summary>
        /// Specific actions that are forbidden during active PunchOut session
        /// Key: Controller name, Value: Set of forbidden action names
        /// </summary>
        private static readonly Dictionary<string, HashSet<string>> _forbiddenActions = new(StringComparer.OrdinalIgnoreCase)
        {
            {
                "ShoppingCart",
                new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "StartCheckout",
                    "Checkout"
                }
            },
            {
                "Common",
                new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "ContactUs",
                    "ContactVendor"
                }
            },
            {
                "Product",
                new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "ProductReviews",
                    "SetProductReviewHelpfulness",
                    "CustomerProductReviews"
                }
            },
            {
                "Customer",
                new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "Info",
                    "Register",
                    "PasswordRecovery",
                    "Addresses",
                    "DownloadableProducts",
                    "Avatar",
                    "GdprTools",
                    "CheckGiftCardBalance",
                    "MultiFactorAuthentication"
                }
            }
        };

        #endregion

        #region Ctor

        public PunchOutSessionGuardFilter(ILocalizationService localizationService,
            INopHtmlHelper nopHtmlHelper,
            INotificationService notificationService,
            IWebHelper webHelper,
            IWorkContext workContext,
            PunchOutService punchOutService,
            PunchOutSettings punchOutSettings)
        {
            _localizationService = localizationService;
            _nopHtmlHelper = nopHtmlHelper;
            _notificationService = notificationService;
            _webHelper = webHelper;
            _workContext = workContext;
            _punchOutService = punchOutService;
            _punchOutSettings = punchOutSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Checks if there is an active PunchOut session and restricts access to certain controllers and actions if the session is active
        /// </summary>
        /// <param name="context">The action executing context</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task IsActivePunchoutSessionAsync(ActionExecutingContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (!_punchOutSettings.IsActive)
                return;

            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            var actionName = actionDescriptor?.ActionName;
            var controllerName = actionDescriptor?.ControllerName;
            if (string.IsNullOrEmpty(actionName) || string.IsNullOrEmpty(controllerName))
                return;

            //ignore AJAX requests
            if (_webHelper.IsAjaxRequest(context.HttpContext.Request))
                return;

            //ignore search engines and background tasks
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (customer.IsSystemAccount)
                return;

            //ignore admin area requests
            if (context.RouteData.Values.TryGetValue("area", out var area) &&
                area is string areaStr &&
                areaStr.Equals(AreaNames.ADMIN, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var session = await _punchOutService.GetPunchOutSessionAsync();
            if (session != null && session.IsActive)
            {
                var timeToExpire = _punchOutSettings.TimeToExpire;
                if (session.CreatedOnUtc.AddHours(timeToExpire) < DateTime.UtcNow)
                {
                    await _punchOutService.ClearPunchoutSessionDataAsync();

                    _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Plugins.Misc.PunchOut.SessionExpired"));
                    context.Result = new RedirectToRouteResult(NopRouteNames.General.HOMEPAGE, null);
                }
                else if (!_allowedControllers.Contains(controllerName))
                {
                    //if PunchOut session is active, restrict access to all controllers except those in AllowedControllers
                    context.Result = new RedirectToRouteResult(NopRouteNames.General.HOMEPAGE, null);
                }
                else if (IsForbiddenAction(controllerName, actionName))
                {
                    //if specific action is forbidden during active PunchOut session, restrict access
                    context.Result = new RedirectToRouteResult(NopRouteNames.General.HOMEPAGE, null);
                }
            }
        }

        /// <summary>
        /// Checks if the action is forbidden during active PunchOut session
        /// </summary>
        /// <param name="controllerName">The name of the controller</param>
        /// <param name="actionName">The name of the action</param>
        /// <returns>True if the action is forbidden; otherwise false</returns>
        private static bool IsForbiddenAction(string controllerName, string actionName)
        {
            return _forbiddenActions.TryGetValue(controllerName, out var forbiddenActions) && forbiddenActions.Contains(actionName);
        }

        /// <summary>
        /// Replaces the view for the Cart action with a custom PunchOut view if PunchOut session is active
        /// </summary>
        /// <param name="context">The result executing context</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task ReplaceCartViewIfPunchOutActiveAsync(ResultExecutingContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (!_punchOutSettings.IsActive)
                return;

            //ignore AJAX requests
            if (_webHelper.IsAjaxRequest(context.HttpContext.Request))
                return;

            //ignore search engines and background tasks
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (customer.IsSystemAccount)
                return;

            //ignore admin area requests
            if (context.RouteData.Values.TryGetValue("area", out var area) &&
                area is string areaStr &&
                areaStr.Equals(AreaNames.ADMIN, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var session = await _punchOutService.GetPunchOutSessionAsync();
            if (session != null && session.IsActive)
            {
                var routeName = _nopHtmlHelper.GetRouteName();

                // Only replace view for Cart action when PunchOut is active
                if (routeName is NopRouteNames.General.CART && context.Result is ViewResult viewResult)
                {
                    if (viewResult.ViewData.Model is ShoppingCartModel model)
                        model.ShowItemDiscount = false;

                    // Replace the view name while keeping the model
                    viewResult.ViewName = "~/Plugins/Misc.PunchOut/Views/PunchOutCart.cshtml";
                }

                if (routeName is "ProductDetails" && context.Result is ViewResult productDetailsViewResult)
                {
                    if (productDetailsViewResult.ViewData.Model is ProductDetailsModel model)
                    {
                        model.ProductReviews.AddProductReview.CanCurrentCustomerLeaveReview = false;
                        model.ProductReviews.AddProductReview.CanAddNewReview = false;

                        model.ProductReviewOverview.CanAddNewReview = false;
                        model.ProductReviewOverview.CanCurrentCustomerLeaveReview = false;
                    }
                }
            }
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
            await IsActivePunchoutSessionAsync(context);

            if (context.Result == null)
                await next();
        }

        /// <summary>
        /// Called asynchronously before the result execution.
        /// </summary>
        /// <param name="context">A context for result filters</param>
        /// <param name="next">A delegate invoked to execute the next result filter or the result</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            await ReplaceCartViewIfPunchOutActiveAsync(context);
            await next();
        }

        #endregion
    }

    #endregion
}
