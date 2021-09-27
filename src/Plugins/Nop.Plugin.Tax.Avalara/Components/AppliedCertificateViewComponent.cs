using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Tax.Avalara.Services;
using Nop.Services.Customers;
using Nop.Services.Tax;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Models.ShoppingCart;

namespace Nop.Plugin.Tax.Avalara.Components
{
    /// <summary>
    /// Represents a view component to display applied exemption certificate on the order confirmation page
    /// </summary>
    [ViewComponent(Name = AvalaraTaxDefaults.APPLIED_CERTIFICATE_VIEW_COMPONENT_NAME)]
    public class AppliedCertificateViewComponent : NopViewComponent
    {
        #region Fields

        private readonly AvalaraTaxManager _avalaraTaxManager;
        private readonly AvalaraTaxSettings _avalaraTaxSettings;
        private readonly ICustomerService _customerService;
        private readonly IStoreContext _storeContext;
        private readonly ITaxPluginManager _taxPluginManager;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public AppliedCertificateViewComponent(AvalaraTaxManager avalaraTaxManager,
            AvalaraTaxSettings avalaraTaxSettings,
            ICustomerService customerService,
            IStoreContext storeContext,
            ITaxPluginManager taxPluginManager,
            IWorkContext workContext)
        {
            _avalaraTaxManager = avalaraTaxManager;
            _avalaraTaxSettings = avalaraTaxSettings;
            _customerService = customerService;
            _storeContext = storeContext;
            _taxPluginManager = taxPluginManager;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke the widget view component
        /// </summary>
        /// <param name="widgetZone">Widget zone</param>
        /// <param name="additionalData">Additional parameters</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the view component result
        /// </returns>
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            //ensure that Avalara tax provider is active
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName, customer))
                return Content(string.Empty);

            if (!_avalaraTaxSettings.EnableCertificates)
                return Content(string.Empty);

            //ACL
            if (_avalaraTaxSettings.CustomerRoleIds.Any())
            {
                var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
                if (!customerRoleIds.Intersect(_avalaraTaxSettings.CustomerRoleIds).Any())
                    return Content(string.Empty);
            }

            //ensure that it's a proper widget zone
            if (!widgetZone.Equals(PublicWidgetZones.OrderSummaryContentBefore))
                return Content(string.Empty);

            //ensure that model is passed
            if (additionalData is not ShoppingCartModel cartModel || cartModel.OrderReviewData?.Display != true)
                return Content(string.Empty);

            var store = await _storeContext.GetCurrentStoreAsync();
            var validCertificate = await _avalaraTaxManager.GetValidCertificatesAsync(customer, store.Id);
            var certificateValue = !string.IsNullOrEmpty(validCertificate?.exemptionNumber)
                ? validCertificate.exemptionNumber
                : validCertificate?.id?.ToString();

            return View("~/Plugins/Tax.Avalara/Views/Checkout/AppliedCertificate.cshtml", certificateValue);
        }

        #endregion
    }
}