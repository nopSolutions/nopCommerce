using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Services.Discounts;

namespace Nop.Web.Framework.Mvc.Filters
{
    /// <summary>
    /// Represents filter attribute that checks and applied discount coupon code to customer
    /// </summary>
    public class CheckDiscountCouponAttribute : TypeFilterAttribute
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
            private readonly IWorkContext _workContext;

            #endregion

            #region Ctor

            public CheckDiscountCouponFilter(ICustomerService customerService,
                IDiscountService discountService,
                IWorkContext workContext)
            {
                this._customerService = customerService;
                this._discountService = discountService;
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

                //check request query parameters
                if (!context.HttpContext.Request?.Query?.Any() ?? true)
                    return;

                //only in GET requests
                if (!context.HttpContext.Request.Method.Equals(WebRequestMethods.Http.Get, StringComparison.InvariantCultureIgnoreCase))
                    return;

                if (!DataSettingsManager.DatabaseIsInstalled)
                    return;

                //ignore search engines
                if (_workContext.CurrentCustomer.IsSearchEngineAccount())
                    return;

                //try to get discount coupon code
                var queryKey = NopDiscountDefaults.DiscountCouponQueryParameter;
                if (!context.HttpContext.Request.Query.TryGetValue(queryKey, out StringValues couponCodes) || StringValues.IsNullOrEmpty(couponCodes))
                    return;

                //get validated discounts with passed coupon codes
                var discounts = couponCodes
                    .SelectMany(couponCode => _discountService.GetAllDiscountsForCaching(couponCode: couponCode))
                    .Distinct()
                    .Where(discount => _discountService.ValidateDiscount(discount, _workContext.CurrentCustomer, couponCodes.ToArray()).IsValid)
                    .ToList();

                //apply discount coupon codes to customer
                discounts.ForEach(discount => _customerService.ApplyDiscountCouponCode(_workContext.CurrentCustomer, discount.CouponCode));
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