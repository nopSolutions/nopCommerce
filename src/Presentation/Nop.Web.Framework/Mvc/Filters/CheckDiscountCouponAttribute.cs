using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Messages;

namespace Nop.Web.Framework.Mvc.Filters
{
    /// <summary>
    /// Represents filter attribute that checks and applied discount coupon code to customer
    /// </summary>
    public sealed class CheckDiscountCouponAttribute : TypeFilterAttribute
    {
        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        public CheckDiscountCouponAttribute() : base(typeof(CheckDiscountCouponFilter))
        {
        }

        #endregion

        #region Nested filter

        /// <summary>
        /// Represents a filter that checks and applied discount coupon code to customer
        /// </summary>
        private class CheckDiscountCouponFilter : IActionFilter
        {
            #region Fields

            private readonly ICustomerService _customerService;
            private readonly IDiscountService _discountService;
            private readonly ILocalizationService _localizationService;
            private readonly INotificationService _notificationService;
            private readonly IWorkContext _workContext;

            #endregion

            #region Ctor

            public CheckDiscountCouponFilter(ICustomerService customerService,
                IDiscountService discountService,
                ILocalizationService localizationService,
                INotificationService notificationService,
                IWorkContext workContext)
            {
                _customerService = customerService;
                _discountService = discountService;
                _localizationService = localizationService;
                _notificationService = notificationService;
                _workContext = workContext;
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

                //check request query parameters
                if (!context.HttpContext.Request?.Query?.Any() ?? true)
                    return;

                //only in GET requests
                if (!context.HttpContext.Request.Method.Equals(WebRequestMethods.Http.Get, StringComparison.InvariantCultureIgnoreCase))
                    return;

                if (!DataSettingsManager.DatabaseIsInstalled)
                    return;

                var currentCustomer = _workContext.CurrentCustomer;

                //ignore search engines
                if (currentCustomer.IsSearchEngineAccount())
                    return;

                //try to get discount coupon code
                var queryKey = NopDiscountDefaults.DiscountCouponQueryParameter;
                if (!context.HttpContext.Request.Query.TryGetValue(queryKey, out var couponCodes) || StringValues.IsNullOrEmpty(couponCodes))
                    return;

                //get validated discounts with passed coupon codes
                var discounts = couponCodes
                    .SelectMany(couponCode => _discountService.GetAllDiscounts(couponCode: couponCode))
                    .Distinct()
                    .ToList();

                var validCouponCodes = new List<string>();

                foreach (var discount in discounts)
                {
                    if (!_discountService.ValidateDiscount(discount, currentCustomer, couponCodes.ToArray()).IsValid)
                        continue;
                    
                    //apply discount coupon codes to customer
                    _customerService.ApplyDiscountCouponCode(currentCustomer, discount.CouponCode);
                    validCouponCodes.Add(discount.CouponCode);
                }

                //show notifications for activated coupon codes
                foreach (var validCouponCode in validCouponCodes.Distinct())
                {
                    _notificationService.SuccessNotification(
                        string.Format(_localizationService.GetResource("ShoppingCart.DiscountCouponCode.Activated"),
                            validCouponCode));
                }

                //show notifications for invalid coupon codes
                foreach (var invalidCouponCode in couponCodes.Except(
                    validCouponCodes.Distinct()))
                {
                    _notificationService.WarningNotification(
                        string.Format(_localizationService.GetResource("ShoppingCart.DiscountCouponCode.Invalid"),
                            invalidCouponCode));
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