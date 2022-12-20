using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Plugin.Payments.CyberSource.Domain;
using Nop.Plugin.Payments.CyberSource.Services;
using Nop.Services.Payments;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Payments.CyberSource.Components
{
    /// <summary>
    /// Represents the view component to add payer authentication forms to payment info page
    /// </summary>
    public class PayerAuthenticationViewComponent : NopViewComponent
    {
        #region Fields

        private readonly CyberSourceSettings _cyberSourceSettings;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public PayerAuthenticationViewComponent(CyberSourceSettings cyberSourceSettings,
            IPaymentPluginManager paymentPluginManager,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _cyberSourceSettings = cyberSourceSettings;
            _paymentPluginManager = paymentPluginManager;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <param name="widgetZone">Widget zone name</param>
        /// <param name="additionalData">Additional data</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the view component result
        /// </returns>
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            if (!widgetZone.Equals(PublicWidgetZones.BodyStartHtmlTagAfter))
                return Content(string.Empty);

            var routeName = HttpContext.GetEndpoint()?.Metadata.GetMetadata<RouteNameMetadata>()?.RouteName;
            if (routeName != CyberSourceDefaults.CheckoutPaymentInfoRouteName && routeName != CyberSourceDefaults.OnePageCheckoutRouteName)
                return Content(string.Empty);

            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            if (!await _paymentPluginManager.IsPluginActiveAsync(CyberSourceDefaults.SystemName, customer, store.Id))
                return Content(string.Empty);

            if (!CyberSourceService.IsConfigured(_cyberSourceSettings))
                return Content(string.Empty);

            if (_cyberSourceSettings.PaymentConnectionMethod != ConnectionMethodType.FlexMicroForm)
                return Content(string.Empty);

            if (!_cyberSourceSettings.PayerAuthenticationEnabled)
                return Content(string.Empty);

            return View("~/Plugins/Payments.CyberSource/Views/PayerAuthentication.cshtml");
        }

        #endregion
    }
}