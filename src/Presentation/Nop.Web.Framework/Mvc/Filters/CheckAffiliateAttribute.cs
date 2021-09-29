using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Services.Affiliates;
using Nop.Services.Customers;

namespace Nop.Web.Framework.Mvc.Filters
{
    /// <summary>
    /// Represents filter attribute that checks and updates affiliate of customer
    /// </summary>
    public sealed class CheckAffiliateAttribute : TypeFilterAttribute
    {
        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        public CheckAffiliateAttribute() : base(typeof(CheckAffiliateFilter))
        {
        }

        #endregion

        #region Nested filter

        /// <summary>
        /// Represents a filter that checks and updates affiliate of customer
        /// </summary>
        private class CheckAffiliateFilter : IAsyncActionFilter
        {
            #region Constants

            private const string AFFILIATE_ID_QUERY_PARAMETER_NAME = "affiliateid";
            private const string AFFILIATE_FRIENDLYURLNAME_QUERY_PARAMETER_NAME = "affiliate";

            #endregion

            #region Fields

            private readonly IAffiliateService _affiliateService;
            private readonly ICustomerService _customerService;
            private readonly IWorkContext _workContext;

            #endregion

            #region Ctor

            public CheckAffiliateFilter(IAffiliateService affiliateService,
                ICustomerService customerService,
                IWorkContext workContext)
            {
                _affiliateService = affiliateService;
                _customerService = customerService;
                _workContext = workContext;
            }

            #endregion

            #region Utilities

            /// <summary>
            /// Set the affiliate identifier of current customer
            /// </summary>
            /// <param name="affiliate">Affiliate</param>
            /// <param name="customer">Customer</param>
            /// <returns>A task that represents the asynchronous operation</returns>
            private async Task SetCustomerAffiliateIdAsync(Affiliate affiliate, Customer customer)
            {
                if (affiliate == null || affiliate.Deleted || !affiliate.Active)
                    return;

                if (affiliate.Id == customer.AffiliateId)
                    return;

                //ignore search engines
                if (customer.IsSearchEngineAccount())
                    return;

                //update affiliate identifier
                customer.AffiliateId = affiliate.Id;
                await _customerService.UpdateCustomerAsync(customer);
            }

            /// <summary>
            /// Called asynchronously before the action, after model binding is complete.
            /// </summary>
            /// <param name="context">A context for action filters</param>
            /// <returns>A task that represents the asynchronous operation</returns>
            private async Task CheckAffiliateAsync(ActionExecutingContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                //check request query parameters
                var request = context.HttpContext.Request;
                if (request?.Query == null || !request.Query.Any())
                    return;

                if (!DataSettingsManager.IsDatabaseInstalled())
                    return;

                //try to find by ID
                var customer = await _workContext.GetCurrentCustomerAsync();
                var affiliateIds = request.Query[AFFILIATE_ID_QUERY_PARAMETER_NAME];

                if (int.TryParse(affiliateIds.FirstOrDefault(), out var affiliateId) && affiliateId > 0 && affiliateId != customer.AffiliateId)
                {
                    var affiliate = await _affiliateService.GetAffiliateByIdAsync(affiliateId);
                    await SetCustomerAffiliateIdAsync(affiliate, customer);
                    return;
                }

                //try to find by friendly name
                var affiliateNames = request.Query[AFFILIATE_FRIENDLYURLNAME_QUERY_PARAMETER_NAME];
                var affiliateName = affiliateNames.FirstOrDefault();
                if (!string.IsNullOrEmpty(affiliateName))
                {
                    var affiliate = await _affiliateService.GetAffiliateByFriendlyUrlNameAsync(affiliateName);
                    await SetCustomerAffiliateIdAsync(affiliate, customer);
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
                await CheckAffiliateAsync(context);
                if (context.Result == null)
                    await next();
            }

            #endregion
        }

        #endregion
    }
}