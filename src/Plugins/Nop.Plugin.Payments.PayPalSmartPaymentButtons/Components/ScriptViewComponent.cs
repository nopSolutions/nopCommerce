using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.PayPalSmartPaymentButtons.Models;
using Nop.Plugin.Payments.PayPalSmartPaymentButtons.Services;
using Nop.Services.Payments;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Payments.PayPalSmartPaymentButtons.Components
{
    /// <summary>
    /// Represents the view component to add script to pages
    /// </summary>
    [ViewComponent(Name = Defaults.SCRIPT_VIEW_COMPONENT_NAME)]
    public class ScriptViewComponent : NopViewComponent
    {
        #region Fields

        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly ServiceManager _serviceManager;

        #endregion

        #region Ctor

        public ScriptViewComponent(IPaymentPluginManager paymentPluginManager,
            IStoreContext storeContext,
            IWorkContext workContext,
            ServiceManager serviceManager)
        {
            _paymentPluginManager = paymentPluginManager;
            _storeContext = storeContext;
            _workContext = workContext;
            _serviceManager = serviceManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <param name="widgetZone">Widget zone name</param>
        /// <param name="additionalData">Additional data</param>
        /// <returns>View component result</returns>
        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            if (!_paymentPluginManager.IsPluginActive(Defaults.SystemName, _workContext.CurrentCustomer, _storeContext.CurrentStore.Id))
                return Content(string.Empty);

            if (!widgetZone.Equals(PublicWidgetZones.CheckoutPaymentInfoTop) &&
                !widgetZone.Equals(PublicWidgetZones.OpcContentBefore) &&
                !widgetZone.Equals(PublicWidgetZones.ProductDetailsTop) &&
                !widgetZone.Equals(PublicWidgetZones.OrderSummaryContentBefore))
            {
                return Content(string.Empty);
            }

            var model = new ScriptModel { ScriptUrl = _serviceManager.GetScriptUrl() };
            return View("~/Plugins/Payments.PayPalSmartPaymentButtons/Views/Script.cshtml", model);
        }

        #endregion
    }
}